using FlowForge.Application;
using FlowForge.Infrastructure;

namespace FlowForge.Api;

public class Program
{
  public static async Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    builder.Services
        .AddApplication()
        .AddInfrastructure();

    builder.Services.AddEndpointsApiExplorer();

    var app = builder.Build();

    app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));
    app.Run();
  }
}
