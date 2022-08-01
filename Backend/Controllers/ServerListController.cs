using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServerList.Database.Entities;

namespace ServerList.Controllers;

[ApiController]
[Route("[controller]")]
public class ServerListController : Controller
{
    [HttpGet]
    [Route("/servers/{page:int?}")]
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
            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(server.Image.Split(',')[1]);
            }
            catch (System.IndexOutOfRangeException)
            {
                // empty placeholder file
                bytes = Convert.FromBase64String(
                    "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");
            }

            return File(bytes, "image/png");
        }
    }

    [HttpGet]
    [Route("/servers/{id:guid}")]
    public IActionResult GetServerById(Guid id)
    {
        Server? server;
        using (var db = new Database.Database())
        {
            server = db.Servers.Include(s => s.Tags).FirstOrDefault(s => s.ServerId == id);
        }
        if (server == null)
            return NotFound();
        server.Image = $"{Request.Scheme}://{Request.Host}/servers/img/{server.ServerId}.png";
        return Json(server);
    }
    
    [HttpGet]
    [Route("/servers/random")]
    public IActionResult GetRandomServer()
    {
        Server server;
        using (var db = new Database.Database())
        {
            var servers = db.Servers.Where(s => DateTime.Now < s.LastOnline.AddDays(1) && s.Players > 0).Include(s => s.Tags).ToList();
            var random = new Random();
            if (servers.Count == 0)
                return Json(new Server("Not found", "https://developer.mozilla.org/pl/docs/Web/HTTP/Status/404", false, "No server found"));
            server = servers[random.Next(servers.Count)];
        }
        server.Image = $"{Request.Scheme}://{Request.Host}/servers/img/{server.ServerId}.png";
        return Json(server);
    }
    
    [HttpGet]
    [Route("/servers/count")]
    public IActionResult GetServerCount()
    {
        int count;
        using (var db = new Database.Database())
        {
            count = db.Servers.Where(s => DateTime.Now < s.LastOnline.AddDays(1)).Include(s => s.Tags).ToList().Count;
        }
        return Ok(count);
    }
    
}