using System;
using System.IO;
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
        var localPath = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "data");
        if (!Directory.Exists(localPath))
        {
            Directory.CreateDirectory(localPath);
        }
        DbPath = Path.Join(localPath, "servers.db");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}