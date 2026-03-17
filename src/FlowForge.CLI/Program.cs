using FlowForge.Application;
using FlowForge.Domain;
using FlowForge.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services
    .AddApplication()
    .AddInfrastructure();

Console.WriteLine("FlowForge CLI is ready.");
Console.WriteLine($"Domain marker: {typeof(AssemblyReference).FullName}");

if (args.Contains("wait", StringComparer.OrdinalIgnoreCase))
{
    Console.WriteLine("Waiting until the container is stopped...");
    await Task.Delay(Timeout.InfiniteTimeSpan);
}
