using Npgsql;

namespace Smeti.Domain.Projections.Services;

public interface IPostgresConnectionFactory
{
    NpgsqlConnection Create();
}