using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using TastifyAPI.Data;
using TastifyAPI.Entities;
using TastifyAPI.Services;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using TastifyAPI.Extensions;
using Microsoft.Extensions.DependencyInjection;
using TastifyAPI.IServices;
using TastifyAPI.BuildInjections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<TastifyDbSettings>(
    builder.Configuration.GetSection("ConnectionStrings"));

// Register MongoClient
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TastifyDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

// Register IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<TastifyDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7206")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(_ => true)
            .AllowCredentials();
    });
});

builder.Services.AddServices();
builder.Services.AddAutoMapperProfiles();
builder.Services.AddMongoCollections();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerWithJwtAuthorization();
builder.Services.AddLogging();
builder.Services.AddSetSecurity(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tastify API V1");
        c.RoutePrefix = "";
        c.OAuthClientId("swagger");
        c.OAuthAppName("Swagger UI");
    });
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
