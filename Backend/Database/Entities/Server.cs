namespace ServerList.Database.Entities;

public class Server
{
    public Guid ServerId { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
    public string Version { get; set; }
    public bool Premium { get; set; }
    public string About { get; set; }
    public int Players { get; set; } = 0;
    public string Motd { get; set; } = String.Empty;
    public DateTime LastOnline { get; set; } = DateTime.Now;
    public DateTime SubmitDate { get; set; } = DateTime.Now;

    public List<Tag> Tags { get; set; } = new List<Tag>();

    public Server(string name, string url, bool premium = true, string about = "", string version = "1.0")
    {
        this.Name = name;
        this.Url = url;
        this.Premium = premium;
        this.Version = version;
        this.About = about;
    }
}