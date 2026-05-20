# C# SQL Server Dynamic MCP Suite

A high-performance, unrestricted Model Context Protocol (MCP) server for Microsoft SQL Server. This server enables LLM agents (like Gemini CLI, Claude Desktop, or Cursor) to explore, search, and fully interact with any SQL Server database using dynamic credentials passed per-request.

## Features

- 🛠 **Full Tool Suite**:
  - `execute_query`: Unrestricted execution of any SQL query (SELECT, INSERT, UPDATE, DELETE, DDL).
  - `list_tables`: Discover all tables in the connected database.
  - `describe_table`: Detailed schema inspection (columns, types, nullability).
  - `search_tables`: Fuzzy search for table names.
- 🔑 **Dynamic Credentials**: Credentials are not stored; the AI provides Server, Database, Username, and Password for each session or request.
- ⚡ **Built on Official SDK**: Uses the official `ModelContextProtocol.Sdk` and .NET 9.0.
- 🏗 **Enterprise Clean Code**: Implemented with Dependency Inversion, explicit DTOs, and Dapper for efficiency.

## Installation

### Prerequisite: .NET 9 SDK
If you don't have .NET 9 installed, download it from [dot.net](https://dotnet.microsoft.com/download/dotnet/9.0).

### Option 1: Using Gemini CLI (Recommended)
Add this server to your Gemini CLI environment:
```bash
gemini mcp add sql-server dotnet run --project "/path/to/gemini.dynamic.mcp.csproj"
```

### Option 2: Manual Configuration
Add the server to your `~/.gemini/settings.json` or `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "sql-server-dynamic": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "C:/path/to/gemini.dynamic.mcp.csproj"
      ]
    }
  }
}
```

## Tools Documentation

### 1. `execute_query`
Executes any SQL command. 
- **Parameters**: `ServerAddress`, `DatabaseName`, `Username`, `Password`, `SqlQuery`.

### 2. `list_tables`
Lists all base tables in the current database.
- **Parameters**: `ServerAddress`, `DatabaseName`, `Username`, `Password`.

### 3. `describe_table`
Returns columns, data types, and nullability for a specific table.
- **Parameters**: `ServerAddress`, `DatabaseName`, `Username`, `Password`, `TableName`.

### 4. `search_tables`
Searches for tables using a LIKE pattern.
- **Parameters**: `ServerAddress`, `DatabaseName`, `Username`, `Password`, `SearchPattern` (e.g., `%Users%`).

## Security Note
This server has **no query restrictions**. It allows the agent to perform destructive operations if provided with high-privilege credentials. It is recommended to:
1. Provide the AI with a **Read-Only** SQL user where possible.
2. Only provide administrative credentials for specific maintenance tasks.
3. Run the server in a secure, isolated environment.

## License
MIT
