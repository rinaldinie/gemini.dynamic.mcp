using System.ComponentModel;
using System.Text.Json;
using gemini.dynamic.mcp.Models;
using gemini.dynamic.mcp.Services;
using ModelContextProtocol.Server;

namespace gemini.dynamic.mcp.Tools;

/// <summary>
/// Wrapper class for the MCP tool.
/// </summary>
[McpToolType]
public class SqlServerTools
{
    private readonly IDynamicSqlServerMcpService _service;

    public SqlServerTools(IDynamicSqlServerMcpService service)
    {
        _service = service;
    }

    /// <summary>
    /// Connects to a SQL Server dynamically and executes a SQL query.
    /// </summary>
    /// <param name="parameters">The query parameters.</param>
    /// <returns>A serialized string of the result.</returns>
    [McpTool]
    [Description("Executes a SQL query on a SQL Server database using dynamic credentials.")]
    public async Task<string> ExecuteDynamicSqlQuery(QueryParameters parameters)
    {
        QueryResultDto result = await _service.ExecuteQueryAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Lists all tables in the database.
    /// </summary>
    [McpTool]
    [Description("Lists all tables in the specified SQL Server database.")]
    public async Task<string> ListTables(DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.ListTablesAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Describes the schema of a specific table.
    /// </summary>
    [McpTool]
    [Description("Describes the schema (columns, types) of a specific table in the database.")]
    public async Task<string> DescribeTable(TableParameters parameters)
    {
        QueryResultDto result = await _service.DescribeTableAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Searches for tables matching a pattern.
    /// </summary>
    [McpTool]
    [Description("Searches for tables in the database that match a specific name pattern.")]
    public async Task<string> SearchTables(SearchParameters parameters)
    {
        QueryResultDto result = await _service.SearchTablesAsync(parameters);
        return JsonSerializer.Serialize(result);
    }
}
