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
            servers = db.Servers.Include(s => s.Tags).OrderBy(s => s.Players).Skip(page * amount).Take(amount).ToList();
        }

        return Json(servers);
    }

    [HttpPost]
    [Route("/servers")]
    public IActionResult CreateServer(string name, string url, bool premium = true, string about = "", List<string>? tags = null)
    {
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
            db.Servers.Add(server);
            if (tags is not null)
            {
                foreach (var tagName in tags)
                {
                    var tag = new Tag(tagName);
                    server.Tags.Add(tag);
                }
            }
            //db.SaveChanges();
        }

        return Created(server.Name, server);
    }
}