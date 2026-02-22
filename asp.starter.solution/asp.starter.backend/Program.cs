using asp.starter.backend;
using asp.starter.backend.InfrastructureModule.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

app.ConfigurePipeline();

app.MapGet("/db-ping", async (AppDbContext db) =>
{
    try
    {
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();
        return Results.Ok(new { ok = true });
    }
    catch (Exception ex)
    {
        return Results.Problem(
            title: ex.GetType().Name,
            detail: ex.Message
        );
    }
});


app.Run();