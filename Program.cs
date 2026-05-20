// Program.cs
using Incaricotech.Wms.DynamicMcp.Services;
using McpDotNet.Configuration;
using McpDotNet.Server;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Incaricotech.Wms.DynamicMcp;

internal class Program
{
    static async Task Main(string[] args)
    {
        // 1. Configurazione della Dependency Injection
        ServiceCollection services = new ServiceCollection();
        services.AddSingleton();

        ServiceProvider serviceProvider = services.BuildServiceProvider();

        // 2. Inizializzazione del Server MCP
        // Configura il server per comunicare tramite Standard IO
        IMcpServer server = new McpServerBuilder()
            .WithStdioTransport()
            .WithServiceProvider(serviceProvider)
            .Build();

        // 3. Registrazione esplicita del Tool
        IDynamicSqlServerMcpService sqlService = serviceProvider.GetRequiredService();

        server.RegisterTool(
            name: "execute_dynamic_sql_query",
            description: "Connette a un SQL Server dinamicamente ed esegue una query SELECT.",
            handler: async (Models.DynamicSqlQueryParameters parameters) =>
                await sqlService.ExecuteDynamicQueryAsync(parameters)
        );

        // 4. Avvio in ascolto
        // Il server resterà in esecuzione intercettando i messaggi JSON-RPC in entrata
        await server.StartAsync();
    }
}