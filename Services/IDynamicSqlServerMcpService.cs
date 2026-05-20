using Incaricotech.Wms.DynamicMcp.Models;

namespace Incaricotech.Wms.DynamicMcp.Services;

public interface IDynamicSqlServerMcpService
{
    Task ExecuteDynamicQueryAsync(DynamicSqlQueryParameters parameters);
}