namespace gemini.dynamic.mcp.Models;

public class QueryParameters : DbConnectionParameters
{
    /// <summary>The T-SQL query to execute.</summary>
    public string SqlQuery { get; set; } = string.Empty;
}
