namespace gemini.dynamic.mcp.Models;

public class DbConnectionParameters
{
    /// <summary>The IP address or hostname of the SQL Server.</summary>
    public string ServerAddress { get; set; } = string.Empty;

    /// <summary>The name of the database to connect to.</summary>
    public string DatabaseName { get; set; } = string.Empty;

    /// <summary>The username for SQL Server authentication.</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>The password for SQL Server authentication.</summary>
    public string Password { get; set; } = string.Empty;
}
