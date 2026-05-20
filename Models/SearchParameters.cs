namespace gemini.dynamic.mcp.Models;

public class SearchParameters : DbConnectionParameters
{
    public string SearchPattern { get; set; } = string.Empty;
}
