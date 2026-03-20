using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Fedelicious_api.Model;
using Fedelicious_api.Repository;
using Fedelicous_api.Model;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
            IConfiguration config) // Ininject ang config para sa Connection String
        {
            _menuRepo = menuRepo;
            _categoryRepo = categoryRepo;
            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        // ==================================
        // MENU ITEMS
        // ==================================

        public IEnumerable<menu_items> GetAllMenuItems()
        {
            return _menuRepo.GetAll();
        }

        public menu_items GetMenuItemById(int id)
        {
            return _menuRepo.GetById(id);
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

        // ==================================
        // CATEGORY METHODS
        // ==================================

        public IEnumerable<categories> GetAllCategories()
        {
            return _categoryRepo.GetAll();
        }

        public IEnumerable<menu_items> GetMenuItemsByCategory(int categoryId)
        {
            return _menuRepo
                .GetAll()
                .Where(m => m.category_id == categoryId);
        }

        public bool AddCategory(categories newCat)
        {
            if (newCat == null || string.IsNullOrEmpty(newCat.category_name))
                return false;

            return _categoryRepo.Add(newCat);
        }

        // FIX: Manual cleanup para iwas Foreign Key Error (400)
        public bool DeleteCategory(int id)
        {
            using (IDbConnection db = new SqlConnection("Server=LAPTOP-OU71PFMJ\\SQLEXPRESS; Database=Fedelicious; Trusted_Connection=True; TrustServerCertificate=True;"))
            {
                db.Open();
                using (var transaction = db.BeginTransaction())
                {
                    try
                    {
                        // 1. Burahin ang order_items na linked sa menu items ng category na ito
                        string deleteOrderItems = @"
                            DELETE FROM order_items 
                            WHERE menu_id IN (SELECT menu_id FROM menu_items WHERE category_id = @catId)";
                        db.Execute(deleteOrderItems, new { catId = id }, transaction);

                        // 2. Burahin ang lahat ng menu_items sa ilalim ng category na ito
                        string deleteMenuItems = "DELETE FROM menu_items WHERE category_id = @catId";
                        db.Execute(deleteMenuItems, new { catId = id }, transaction);

                        // 3. Burahin ang mismong category
                        string deleteCategory = "DELETE FROM categories WHERE category_id = @catId";
                        int affectedRows = db.Execute(deleteCategory, new { catId = id }, transaction);

                        transaction.Commit();
                        return affectedRows > 0;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine("DeleteCategory Error: " + ex.Message);
                        return false;
                    }
                }
            }
        }
    }
}