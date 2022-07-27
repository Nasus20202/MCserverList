using ServerList.Database;
using ServerList.Database.Entities;
using System.Collections.Generic;
using ServerList;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var db = new Database())
{
    db.Database.EnsureCreated();
    /*db.Servers.Add(new Server("Krzyśland", "krzysland.tk", false, "Krzyśland - serwer Minecraft", "1.18.2 Fabric"));
    db.Servers.Add(new Server("Hypixel", "mc.hypixel.net", true, "Hypixel MC server", "1.8 - 1.19"));
    db.SaveChanges();*/
    var server = db.Servers.FirstOrDefault(s => s.Name == "Krzyśland");
    /*server.Tags.Add(new Tag("Survival"));
    server.Tags.Add(new Tag("Building"));
    server.Tags.Add(new Tag("Mods"));
    server.Tags.Add(new Tag("Fabric"));
    var server2 = db.Servers.FirstOrDefault(s => s.Name == "Hypixel");
    server2.Tags.Add(new Tag("Minigames"));
    server2.Tags.Add(new Tag("Skyblock"));
    server2.Tags.Add(new Tag("Bedwars"));
    db.SaveChanges();*/
    ServerInfoUpdater.UpdateServerInfo(server);
}


//app.Run();
