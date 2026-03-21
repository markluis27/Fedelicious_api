using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Fedelicous_api.Model;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Service
{
    public class MenuService : IMenuService
    {
        private readonly IGenericRepository<menu_items> _menuRepo;
        private readonly IGenericRepository<categories> _categoryRepo;
        private readonly string _connectionString;

        public MenuService(
            IGenericRepository<menu_items> menuRepo,
            IGenericRepository<categories> categoryRepo,
            IConfiguration configuration)
        {
            _menuRepo = menuRepo;
            _categoryRepo = categoryRepo;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IEnumerable<menu_items> GetAllMenuItems()
        {
            return _menuRepo.GetAll();
        }

        public menu_items GetMenuItemById(int id)
        {
            return _menuRepo.GetById(id);
        }

        public IEnumerable<menu_items> GetMenuItemsByCategory(int categoryId)
        {
            return _menuRepo.GetAll().Where(m => m.category_id == categoryId);
        }

        public bool AddMenuItem(menu_items item)
        {
            if (item == null) return false;
            return _menuRepo.Add(item);
        }

        public bool UpdateMenuItem(menu_items item)
        {
            if (item == null) return false;
            return _menuRepo.Update(item);
        }

        public bool DeleteMenuItem(int id)
        {
            return _menuRepo.Delete(id);
        }

        public IEnumerable<categories> GetAllCategories()
        {
            return _categoryRepo.GetAll();
        }

        public bool AddCategory(categories newCat)
        {
            if (newCat == null || string.IsNullOrWhiteSpace(newCat.category_name))
                return false;

            return _categoryRepo.Add(newCat);
        }

        public bool DeleteCategory(int id)
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            db.Open();

            using var transaction = db.BeginTransaction();

            try
            {
                string deleteOrderItemsSql = @"
                    DELETE FROM order_items
                    WHERE menu_id IN (
                        SELECT menu_id FROM menu_items WHERE category_id = @catId
                    )";

                db.Execute(deleteOrderItemsSql, new { catId = id }, transaction);

                string deleteMenuItemsSql = "DELETE FROM menu_items WHERE category_id = @catId";
                db.Execute(deleteMenuItemsSql, new { catId = id }, transaction);

                string deleteCategorySql = "DELETE FROM categories WHERE category_id = @catId";
                int affected = db.Execute(deleteCategorySql, new { catId = id }, transaction);

                transaction.Commit();
                return affected > 0;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}