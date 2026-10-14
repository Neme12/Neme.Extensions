using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Neme.Extensions.EntityFrameworkCore.SqlServer;

public static class SqlServerDatabaseFacadeExtensions
{
    extension(DatabaseFacade database)
    {
        public IdentityInsertScope CreateSqlServerIdentityInsertScope<TEntity>()
        {
            if (!database.IsSqlServer())
                throw new InvalidOperationException("The database provider is not SQL Server.");

            var serviceProvider = database.GetInfrastructure();
            var context = serviceProvider.GetRequiredService<ICurrentDbContext>().Context;
            var entityType = context.Model.FindEntityType(typeof(TEntity))
                ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity)}' is not part of the current DbContext model.");
            var tableName = entityType.GetTableName()
                ?? throw new InvalidOperationException($"Entity type '{typeof(TEntity)}' is not mapped to a table.");

            return database.CreateSqlServerIdentityInsertScope(tableName, entityType.GetSchema());
        }

        public IdentityInsertScope CreateSqlServerIdentityInsertScope(string tableName, string? schema = null)
        {
            if (!database.IsSqlServer())
                throw new InvalidOperationException("The database provider is not SQL Server.");

            ArgumentException.ThrowIfNullOrEmpty(tableName);

            var sqlGenerationHelper = database.GetInfrastructure().GetRequiredService<ISqlGenerationHelper>();
            var delimitedTableName = sqlGenerationHelper.DelimitIdentifier(tableName, schema);

#pragma warning disable EF1002
            database.ExecuteSqlRaw($"SET IDENTITY_INSERT {delimitedTableName} ON;");
#pragma warning restore EF1002

            return new IdentityInsertScope(database, delimitedTableName);
        }
    }

    public struct IdentityInsertScope : IDisposable
    {
        private DatabaseFacade _database;
        private readonly string _tableName;

        public IdentityInsertScope(DatabaseFacade database, string tableName)
        {
            _database = database;
            _tableName = tableName;
        }

        public void Dispose()
        {
            if (_database is not null)
            {
#pragma warning disable EF1002
                _database.ExecuteSqlRaw($"SET IDENTITY_INSERT {_tableName} OFF;");
#pragma warning restore EF1002
                _database = null!;
            }
        }
    }
}
