using gemini.dynamic.mcp.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;

namespace gemini.dynamic.mcp;

internal static class Program
{
    static async Task Main(string[] args)
    {
        HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

        // 1. Dependency Injection Configuration
        builder.Services.AddSingleton<IDynamicSqlServerMcpService, DynamicSqlServerMcpService>();

        // 2. MCP Server Configuration
        builder.Services.AddMcpServer()
            .WithStdioServerTransport()
            .WithTools();

        using IHost host = builder.Build();

        // 3. Start the server
        // The server will listen on Standard IO for JSON-RPC messages
        await host.RunAsync();
    }
}
