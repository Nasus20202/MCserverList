namespace ServerList.Database.Entities;

public class Tag
{
    public Guid TagId { get; set; }
    public string Name { get; set; }

    public Guid ServerId { get; set; }

    public Tag(string name)
    {
        this.Name = name.ToLower();
    }
}