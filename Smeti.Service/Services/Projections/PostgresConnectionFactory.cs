using Npgsql;
using Smeti.Domain.Projections.Services;

namespace Smeti.Service.Services.Projections;

public sealed class PostgresConnectionFactory
    : IPostgresConnectionFactory
{
    private readonly string _connectionString;

    public PostgresConnectionFactory(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("EventStore");
    }

    public NpgsqlConnection Create() => new (_connectionString);
}