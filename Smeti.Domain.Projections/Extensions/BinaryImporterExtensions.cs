using LanguageExt;
using Npgsql;
using NpgsqlTypes;

namespace Smeti.Domain.Projections.Extensions;

public static class BinaryImporterExtensions
{
    public static void WriteOption<T>(this NpgsqlBinaryImporter importer, Option<T> option, NpgsqlDbType dbType) =>
        Prelude.match(option, value => importer.Write(value, dbType), importer.WriteNull);

    public static void WriteOption<T>(this NpgsqlBinaryImporter importer, Option<T> option) =>
        Prelude.match(option, importer.Write, importer.WriteNull);
}