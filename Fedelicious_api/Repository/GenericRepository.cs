using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _connectionString;

        public GenericRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public IEnumerable<T> GetAll()
        {
            using var connection = CreateConnection();
            string tableName = GetTableName();
            string query = $"SELECT * FROM {tableName}";
            return connection.Query<T>(query);
        }

        public T GetById(int id)
        {
            using var connection = CreateConnection();
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string query = $"SELECT * FROM {tableName} WHERE [{keyColumn}] = @Id";
            return connection.QueryFirstOrDefault<T>(query, new { Id = id });
        }

        public bool Add(T entity)
        {
            using var connection = CreateConnection();
            string tableName = GetTableName();

            var props = typeof(T).GetProperties().Where(p =>
            {
                var keyAttr = p.GetCustomAttribute<KeyAttribute>();
                var dbGenerated = p.GetCustomAttribute<DatabaseGeneratedAttribute>();

                bool isIdentityKey = keyAttr != null &&
                                     dbGenerated != null &&
                                     dbGenerated.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;

                return !isIdentityKey && IsAllowedType(p.PropertyType);
            }).ToList();

            var columns = string.Join(", ", props.Select(p => $"[{GetColumnName(p)}]"));
            var values = string.Join(", ", props.Select(p => "@" + p.Name));

            string query = $@"
                INSERT INTO {tableName} ({columns})
                VALUES ({values});
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            try
            {
                int newId = connection.QuerySingle<int>(query, entity);

                var keyProp = typeof(T).GetProperties()
                    .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);

                if (keyProp != null && keyProp.CanWrite && keyProp.PropertyType == typeof(int))
                {
                    keyProp.SetValue(entity, newId);
                }

                return newId > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Repository Add Error: " + ex.Message);
                return false;
            }
        }

        public bool Update(T entity)
        {
            using var connection = CreateConnection();
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();
            string keyProperty = GetKeyPropertyName();
            string setClause = GetUpdateSetClause();

            string query = $"UPDATE {tableName} SET {setClause} WHERE [{keyColumn}] = @{keyProperty}";

            try
            {
                return connection.Execute(query, entity) > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Repository Update Error: " + ex.Message);
                throw;
            }
        }

        public bool Delete(int id)
        {
            using var connection = CreateConnection();
            string tableName = GetTableName();
            string keyColumn = GetKeyColumnName();

            string query = $"DELETE FROM {tableName} WHERE [{keyColumn}] = @id";
            return connection.Execute(query, new { id }) > 0;
        }

        public void Delete(string token)
        {
            throw new NotImplementedException();
        }

        private string GetTableName()
        {
            var type = typeof(T);
            var tableAttr = type.GetCustomAttribute<TableAttribute>();
            return tableAttr != null ? $"[{tableAttr.Name}]" : $"[{type.Name}]";
        }

        private string GetKeyColumnName()
        {
            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);

            if (prop == null) return "id";

            return GetColumnName(prop);
        }

        private string GetKeyPropertyName()
        {
            var prop = typeof(T).GetProperties()
                .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);

            return prop != null ? prop.Name : "id";
        }

        private string GetColumnName(PropertyInfo property)
        {
            return property.GetCustomAttribute<ColumnAttribute>()?.Name ?? property.Name;
        }

        private string GetUpdateSetClause()
        {
            var properties = typeof(T).GetProperties().Where(p =>
                p.GetCustomAttribute<KeyAttribute>() == null &&
                IsAllowedType(p.PropertyType));

            return string.Join(", ", properties.Select(p => $"[{GetColumnName(p)}] = @{p.Name}"));
        }

        private bool IsAllowedType(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            return type == typeof(string) ||
                   type == typeof(int) ||
                   type == typeof(decimal) ||
                   type == typeof(bool) ||
                   type == typeof(DateTime) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(byte[]);
        }
    }
}