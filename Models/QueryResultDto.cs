namespace gemini.dynamic.mcp.Models;

public class QueryResultDto
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}
