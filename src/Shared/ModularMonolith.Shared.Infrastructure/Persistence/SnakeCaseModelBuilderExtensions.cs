using System.Text;

using Microsoft.EntityFrameworkCore;

namespace ModularMonolith.Shared.Infrastructure.Persistence;

public static class SnakeCaseModelBuilderExtensions
{
    public static ModelBuilder UseSnakeCaseNames(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();

            if (tableName is not null)
            {
                entityType.SetTableName(ToSnakeCase(tableName));
            }

            foreach (var index in entityType.GetIndexes())
            {
                var indexName = index.GetDatabaseName();

                if (indexName is not null)
                {
                    index.SetDatabaseName(ToSnakeCase(indexName));
                }
            }
        }

        return modelBuilder;
    }

    public static string ToSnakeCase(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return name;
        }

        var builder = new StringBuilder(name.Length + 8);

        for (var i = 0; i < name.Length; i++)
        {
            var current = name[i];

            if (!char.IsUpper(current))
            {
                builder.Append(current);
                continue;
            }

            var previous = i > 0 ? name[i - 1] : default;
            var startsNewWord = i > 0
                && previous != '_'
                && (!char.IsUpper(previous) || (i + 1 < name.Length && char.IsLower(name[i + 1])));

            if (startsNewWord)
            {
                builder.Append('_');
            }

            builder.Append(char.ToLowerInvariant(current));
        }

        return builder.ToString();
    }
}
