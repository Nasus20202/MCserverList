﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerList.Database.Entities;

namespace ServerList.Controllers;

[ApiController]
[Route("[controller]")]
public class ServerListController : Controller
{
    [HttpGet]
    [Route("/servers/{page:int}")]
    [Route("/servers")]
    public IActionResult GetServers(int page = 0, [FromQuery] int amount = 25)
    {
        var servers = new List<Server>();
        using (var db = new Database.Database())
        {
            servers = db.Servers.Where(s => DateTime.Now < s.LastOnline.AddDays(1)).Include(s => s.Tags).OrderByDescending(s => s.Players).Skip(page * amount).Take(amount).ToList();
        }
        foreach (var server in servers)
        {
            server.Image = $"{Request.Scheme}://{Request.Host}/servers/img/{server.ServerId}.png";
        }
        return Json(servers);
    }
    
    public class CreateServerModel
    {
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public bool Premium { get; set; } = false;
        public string About { get; set; } = "";
        public List<string>? Tags { get; set; } = null;
    }

    [HttpPost]
    [Route("/servers")]
    public IActionResult CreateServer([FromBody] CreateServerModel model)
    {
        var server = new Server(model.Name, model.Url.Trim(), model.Premium, model.About);
        int dots = server.Url.Split('.').Length - 1;
        if (dots is < 1 or > 2 || server.Url.Length > 2048)
            return BadRequest(server.Url + " is not a valid URL");
        if (server.About.Length > 8192)
            return BadRequest("About server should can have up to 8192 characters (you have " + server.About.Length + ")");
        using (var db = new Database.Database())
        {
            if (db.Servers.Where(s => s.Url == server.Url).ToList().Any())
            {
                return Conflict(server.Url + " is already in our database");
            }
            var serverInfo = ServerInfoUpdater.GetServerInfo(server);
            if (serverInfo is null)
                return NotFound("Server " + server.Url + " is offline or query is not enabled");
            server.Image = serverInfo.Image; 
            server.Motd = serverInfo.Motd;
            server.Version = serverInfo.Version;
            server.Players = serverInfo.Players;
            server.MaxPlayers = serverInfo.MaxPlayers;
            db.Servers.Add(server);
            if (model.Tags is not null)
            {
                foreach (var tagName in model.Tags)
                {
                    var tag = new Tag(tagName);
                    server.Tags.Add(tag);
                }
            }
            db.SaveChanges();
        }
        //ServerInfoUpdater.UpdateServerInfo(server);

        return Created(server.Name, server);
    }

    [HttpGet]
    [Route("/servers/img/{filename}")]
    public IActionResult GetServerIcon(string filename)
    {
        filename = filename.Split(".")[0];
        using (var db = new Database.Database())
        {
            Guid guid;
            if (!Guid.TryParse(filename, out guid))
                return BadRequest();
            var server = db.Servers.FirstOrDefault(s => s.ServerId == guid);
            if (server is null)
                return NotFound();
            byte[] bytes = Convert.FromBase64String(server.Image.Split(',')[1]);
            return File(bytes, "image/png");
        }
    }
}