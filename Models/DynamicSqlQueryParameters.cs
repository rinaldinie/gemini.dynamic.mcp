namespace Incaricotech.Wms.DynamicMcp.Models;

public class DynamicSqlQueryParameters
{
    public string ServerAddress { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string SqlQuery { get; set; } = string.Empty;
}

public class QueryResultDto
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
}