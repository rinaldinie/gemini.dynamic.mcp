using Dapper;
using Incaricotech.Wms.DynamicMcp.Models;
using McpDotNet.Server;
using Microsoft.Data.SqlClient;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Incaricotech.Wms.DynamicMcp
{
    /// <summary>
    /// Servizio MCP che espone i tool all'agente.
    /// </summary>
    public class DynamicSqlServerMcpService
    {
        /// <summary>
        /// Questo attributo espone il metodo come Tool MCP. Gemini leggerà la descrizione
        /// per capire quando e come utilizzarlo.
        /// </summary>
        [McpTool(
            "execute_dynamic_sql_query",
            "Connette a un SQL Server specificato dinamicamente ed esegue una query SELECT in sola lettura."
        )]
        public async Task<string> ExecuteDynamicQueryAsync(DynamicSqlQueryParameters parameters)
        {
            // 1. Validazione base per bloccare tentativi di modifica dati (Clean Code practice)
            if (!parameters.SqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            {
                return "Errore: Sono consentite solo query SELECT per questioni di sicurezza.";
            }

            // 2. Costruzione sicura della stringa di connessione
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder
            {
                DataSource = parameters.ServerAddress,
                InitialCatalog = parameters.DatabaseName,
                UserID = parameters.Username,
                Password = parameters.Password,
                TrustServerCertificate = true, // Necessario per ambienti di sviluppo/container Docker
                ConnectTimeout = 10
            };

            QueryResultDto result = new QueryResultDto();

            // 3. Esecuzione della query con gestione sicura delle risorse (using)
            try
            {
                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();

                    // Utilizziamo Dapper per estrarre i dati in formato dinamico e li serializziamo.
                    // Nota: In questo caso isolato usiamo dynamic perché la struttura dei dati di 
                    // ritorno non è nota a priori (essendo la query arbitraria).
                    var queryData = await connection.QueryAsync<dynamic>(parameters.SqlQuery);

                    result.Success = true;
                    result.JsonData = System.Text.Json.JsonSerializer.Serialize(queryData);
                }
            }
            catch (SqlException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Errore di connessione o esecuzione SQL: {ex.Message}";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Errore generico: {ex.Message}";
            }

            // 4. Restituiamo una risposta strutturata in JSON che Gemini possa parsare facilmente
            return System.Text.Json.JsonSerializer.Serialize(result);
        }
    }
}