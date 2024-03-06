using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerList.Database.Entities;

namespace ServerList;

// Source: https://gist.github.com/csh/2480d14fbbb33b4bbae3
public class ServerInfoUpdater
{
    private static NetworkStream _stream;
    private static List<byte> _buffer;
    private static int _offset;


    public static void Start()
    {
        Task task = new Task(UpdaterThread);
        task.Start();
    }
    private static void UpdaterThread()
    {
        while (true)
        {
            List<Server> servers;
            using (var db = new Database.Database())
            {
                servers = db.Servers.ToList();
            }
            Console.WriteLine("Updating server info");
            foreach (var server in servers)
            {
                UpdateServerInfo(server);
            }
            Console.WriteLine("Server info updated");
            Thread.Sleep(3600000);
        }
    }
    
    public static void UpdateServerInfo(Server server)
    {
        var data = GetServerInfo(server);
        if (data is null)
        {
            return;
        }
        server.Players = data.Players;
        server.MaxPlayers = data.MaxPlayers;
        server.Version = data.Version;
        server.Motd = data.Motd;
        server.Image = data.Image;
        server.LastOnline = DateTime.Now;
        using var db = new Database.Database();
        db.Servers.Update(server);
        db.SaveChanges();
    }
    
    public static ServerData? GetServerInfo(Server server)
    {
        var client = new TcpClient();
        var task = client.ConnectAsync(server.Url, 25565);
        int timeout = 0;
        while (!task.IsCompleted && timeout < 30)
        {
            Thread.Sleep(100);
            timeout++;
        }
        if (!client.Connected)
        {
            return null;
        }
        _buffer = new List<byte>();
        _stream = client.GetStream();
        _stream.ReadTimeout = 1000;
        _stream.WriteTimeout = 1000;

        try{
            WriteVarInt(47);
            WriteString(server.Url);
            WriteShort(25565);
            WriteVarInt(1);
            Flush(0);
            Flush(0);
        }
        catch (Exception e)
        {
            Console.WriteLine($"{server.Url} : {e.Message}");
            return null;
        }
        string json;
        byte[] data = new byte[1024];
        using (MemoryStream ms = new MemoryStream())
        {
            int numBytesRead ;
            try
            {
                while ((numBytesRead = _stream.Read(data, 0, data.Length)) > 0)
                {
                    ms.Write(data, 0, numBytesRead);
                }
            }
            catch (System.IO.IOException)
            {
                // Timeout error - data is  already transferred
            }

            json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);
        }
        if (json.Contains("{")) {
            json = json.Substring(json.IndexOf("{")); // remove random characters at the start
        }
        ServerData? serverData;
        try
        {
            JObject? response = JsonConvert.DeserializeObject<JObject>(json);
            var version = response?["version"]["name"];
            var players = response?["players"]["online"];
            var maxPlayers = response?["players"]["max"];
            var desc = response?["description"];
            string motd = desc.ToString();
            if (desc.ToString().Contains("{"))
            {
                var jobject = desc as JObject;
                if (jobject.ContainsKey("extra"))
                    motd = diveIntoMotd(jobject, "") + jobject["text"];
                else if (jobject.ContainsKey("text"))
                    motd = jobject["text"].ToString();
            }
            var image = response?["favicon"];
            if (image == null)
                image = JsonConvert.SerializeObject("");
            serverData = new ServerData(version.Value<string>(), motd, players.Value<int>(), maxPlayers.Value<int>(), image.Value<string>());
        }
        catch (Exception e)
        {
            Console.WriteLine(server.Url + " : " + e.Message);
            return null; //Data is corrupted... or to be clear, my code is bugged
        }

        return serverData;
    }

    internal static char getColorChar(string color)
    {
        switch (color)
        {
            case "black":
                return '0';
            case "dark_blue":
                return '1';
            case "dark_green":
                return '2';
            case "dark_aqua":
                return '3';
            case "dark_red":
                return '4';
            case "dark_purple":
                return '5';
            case "gold":
                return '6';
            case "gray":
                return '7';
            case "dark_gray":
                return '8';
            case "blue":
                return '9';
            case "green":
                return 'a';
            case "aqua":
                return 'b';
            case "red":
                return 'c';
            case "light_purple":
                return 'd';
            case "yellow":
                return 'e';
            case "white":
                return 'f';
        }
        return 'o';
    }

    internal static string diveIntoMotd(JObject obj, string value)
    {
        if (obj.ContainsKey("extra"))
        {
            foreach (var obj2 in obj["extra"])
            {
                if (!obj2.HasValues) {
                    value += '\n';
                    continue;
                }
                if (obj2["color"] != null)
                    value += $"\u00A7{getColorChar(obj2["color"].ToString())}";
                if(obj2["bold"] != null && obj2["bold"].ToString() == "true")
                    value += "\u00A7l";
                value += obj2["text"];
                var obj3 = obj2 as JObject;
                if (obj3 == null)
                    return value;
                if (obj3.ContainsKey("extra"))
                {
                    value = diveIntoMotd((JObject)obj3, value);
                }
            }
        }
        return value + obj["text"];;
    }

    public class ServerData
    {
        public string Version { get; set; }
        public string Motd { get; set; }
        public int Players { get; set; }
        public int MaxPlayers { get; set; }
        public string Image { get; set; }

        public ServerData(string version, string motd, int players, int maxPlayers, string image)
        {
            this.Version = version;
            this.Motd = motd;
            this.Players = players;
            this.MaxPlayers = maxPlayers;
            this.Image = image;
        }
    }
    
    internal static byte ReadByte(byte[] buffer)
        {
            var b = buffer[_offset];
            _offset += 1;
            return b;
        }

        internal static byte[] Read(byte[] buffer, int length)
        {
            var data = new byte[length];
            Array.Copy(buffer, _offset, data, 0, length);
            _offset += length;
            return data;
        }

        internal static int ReadVarInt(byte[] buffer)
        {
            var value = 0;
            var size = 0;
            int b;
            while (((b = ReadByte(buffer)) & 0x80) == 0x80)
            {
                value |= (b & 0x7F) << (size++*7);
                if (size > 5)
                {
                    throw new IOException("This VarInt is an imposter!");
                }
            }
            return value | ((b & 0x7F) << (size*7));
        }

        internal static string ReadString(byte[] buffer, int length)
        {
            var data = Read(buffer, length);
            return Encoding.UTF8.GetString(data);
        }

        internal static void WriteVarInt(int value)
        {
            while ((value & 128) != 0)
            {
                _buffer.Add((byte) (value & 127 | 128));
                value = (int) ((uint) value) >> 7;
            }
            _buffer.Add((byte) value);
        }

        internal static void WriteShort(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
        }

        internal static void WriteString(string data)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            WriteVarInt(buffer.Length);
            _buffer.AddRange(buffer);
        }

        internal static void Write(byte b)
        {
            _stream.WriteByte(b);
        }

        internal static void Flush(int id = -1)
        {
            var buffer = _buffer.ToArray();
            _buffer.Clear();

            var add = 0;
            var packetData = new[] {(byte) 0x00};
            if (id >= 0)
            {
                WriteVarInt(id);
                packetData = _buffer.ToArray();
                add = packetData.Length;
                _buffer.Clear();
            }

            WriteVarInt(buffer.Length + add);
            var bufferLength = _buffer.ToArray();
            _buffer.Clear();

            _stream.Write(bufferLength, 0, bufferLength.Length);
            _stream.Write(packetData, 0, packetData.Length);
            _stream.Write(buffer, 0, buffer.Length);
        }
}