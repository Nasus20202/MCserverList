using Microsoft.AspNetCore.Mvc;
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
            servers = db.Servers.Include(s => s.Tags).OrderByDescending(s => s.Players).Skip(page * amount).Take(amount).ToList();
        }

        return Json(servers);
    }

    [HttpPost]
    [Route("/servers")]
    public IActionResult CreateServer([FromBody] string name,[FromBody] string url,[FromBody] bool premium = true,[FromBody] string about = "",[FromBody] List<string>? tags = null)
    {
        url = url.Trim();
        var server = new Server(name, url, premium, about);
        int dots = url.Split('.').Length - 1;
        if (dots is < 1 or > 2)
            return BadRequest("Not a valid URL");
        using (var db = new Database.Database())
        {
            if (db.Servers.Where(s => s.Url == url).ToList().Any())
            {
                return Conflict();
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
            if (tags is not null)
            {
                foreach (var tagName in tags)
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