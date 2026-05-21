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
    /// Ping tool for testing discovery.
    /// </summary>
    [McpTool]
    [Description("A simple ping tool to verify server connectivity and tool discovery.")]
    public Task<string> Ping(
        [Description("A message to echo back.")]
        string message)
    {
        return Task.FromResult($"Server is ready. Echo: {message}");
    }

    /// <summary>
    /// Connects to a SQL Server dynamically and executes a SQL query.
    /// </summary>
    /// <param name="parameters">The query parameters.</param>
    /// <returns>A serialized string of the result.</returns>
    [McpTool]
    [Description("Executes a SQL query on a SQL Server database using dynamic credentials.")]
    public async Task<string> ExecuteDynamicSqlQuery(
        [Description("Query parameters including connection details and SQL query.")]
        QueryParameters parameters)
    {
        QueryResultDto result = await _service.ExecuteQueryAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Lists all tables in the database.
    /// </summary>
    [McpTool]
    [Description("Lists all tables in the specified SQL Server database.")]
    public async Task<string> ListTables(
        [Description("Connection parameters for the database.")]
        DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.ListTablesAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Describes the schema of a specific table.
    /// </summary>
    [McpTool]
    [Description("Describes the schema (columns, types) of a specific table in the database.")]
    public async Task<string> DescribeTable(
        [Description("Parameters including the table name to describe.")]
        TableParameters parameters)
    {
        QueryResultDto result = await _service.DescribeTableAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Searches for tables matching a pattern.
    /// </summary>
    [McpTool]
    [Description("Searches for tables in the database that match a specific name pattern.")]
    public async Task<string> SearchTables(
        [Description("Parameters including the search pattern (e.g., %Users%).")]
        SearchParameters parameters)
    {
        QueryResultDto result = await _service.SearchTablesAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Gets the top 10 slow queries based on CPU time.
    /// </summary>
    [McpTool]
    [Description("Gets the top 10 slow queries in the database based on CPU time.")]
    public async Task<string> GetSlowQueries(
        [Description("Connection parameters for the database.")]
        DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.GetSlowQueriesAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Gets active locks and blocking sessions.
    /// </summary>
    [McpTool]
    [Description("Gets active locks and identifies blocking chains in the database.")]
    public async Task<string> GetActiveLocks(
        [Description("Connection parameters for the database.")]
        DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.GetActiveLocksAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Gets fragmented indexes (avg fragmentation > 10%).
    /// </summary>
    [McpTool]
    [Description("Identifies fragmented indexes in the database (fragmentation > 10%).")]
    public async Task<string> GetIndexFragmentation(
        [Description("Connection parameters for the database.")]
        DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.GetIndexFragmentationAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Gets missing index recommendations.
    /// </summary>
    [McpTool]
    [Description("Retrieves missing index recommendations based on SQL Server DMVs.")]
    public async Task<string> GetMissingIndexes(
        [Description("Connection parameters for the database.")]
        DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.GetMissingIndexesAsync(parameters);
        return JsonSerializer.Serialize(result);
    }

    /// <summary>
    /// Gets logical and physical database size.
    /// </summary>
    [McpTool]
    [Description("Gets the logical and physical size of the current database files.")]
    public async Task<string> GetDatabaseSize(
        [Description("Connection parameters for the database.")]
        DbConnectionParameters parameters)
    {
        QueryResultDto result = await _service.GetDatabaseSizeAsync(parameters);
        return JsonSerializer.Serialize(result);
    }
}
