namespace gemini.dynamic.mcp.Models;

public class QueryParameters : DbConnectionParameters
{
    public string SqlQuery { get; set; } = string.Empty;
}
