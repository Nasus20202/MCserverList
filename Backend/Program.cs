using ServerList.Database;
using ServerList.Database.Entities;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using ServerList;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader()); // Todo: Change this to allow only website

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
}

ServerInfoUpdater.Start();

app.Run();
