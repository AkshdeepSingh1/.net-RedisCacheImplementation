using Microsoft.EntityFrameworkCore;
using RedisCacheExample.DbContext;
using RedisCacheExample.Interface;
using RedisCacheExample.RedisCacheProvider;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DbApplication>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetSection("RedisConnection").GetValue<string>("Configuration");
});
builder.Services.AddSingleton<IRedisCacheProvider, RedisCacheProvider>();

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

app.Run();
