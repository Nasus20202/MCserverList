using System;
using System.Net.Sockets;
using System.Threading;
using ServerList.Database.Entities;

namespace ServerList;

public class ServerInfoUpdater
{
    public static void UpdateServerInfo(Server server)
    {
        var client = new TcpClient();
        var task = client.ConnectAsync(server.Url, 25565);
        int timeout = 0;
        while (timeout < 6 || !task.IsCompleted)
        {
            Thread.Sleep(500);
            timeout++;
        }
        if (!client.Connected)
        {
           Console.WriteLine("Can't connect to " + server.Url); 
        }
        else
        {
            Console.WriteLine("Connected to " + server.Url);
        }
    }
}