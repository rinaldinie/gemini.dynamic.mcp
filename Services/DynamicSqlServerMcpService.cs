using Dapper;
using gemini.dynamic.mcp.Models;
using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace gemini.dynamic.mcp.Services;

public class DynamicSqlServerMcpService : IDynamicSqlServerMcpService
{
    private string BuildConnectionString(DbConnectionParameters parameters)
    {
        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
        {
            DataSource = parameters.ServerAddress,
            InitialCatalog = parameters.DatabaseName,
            UserID = parameters.Username,
            Password = parameters.Password,
            TrustServerCertificate = true, // Necessary for development/Docker environments
            ConnectTimeout = 10
        };
        return builder.ConnectionString;
    }

    public async Task<QueryResultDto> ExecuteQueryAsync(QueryParameters parameters)
    {
        try
        {
            // Execute the query using Dapper
            using (SqlConnection connection = new SqlConnection(BuildConnectionString(parameters)))
            {
                await connection.OpenAsync();

                // Execute query as IEnumerable<dynamic>
                // This allows for any query, including those that return results
                IEnumerable<dynamic> queryData = await connection.QueryAsync<dynamic>(parameters.SqlQuery);

                return new QueryResultDto
                {
                    Success = true,
                    JsonData = JsonSerializer.Serialize(queryData)
                };
            }
        }
        catch (SqlException ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"SQL Connection or Execution Error: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Generic Error: {ex.Message}"
            };
        }
    }

    public async Task<QueryResultDto> ListTablesAsync(DbConnectionParameters parameters)
    {
        const string query = "SELECT TABLE_SCHEMA, TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE'";

        try
        {
            using (SqlConnection connection = new SqlConnection(BuildConnectionString(parameters)))
            {
                await connection.OpenAsync();
                IEnumerable<dynamic> tables = await connection.QueryAsync<dynamic>(query);

                return new QueryResultDto
                {
                    Success = true,
                    JsonData = JsonSerializer.Serialize(tables)
                };
            }
        }
        catch (SqlException ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"SQL Error listing tables: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error listing tables: {ex.Message}"
            };
        }
    }

    public async Task<QueryResultDto> DescribeTableAsync(TableParameters parameters)
    {
        const string query = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName";

        try
        {
            using (SqlConnection connection = new SqlConnection(BuildConnectionString(parameters)))
            {
                await connection.OpenAsync();
                IEnumerable<dynamic> columns = await connection.QueryAsync<dynamic>(query, new { parameters.TableName });

                return new QueryResultDto
                {
                    Success = true,
                    JsonData = JsonSerializer.Serialize(columns)
                };
            }
        }
        catch (SqlException ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"SQL Error describing table {parameters.TableName}: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error describing table {parameters.TableName}: {ex.Message}"
            };
        }
    }

    public async Task<QueryResultDto> SearchTablesAsync(SearchParameters parameters)
    {
        const string query = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME LIKE @SearchPattern";

        try
        {
            using (SqlConnection connection = new SqlConnection(BuildConnectionString(parameters)))
            {
                await connection.OpenAsync();
                IEnumerable<dynamic> tables = await connection.QueryAsync<dynamic>(query, new { parameters.SearchPattern });

                return new QueryResultDto
                {
                    Success = true,
                    JsonData = JsonSerializer.Serialize(tables)
                };
            }
        }
        catch (SqlException ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"SQL Error searching tables with pattern {parameters.SearchPattern}: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error searching tables with pattern {parameters.SearchPattern}: {ex.Message}"
            };
        }
    }

    public async Task<QueryResultDto> GetSlowQueriesAsync(DbConnectionParameters parameters)
    {
        const string query = @"
            SELECT TOP 10 
                SUBSTRING(st.text, (qs.statement_start_offset/2) + 1,
                ((CASE statement_end_offset WHEN -1 THEN DATALENGTH(st.text) ELSE qs.statement_end_offset END 
                    - qs.statement_start_offset)/2) + 1) AS [Query Text],
                qs.total_worker_time/1000 AS [Total CPU Time (ms)],
                qs.execution_count AS [Execution Count],
                qs.total_elapsed_time/1000 AS [Total Elapsed Time (ms)],
                qs.max_elapsed_time/1000 AS [Max Elapsed Time (ms)],
                qs.last_execution_time AS [Last Execution Time]
            FROM sys.dm_exec_query_stats AS qs
            CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) AS st
            ORDER BY qs.total_worker_time DESC;";

        return await ExecuteDiagnosticQueryAsync(parameters, query, "slow queries");
    }

    public async Task<QueryResultDto> GetActiveLocksAsync(DbConnectionParameters parameters)
    {
        const string query = @"
            SELECT 
                tl.resource_type AS [Resource Type],
                tl.resource_database_id AS [Database ID],
                tl.request_mode AS [Mode],
                tl.request_status AS [Status],
                tl.request_session_id AS [Session ID],
                r.blocking_session_id AS [Blocking Session ID],
                st.text AS [Query Text]
            FROM sys.dm_tran_locks tl
            LEFT JOIN sys.dm_exec_requests r ON tl.request_session_id = r.session_id
            OUTER APPLY sys.dm_exec_sql_text(r.sql_handle) st
            WHERE tl.resource_database_id = DB_ID();";

        return await ExecuteDiagnosticQueryAsync(parameters, query, "active locks");
    }

    public async Task<QueryResultDto> GetIndexFragmentationAsync(DbConnectionParameters parameters)
    {
        const string query = @"
            SELECT 
                OBJECT_NAME(ips.object_id) AS [Table Name],
                i.name AS [Index Name],
                ips.avg_fragmentation_in_percent AS [Fragmentation %],
                ips.page_count AS [Page Count]
            FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'SAMPLED') ips
            INNER JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
            WHERE ips.avg_fragmentation_in_percent > 10
            ORDER BY ips.avg_fragmentation_in_percent DESC;";

        return await ExecuteDiagnosticQueryAsync(parameters, query, "index fragmentation");
    }

    public async Task<QueryResultDto> GetMissingIndexesAsync(DbConnectionParameters parameters)
    {
        const string query = @"
            SELECT TOP 20
                mid.statement AS [Table],
                mid.equality_columns AS [Equality Columns],
                mid.inequality_columns AS [Inequality Columns],
                mid.included_columns AS [Included Columns],
                migs.avg_user_impact AS [Impact %],
                migs.user_seeks AS [User Seeks]
            FROM sys.dm_db_missing_index_groups mig
            INNER JOIN sys.dm_db_missing_index_group_stats migs ON migs.group_handle = mig.index_group_handle
            INNER JOIN sys.dm_db_missing_index_details mid ON mig.index_handle = mid.index_handle
            WHERE mid.database_id = DB_ID()
            ORDER BY migs.avg_user_impact DESC;";

        return await ExecuteDiagnosticQueryAsync(parameters, query, "missing indexes");
    }

    public async Task<QueryResultDto> GetDatabaseSizeAsync(DbConnectionParameters parameters)
    {
        const string query = @"
            SELECT 
                name AS [Logical Name],
                size * 8 / 1024 AS [Size (MB)],
                type_desc AS [Type],
                physical_name AS [Physical Name]
            FROM sys.database_files;";

        return await ExecuteDiagnosticQueryAsync(parameters, query, "database size");
    }

    private async Task<QueryResultDto> ExecuteDiagnosticQueryAsync(DbConnectionParameters parameters, string query, string operationName)
    {
        try
        {
            using (SqlConnection connection = new SqlConnection(BuildConnectionString(parameters)))
            {
                await connection.OpenAsync();
                IEnumerable<dynamic> result = await connection.QueryAsync<dynamic>(query);

                return new QueryResultDto
                {
                    Success = true,
                    JsonData = JsonSerializer.Serialize(result)
                };
            }
        }
        catch (SqlException ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"SQL Error retrieving {operationName}: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new QueryResultDto
            {
                Success = false,
                ErrorMessage = $"Error retrieving {operationName}: {ex.Message}"
            };
        }
    }
}
