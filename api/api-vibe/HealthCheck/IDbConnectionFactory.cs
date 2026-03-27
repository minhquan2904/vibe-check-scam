using System.Data;
using System.Data.Common;

namespace api_vibe.HealthCheck;

/// <summary>
/// Factory interface for creating database connections.
/// Implement this for your specific DB provider (SqlServer, Oracle, Postgres, etc.)
/// and register in DI to enable database health checks.
/// If not registered, the DatabaseHealthCheck will return UP as a stub.
/// </summary>
public interface IDbConnectionFactory
{
    DbConnection CreateConnection();
}
