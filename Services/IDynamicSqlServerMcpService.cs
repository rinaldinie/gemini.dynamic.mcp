using gemini.dynamic.mcp.Models;

namespace gemini.dynamic.mcp.Services;

public interface IDynamicSqlServerMcpService
{
    Task<QueryResultDto> ExecuteQueryAsync(QueryParameters parameters);
    Task<QueryResultDto> ListTablesAsync(DbConnectionParameters parameters);
    Task<QueryResultDto> DescribeTableAsync(TableParameters parameters);
    Task<QueryResultDto> SearchTablesAsync(SearchParameters parameters);

    // --- Diagnostic & Performance Methods ---
    Task<QueryResultDto> GetSlowQueriesAsync(DbConnectionParameters parameters);
    Task<QueryResultDto> GetActiveLocksAsync(DbConnectionParameters parameters);
    Task<QueryResultDto> GetIndexFragmentationAsync(DbConnectionParameters parameters);
    Task<QueryResultDto> GetMissingIndexesAsync(DbConnectionParameters parameters);
    Task<QueryResultDto> GetDatabaseSizeAsync(DbConnectionParameters parameters);
}
