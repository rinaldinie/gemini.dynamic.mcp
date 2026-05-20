using gemini.dynamic.mcp.Models;

namespace gemini.dynamic.mcp.Services;

public interface IDynamicSqlServerMcpService
{
    Task<QueryResultDto> ExecuteQueryAsync(QueryParameters parameters);
    Task<QueryResultDto> ListTablesAsync(DbConnectionParameters parameters);
    Task<QueryResultDto> DescribeTableAsync(TableParameters parameters);
    Task<QueryResultDto> SearchTablesAsync(SearchParameters parameters);
}
