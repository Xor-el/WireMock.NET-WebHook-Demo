using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapPost("/dummy", ([FromServices] ILogger<Dummy> logger, Dummy request) =>
{
    logger.LogInformation("Hello, {Body}", JsonSerializer.Serialize(request));
    return TypedResults.Ok(new { request });
});

app.Run();

internal record Dummy([property: JsonPropertyName("message")] string? Message, [property: JsonPropertyName("status")] string? Status);

