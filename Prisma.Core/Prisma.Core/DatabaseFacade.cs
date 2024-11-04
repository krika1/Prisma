using System.Data;
using System.Data.SqlClient;

namespace Prisma.Core
{
    public class DatabaseFacade : IDisposable
    {
        private readonly string _connectionString;
        private SqlConnection? _sqlConnection;
        private SqlCommand? _sqlCommand;
        private SqlDataReader? _sqlDataReader;

        public DatabaseFacade(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<TEntity>> ExecuteSqlAsync<TEntity>(string sql)
            where TEntity : class, new()
        {
            _sqlConnection = new SqlConnection(_connectionString);

            _sqlConnection.Open();

            _sqlCommand = new SqlCommand(sql, _sqlConnection);

            _sqlDataReader = await _sqlCommand.ExecuteReaderAsync();

            var entities = new List<TEntity>();

            while (await _sqlDataReader.ReadAsync())
            {
                var mappedEntity = MapToEntity<TEntity>(_sqlDataReader);
                entities.Add(mappedEntity);
            }

            return entities;
        }

        public async Task<TEntity> ExecuteSingleSqlAsync<TEntity>(string sql)
           where TEntity : class, new()
        {
            _sqlConnection = new SqlConnection(_connectionString);

            _sqlConnection.Open();

            _sqlCommand = new SqlCommand(sql, _sqlConnection);

            _sqlDataReader = await _sqlCommand.ExecuteReaderAsync();

            var entity = new TEntity();

            while (await _sqlDataReader.ReadAsync())
            {
                entity = MapToEntity<TEntity>(_sqlDataReader);

            }
            return entity;
        }

        public async Task<object?> ExecuteSingleSqlAsync(Type entityType, string sql)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);

            try
            {
                await connection.OpenAsync();
                using var reader = await command.ExecuteReaderAsync();

                object? entity = null;

                if (await reader.ReadAsync())
                {
                    // Dynamically create an instance and map it
                    entity = MapToEntity(entityType, reader);
                }

                return entity;
            }
            catch (Exception ex)
            {
                // Log or handle the exception as needed
                throw new DataException("Error executing SQL query", ex);
            }
        }

        public async Task ExecuteNonQueryAsync(string sql, object entity)
        {
            _sqlConnection = new SqlConnection(_connectionString);

            _sqlConnection.Open();

            _sqlCommand = new SqlCommand(sql, _sqlConnection);

            AddParameters(_sqlCommand, entity);

            await _sqlCommand.ExecuteNonQueryAsync();
        }

        private void AddParameters(SqlCommand command, object obj)
        {
            var type = obj.GetType();
            var properties = type.GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(obj) ?? DBNull.Value;
                command.Parameters.AddWithValue("@" + property.Name, value);
            }
        }

        public void Dispose()
        {
            _sqlConnection!.Dispose();
            _sqlCommand!.Dispose();
        }

        private TEntity MapToEntity<TEntity>(SqlDataReader reader) where TEntity : new()
        {
            var entity = new TEntity();
            var properties = typeof(TEntity).GetProperties();

            foreach (var property in properties)
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    property.SetValue(entity, null);
                }
                else if (reader[property.Name] != DBNull.Value)
                {
                    property.SetValue(entity, reader[property.Name]);
                }
            }

            return entity;
        }

        private object MapToEntity(Type entityType, SqlDataReader reader)
        {
            var entity = Activator.CreateInstance(entityType) ?? throw new InvalidOperationException("Cannot create instance of type " + entityType);

            foreach (var property in entityType.GetProperties())
            {
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    property.SetValue(entity, null);
                }
                else if (reader[property.Name] != DBNull.Value)
                {
                    property.SetValue(entity, reader[property.Name]);
                }
            }

            return entity;
        }
    }
}
