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
}
