using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly string _connectionString =
            "Server=LAPTOP-OU71PFMJ\\SQLEXPRESS;Database=Fedelicious;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

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
            string keyName = GetKeyColumnName();
            string query = $"SELECT * FROM {tableName} WHERE {keyName} = @Id";
            return connection.QueryFirstOrDefault<T>(query, new { Id = id });
        }

        public bool Add(T entity)
        {
            using var connection = CreateConnection();
            var tableName = GetTableName();

            var props = typeof(T).GetProperties().Where(p => {
                var isKey = p.GetCustomAttribute<KeyAttribute>() != null;
                var isIdentity = p.GetCustomAttribute<DatabaseGeneratedAttribute>()?.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity;

                // Return true only if it's a simple property and NOT an identity key
                return (!isKey || !isIdentity) &&
                       (p.PropertyType.IsPrimitive ||
                        p.PropertyType == typeof(string) ||
                        p.PropertyType == typeof(decimal) ||
                        p.PropertyType == typeof(DateTime) ||
                        p.PropertyType == typeof(int?) ||
                        p.PropertyType == typeof(TimeSpan?));
            }).ToList();

            var columns = string.Join(", ", props.Select(p => $"[{p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name}]"));
            var values = string.Join(", ", props.Select(p => "@" + p.Name));

            string query = $@"INSERT INTO {tableName} ({columns}) VALUES ({values}); 
                             SELECT CAST(SCOPE_IDENTITY() as int);";

            try
            {
                var newId = connection.QuerySingle<int>(query, entity);
                if (newId > 0)
                {
                    var keyProp = typeof(T).GetProperties()
                        .FirstOrDefault(p => p.GetCustomAttribute<KeyAttribute>() != null);
                    if (keyProp != null) keyProp.SetValue(entity, newId);
                    return true;
                }
                return false;
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
            string keyName = GetKeyColumnName();
            string setClause = GetUpdateSetClause();

            string query = $"UPDATE {tableName} SET {setClause} WHERE {keyName} = @{keyName}";

            try
            {
                return connection.Execute(query, entity) > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Repository Update Error: " + ex.Message);
                throw; // Rethrow para makita ang Foreign Key error sa Controller catch block
            }
        }

        public bool Delete(int id)
        {
            using var connection = CreateConnection();
            string tableName = GetTableName();
            string keyName = GetKeyColumnName();

            string query = $"DELETE FROM {tableName} WHERE {keyName} = @id";
            return connection.Execute(query, new { id = id }) > 0;
        }

        // ======================
        // HELPERS
        // ======================

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
            return prop != null ? prop.Name : "id";
        }

        private string GetUpdateSetClause()
        {
            var properties = typeof(T).GetProperties().Where(p =>
                p.GetCustomAttribute<KeyAttribute>() == null &&
                (p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(decimal) || p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(int?)));

            return string.Join(", ", properties.Select(p => {
                var colName = p.GetCustomAttribute<ColumnAttribute>()?.Name ?? p.Name;
                return $"[{colName}] = @{p.Name}";
            }));
        }

        public void Delete(string token)
        {
            throw new NotImplementedException();
        }
    }
}