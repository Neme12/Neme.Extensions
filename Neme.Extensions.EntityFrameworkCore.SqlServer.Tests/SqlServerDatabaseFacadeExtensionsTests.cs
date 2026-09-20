using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Neme.Extensions.EntityFrameworkCore.SqlServer;

namespace Neme.Extensions.EntityFrameworkCore.SqlServer.Tests;

public sealed class SqlServerDatabaseFacadeExtensionsTests
{
    [Fact]
    public void CreateSqlServerIdentityInsertScope_WhenCalledWithSchema_ExecutesOnAndOffCommands()
    {
        var connection = new RecordingDbConnection();
        using var context = new PlainSqlServerDbContext(connection);

        var scope = context.Database.CreateSqlServerIdentityInsertScope("Order Details", "sales");

        Assert.Equal(
            ["SET IDENTITY_INSERT [sales].[Order Details] ON;"],
            connection.ExecutedCommands);

        scope.Dispose();
        scope.Dispose();

        Assert.Equal(
            [
                "SET IDENTITY_INSERT [sales].[Order Details] ON;",
                "SET IDENTITY_INSERT [sales].[Order Details] OFF;",
            ],
            connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScope_WhenProviderIsNotSqlServer_ThrowsInvalidOperationException()
    {
        var connection = new RecordingDbConnection();
        var services = new ServiceCollection();
        services.AddEntityFrameworkSqlServer();
        Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.Replace(
            services,
            ServiceDescriptor.Singleton<IDatabaseProvider, FakeDatabaseProvider>());
        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInternalServiceProvider(serviceProvider)
            .UseSqlServer(connection)
            .Options;
        using var context = new DbContext(options);

        var exception = Assert.Throws<InvalidOperationException>(
            () => context.Database.CreateSqlServerIdentityInsertScope("Customers"));

        Assert.Equal("The database provider is not SQL Server.", exception.Message);
        Assert.Empty(connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScope_WhenTableNameIsNull_ThrowsArgumentNullExceptionAndDoesNotExecuteSql()
    {
        var connection = new RecordingDbConnection();
        using var context = new PlainSqlServerDbContext(connection);

        var exception = Assert.Throws<ArgumentNullException>(
            () => context.Database.CreateSqlServerIdentityInsertScope(null!));

        Assert.Equal("tableName", exception.ParamName);
        Assert.Empty(connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScope_WhenTableNameIsEmpty_ThrowsArgumentExceptionAndDoesNotExecuteSql()
    {
        var connection = new RecordingDbConnection();
        using var context = new PlainSqlServerDbContext(connection);

        var exception = Assert.Throws<ArgumentException>(
            () => context.Database.CreateSqlServerIdentityInsertScope(string.Empty));

        Assert.Equal("tableName", exception.ParamName);
        Assert.Empty(connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScopeOfTEntity_WhenProviderIsNotSqlServer_ThrowsInvalidOperationException()
    {
        var connection = new RecordingDbConnection();
        var services = new ServiceCollection();
        services.AddEntityFrameworkSqlServer();
        Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.Replace(
            services,
            ServiceDescriptor.Singleton<IDatabaseProvider, FakeDatabaseProvider>());
        var serviceProvider = services.BuildServiceProvider();
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInternalServiceProvider(serviceProvider)
            .UseSqlServer(connection)
            .Options;
        using var context = new DbContext(options);

        var exception = Assert.Throws<InvalidOperationException>(
            () => context.Database.CreateSqlServerIdentityInsertScope<TableMappedEntity>());

        Assert.Equal("The database provider is not SQL Server.", exception.Message);
        Assert.Empty(connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScopeOfTEntity_WhenEntityTypeIsNotInModel_ThrowsInvalidOperationExceptionAndDoesNotExecuteSql()
    {
        var connection = new RecordingDbConnection();
        using var context = new EmptyModelSqlServerDbContext(connection);

        var exception = Assert.Throws<InvalidOperationException>(
            () => context.Database.CreateSqlServerIdentityInsertScope<UnmappedEntity>());

        Assert.Equal(
            $"Entity type '{typeof(UnmappedEntity)}' is not part of the current DbContext model.",
            exception.Message);
        Assert.Empty(connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScopeOfTEntity_WhenEntityTypeIsMappedToView_ThrowsInvalidOperationExceptionAndDoesNotExecuteSql()
    {
        var connection = new RecordingDbConnection();
        using var context = new ViewMappedSqlServerDbContext(connection);

        var exception = Assert.Throws<InvalidOperationException>(
            () => context.Database.CreateSqlServerIdentityInsertScope<ViewMappedEntity>());

        Assert.Equal(
            $"Entity type '{typeof(ViewMappedEntity)}' is not mapped to a table.",
            exception.Message);
        Assert.Empty(connection.ExecutedCommands);
    }

    [Fact]
    public void CreateSqlServerIdentityInsertScopeOfTEntity_WhenEntityTypeIsMappedToTable_UsesMappedTableName()
    {
        var connection = new RecordingDbConnection();
        using var context = new TableMappedSqlServerDbContext(connection);

        var scope = context.Database.CreateSqlServerIdentityInsertScope<TableMappedEntity>();

        Assert.Equal(
            ["SET IDENTITY_INSERT [Order Details] ON;"],
            connection.ExecutedCommands);

        scope.Dispose();

        Assert.Equal(
            [
                "SET IDENTITY_INSERT [Order Details] ON;",
                "SET IDENTITY_INSERT [Order Details] OFF;",
            ],
            connection.ExecutedCommands);
    }



    private sealed class PlainSqlServerDbContext : DbContext
    {
        private readonly DbConnection _connection;

        public PlainSqlServerDbContext(DbConnection connection)
            => _connection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connection);
    }

    private static readonly IServiceProvider NonSqlServerServiceProvider = new ServiceCollection()
        .AddEntityFrameworkSqlServer()
        .AddSingleton<IDatabaseProvider, FakeDatabaseProvider>()
        .BuildServiceProvider();

    private sealed class NonSqlServerDbContext : DbContext
    {
        private readonly DbConnection _connection;

        public NonSqlServerDbContext(DbConnection connection)
            => _connection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInternalServiceProvider(NonSqlServerServiceProvider);
            optionsBuilder.UseSqlServer(_connection);
        }
    }

    private sealed class EmptyModelSqlServerDbContext : DbContext
    {
        private readonly DbConnection _connection;

        public EmptyModelSqlServerDbContext(DbConnection connection)
            => _connection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connection);
    }

    private sealed class TableMappedSqlServerDbContext : DbContext
    {
        private readonly DbConnection _connection;

        public TableMappedSqlServerDbContext(DbConnection connection)
            => _connection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connection);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<TableMappedEntity>().ToTable("Order Details");
    }

    private sealed class ViewMappedSqlServerDbContext : DbContext
    {
        private readonly DbConnection _connection;

        public ViewMappedSqlServerDbContext(DbConnection connection)
            => _connection = connection;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSqlServer(_connection);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            => modelBuilder.Entity<ViewMappedEntity>().HasNoKey().ToView("Order Details View", "sales");
    }

    private sealed class FakeDatabaseProvider : IDatabaseProvider
    {
        public string Name => "Microsoft.EntityFrameworkCore.Fake";

        public bool IsConfigured(IDbContextOptions options)
            => true;
    }

    private sealed class TableMappedEntity
    {
        public int Id { get; set; }
    }

    private sealed class ViewMappedEntity
    {
        public int Id { get; set; }
    }

    private sealed class UnmappedEntity
    {
        public int Id { get; set; }
    }

    private sealed class RecordingDbConnection : DbConnection
    {
        private readonly List<string> _executedCommands = [];
        private ConnectionState _state = ConnectionState.Closed;

        public IReadOnlyList<string> ExecutedCommands => _executedCommands;

        [AllowNull]
        public override string ConnectionString { get; set; } = string.Empty;

        public override string Database => "TestDatabase";

        public override string DataSource => "TestDataSource";

        public override string ServerVersion => "1.0";

        public override ConnectionState State => _state;

        public override void ChangeDatabase(string databaseName)
        {
        }

        public override void Close()
            => _state = ConnectionState.Closed;

        public override void Open()
            => _state = ConnectionState.Open;

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            => throw new NotSupportedException();

        protected override DbCommand CreateDbCommand()
            => new RecordingDbCommand(this, _executedCommands);
    }

    private sealed class RecordingDbCommand : DbCommand
    {
        private readonly DbConnection _connection;
        private readonly List<string> _executedCommands;
        private readonly RecordingDbParameterCollection _parameters = new();

        public RecordingDbCommand(DbConnection connection, List<string> executedCommands)
        {
            _connection = connection;
            _executedCommands = executedCommands;
        }

        [AllowNull]
        public override string CommandText { get; set; } = string.Empty;

        public override int CommandTimeout { get; set; }

        public override CommandType CommandType { get; set; }

        public override bool DesignTimeVisible { get; set; }

        public override UpdateRowSource UpdatedRowSource { get; set; }

        [AllowNull]
        protected override DbConnection DbConnection
        {
            get => _connection;
            set => throw new NotSupportedException();
        }

        protected override DbParameterCollection DbParameterCollection => _parameters;

        protected override DbTransaction? DbTransaction { get; set; }

        public override void Cancel()
        {
        }

        public override int ExecuteNonQuery()
        {
            _executedCommands.Add(CommandText);
            return 0;
        }

        public override object? ExecuteScalar()
            => null;

        public override void Prepare()
        {
        }

        protected override DbParameter CreateDbParameter()
            => new RecordingDbParameter();

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            => throw new NotSupportedException();
    }

    private sealed class RecordingDbParameterCollection : DbParameterCollection
    {
        private readonly List<DbParameter> _parameters = [];

        public override int Count => _parameters.Count;

        public override object SyncRoot => ((ICollection)_parameters).SyncRoot;

        public override int Add(object value)
        {
            _parameters.Add((DbParameter)value);
            return _parameters.Count - 1;
        }

        public override void AddRange(Array values)
        {
            foreach (var value in values)
                Add(value!);
        }

        public override void Clear()
            => _parameters.Clear();

        public override bool Contains(object value)
            => _parameters.Contains((DbParameter)value);

        public override bool Contains(string value)
            => _parameters.Exists(parameter => parameter.ParameterName == value);

        public override void CopyTo(Array array, int index)
            => ((ICollection)_parameters).CopyTo(array, index);

        public override IEnumerator GetEnumerator()
            => _parameters.GetEnumerator();

        public override int IndexOf(object value)
            => _parameters.IndexOf((DbParameter)value);

        public override int IndexOf(string parameterName)
            => _parameters.FindIndex(parameter => parameter.ParameterName == parameterName);

        public override void Insert(int index, object value)
            => _parameters.Insert(index, (DbParameter)value);

        public override void Remove(object value)
            => _parameters.Remove((DbParameter)value);

        public override void RemoveAt(int index)
            => _parameters.RemoveAt(index);

        public override void RemoveAt(string parameterName)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
                _parameters.RemoveAt(index);
        }

        protected override DbParameter GetParameter(int index)
            => _parameters[index];

        protected override DbParameter GetParameter(string parameterName)
            => _parameters[IndexOf(parameterName)];

        protected override void SetParameter(int index, DbParameter value)
            => _parameters[index] = value;

        protected override void SetParameter(string parameterName, DbParameter value)
        {
            var index = IndexOf(parameterName);
            if (index >= 0)
                _parameters[index] = value;
            else
                _parameters.Add(value);
        }
    }

    private sealed class RecordingDbParameter : DbParameter
    {
        public override DbType DbType { get; set; }

        public override ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        [AllowNull]
        public override string ParameterName { get; set; } = string.Empty;

        [AllowNull]
        public override string SourceColumn { get; set; } = string.Empty;

        public override object? Value { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override int Size { get; set; }

        public override void ResetDbType()
        {
        }
    }
}
