using System;
using System.Collections.Generic;
using ServerList.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace ServerList.Database;

public class Database : DbContext
{
    public DbSet<Server> Servers { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public string DbPath { get; set; }

    public Database()
    {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "servers.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}