using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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
            foreach (var server in servers)
            {
                UpdateServerInfo(server);
                Thread.Sleep(1000);
                //Console.WriteLine("Updated " + server.Name);
            }
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

        WriteVarInt(47);
        WriteString(server.Url);
        WriteShort(25565);
        WriteVarInt(1);
        Flush(0);
        
        Flush(0);
        string json;
        byte[] data = new byte[1024];
        _stream.ReadTimeout = 1000;
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
        if(json.Length > 5)
            json = json.Remove(0, 5);
        ServerData? serverData;
        try
        {
            JObject? response = JsonConvert.DeserializeObject<JObject>(json);
            var version = response?["version"]["name"];
            var players = response?["players"]["online"];
            var maxPlayers = response?["players"]["max"];
            var desc = response?["description"];
            // giga tasty 
            JToken motd = desc;
            if (desc.ToString().Contains("{"))
            {
                var descObj = (JObject)desc;
                if (descObj.ContainsKey("extra"))
                {
                    string value = "";
                    foreach (var obj in descObj["extra"])
                    {
                        value += obj["text"];
                    }

                    motd = JsonConvert.SerializeObject(value, Formatting.Indented);
                }
                else if (descObj.ContainsKey("text"))
                    motd = (JToken) descObj["text"];
            }

            var image = response?["favicon"];
            serverData = new ServerData(version.Value<string>(), motd.Value<string>(), players.Value<int>(), maxPlayers.Value<int>(), image.Value<string>());
        }
        catch (Exception e)
        {
            Console.WriteLine(server.Url + " : " + e.Message);
            return null; //Data is corrupted... or to be clear, my code is bugged
        }

        return serverData;
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