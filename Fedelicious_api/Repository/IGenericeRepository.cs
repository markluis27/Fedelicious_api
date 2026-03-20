using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Reflection;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        // READ
        IEnumerable<T> GetAll();
        T GetById(int id);

        // CREATE
        bool Add(T entity);

        // UPDATE
        bool Update(T entity);

        // DELETE
        bool Delete(int id);
        void Delete(string token);
    }
}
