using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Neme.Extensions.EntityFrameworkCore.SqlServer.Tests;

public sealed class SqlServerMigrationBuilderExtensionsTests
{
    public sealed class AddSqlServerColumnWithInitialValue
    {
        [Fact]
        public void WhenProviderIsNotSqlServer_ThrowsInvalidOperationException()
        {
            var migrationBuilder = new MigrationBuilder("Microsoft.EntityFrameworkCore.Sqlite");

            var exception = Assert.Throws<InvalidOperationException>(
                () => migrationBuilder.AddSqlServerColumnWithInitialValue<int>(
                    name: "Age",
                    table: "Customers"));

            Assert.Equal("The database provider is not SQL Server.", exception.Message);
            Assert.Empty(migrationBuilder.Operations);
        }

        [Fact]
        public void WhenColumnIsNullable_NoInitialValue_AddsColumn()
        {
            var migrationBuilder = CreateSqlServerMigrationBuilder();

            var builder = migrationBuilder.AddSqlServerColumnWithInitialValue<string>(
                name: "Unit Price",
                table: "Order Details",
                type: "nvarchar(50)",
                unicode: true,
                maxLength: 50,
                nullable: true);

            Assert.Single(migrationBuilder.Operations);

            var addColumn = Assert.IsType<AddColumnOperation>(migrationBuilder.Operations[0]);
            Assert.Equal("Unit Price", addColumn.Name);
            Assert.Equal("Order Details", addColumn.Table);
            Assert.Equal(typeof(string), addColumn.ClrType);
            Assert.Equal("nvarchar(50)", addColumn.ColumnType);
            Assert.True(addColumn.IsUnicode);
            Assert.Equal(50, addColumn.MaxLength);
            Assert.True(addColumn.IsNullable);

            Assert.NotNull(builder);
        }

        [Fact]
        public void WhenColumnIsNullable_WithInitialValue_AddsColumnAndSqlWithoutAlter()
        {
            var migrationBuilder = CreateSqlServerMigrationBuilder();

            var builder = migrationBuilder.AddSqlServerColumnWithInitialValue<string>(
                name: "Unit Price",
                table: "Order Details",
                type: "nvarchar(50)",
                unicode: true,
                maxLength: 50,
                nullable: true,
                initialValueSql: "\"test\"");

            Assert.Equal(2, migrationBuilder.Operations.Count);

            var addColumn = Assert.IsType<AddColumnOperation>(migrationBuilder.Operations[0]);
            Assert.Equal("Unit Price", addColumn.Name);
            Assert.Equal("Order Details", addColumn.Table);
            Assert.Equal(typeof(string), addColumn.ClrType);
            Assert.Equal("nvarchar(50)", addColumn.ColumnType);
            Assert.True(addColumn.IsUnicode);
            Assert.Equal(50, addColumn.MaxLength);
            Assert.True(addColumn.IsNullable);

            var sqlOperation = Assert.IsType<SqlOperation>(migrationBuilder.Operations[1]);
            Assert.Equal(
                "UPDATE [Order Details]\nSET [Unit Price] = \"test\";",
                NormalizeNewLines(sqlOperation.Sql.Trim()));

            Assert.NotNull(builder);
        }

        [Fact]
        public void WhenColumnIsNotNullable_NoInitialValue_AddsColumnAndAlter()
        {
            var migrationBuilder = CreateSqlServerMigrationBuilder();

            var builder = migrationBuilder.AddSqlServerColumnWithInitialValue<int>(
                name: "Age",
                table: "Customers",
                type: "int");

            Assert.Single(migrationBuilder.Operations);

            var addColumn = Assert.IsType<AddColumnOperation>(migrationBuilder.Operations[0]);
            Assert.Equal("Age", addColumn.Name);
            Assert.Equal("Customers", addColumn.Table);
            Assert.Equal(typeof(int), addColumn.ClrType);
            Assert.Equal("int", addColumn.ColumnType);
            Assert.False(addColumn.IsNullable);

            Assert.NotNull(builder);
        }

        [Fact]
        public void WhenColumnIsNotNullable_WithInitialValue_AddsColumnSqlAndAlter()
        {
            var migrationBuilder = CreateSqlServerMigrationBuilder();

            var builder = migrationBuilder.AddSqlServerColumnWithInitialValue<int>(
                name: "Age",
                table: "Customers",
                type: "int",
                initialValueSql: "42");

            Assert.Equal(3, migrationBuilder.Operations.Count);

            var addColumn = Assert.IsType<AddColumnOperation>(migrationBuilder.Operations[0]);
            Assert.Equal("Age", addColumn.Name);
            Assert.Equal("Customers", addColumn.Table);
            Assert.Equal(typeof(int), addColumn.ClrType);
            Assert.Equal("int", addColumn.ColumnType);
            Assert.True(addColumn.IsNullable);

            var sqlOperation = Assert.IsType<SqlOperation>(migrationBuilder.Operations[1]);
            Assert.Equal(
                "UPDATE [Customers]\nSET [Age] = 42;",
                NormalizeNewLines(sqlOperation.Sql.Trim()));

            var alterColumn = Assert.IsType<AlterColumnOperation>(migrationBuilder.Operations[2]);
            Assert.Equal("Age", alterColumn.Name);
            Assert.Equal("Customers", alterColumn.Table);
            Assert.Equal(typeof(int), alterColumn.ClrType);
            Assert.Equal("int", alterColumn.ColumnType);
            Assert.False(alterColumn.IsNullable);
            Assert.Equal(typeof(int), alterColumn.OldColumn.ClrType);
            Assert.Equal("int", alterColumn.OldColumn.ColumnType);
            Assert.True(alterColumn.OldColumn.IsNullable);

            Assert.NotNull(builder);
        }

        private static MigrationBuilder CreateSqlServerMigrationBuilder()
            => new("Microsoft.EntityFrameworkCore.SqlServer");

        private static string NormalizeNewLines(string value)
            => value.Replace("\r\n", "\n", StringComparison.Ordinal);
    }
}
