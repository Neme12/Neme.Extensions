using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations.Operations.Builders;
using Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal;

namespace Neme.Extensions.EntityFrameworkCore.SqlServer;

public static class SqlServerMigrationBuilderExtensions
{
    public static OperationBuilder<AddColumnOperation> AddSqlServerColumnWithInitialValue<T>(
        this MigrationBuilder migrationBuilder,
        string name,
        string table,
        string? type = null,
        bool? unicode = null,
        int? maxLength = null,
        bool nullable = false,
        string? initialValueSql = null)
    {
        if (!migrationBuilder.IsSqlServer())
            throw new InvalidOperationException("The database provider is not SQL Server.");

        var builder = migrationBuilder.AddColumn<T>(
            name: name,
            table: table,
            type: type,
            unicode: unicode,
            maxLength: maxLength,
            nullable: nullable || initialValueSql is not null);

        if (initialValueSql is null)
            return builder;

#pragma warning disable EF1001 // Internal EF Core API usage.
            var sqlGenerationHelper = new SqlServerSqlGenerationHelper(new());
            
            // Fill values in for existing rows
            migrationBuilder.Sql(
                InterpolatedString.Invariant($"""
                UPDATE {sqlGenerationHelper.DelimitIdentifier(table)}
                SET {sqlGenerationHelper.DelimitIdentifier(name)} = {initialValueSql ?? "NULL"};
                """));
#pragma warning restore EF1001 // Internal EF Core API usage.

        if (nullable)
            return builder;

        migrationBuilder.AlterColumn<T>(
            name: name,
            table: table,
            type: type,
            unicode: unicode,
            maxLength: maxLength,
            nullable: false,
            oldNullable: true,
            oldClrType: typeof(T),
            oldType: type);

        return builder;
    }
}

