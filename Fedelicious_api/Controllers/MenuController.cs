using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Fedelicious_api.Model;
using Fedelicious_api.Service;
using Fedelicous_api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Fedelicious_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // ==========================================
        // 1. GET ALL MENUS 
        // ==========================================
        [HttpGet]
        public IActionResult GetAllMenus()
        {
            var menus = _menuService.GetAllMenuItems();
            return Ok(menus);
        }

        // ==========================================
        // 2. GET ALL CATEGORIES
        // ==========================================
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            var categories = _menuService.GetAllCategories();
            if (categories == null) return Ok(new List<categories>());
            return Ok(categories);
        }

        // ==========================================
        // 3. GET MENU BY ID
        // ==========================================
        [HttpGet("{id}")]
        public IActionResult GetMenuById(int id)
        {
            var item = _menuService.GetMenuItemById(id);
            if (item == null) return NotFound(new { message = "Item not found." });
            return Ok(item);
        }

        // ==========================================
        // 4. GET MENU BY CATEGORY ID (BAGONG DAGDAG PARA GUMANA SA UI)
        // ==========================================
        [HttpGet("category/{categoryId}")]
        public IActionResult GetMenuByCategory(int categoryId)
        {
            // Kukunin lahat at i-fi-filter sa C# (o pwede rin sa Service/SQL kung may ginawa ka)
            var allMenus = _menuService.GetAllMenuItems();
            var filteredMenus = allMenus.Where(m => m.category_id == categoryId).ToList();

            return Ok(filteredMenus);
        }

        // ==========================================
        // 5. ADD MENU ITEM (ADMIN)
        // ==========================================
        [HttpPost]
        public IActionResult AddMenu([FromBody] menu_items newItem)
        {
            if (newItem == null) return BadRequest(new { message = "Invalid data." });

            bool isAdded = _menuService.AddMenuItem(newItem);
            return isAdded ? Ok(new { message = "Menu item added!" }) : BadRequest("Failed to add.");
        }

        // ==========================================
        // 6. UPDATE MENU ITEM (ADMIN)
        // ==========================================
        [HttpPut("{id}")]
        public IActionResult UpdateMenu(int id, [FromBody] menu_items updatedItem)
        {
            if (updatedItem == null) return BadRequest(new { message = "Invalid data." });

            updatedItem.menu_id = id;

            try
            {
                bool isUpdated = _menuService.UpdateMenuItem(updatedItem);
                return isUpdated ? Ok(new { message = "Update success!" }) : BadRequest("Update failed.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Database Error: " + ex.Message });
            }
        }

        // ==========================================
        // 7. DELETE MENU ITEM (ADMIN)
        // ==========================================
        [HttpDelete("{id}")]
        public IActionResult DeleteMenu(int id)
        {
            bool isDeleted = _menuService.DeleteMenuItem(id);
            return isDeleted ? Ok(new { message = "Deleted successfully." }) : BadRequest("Delete failed.");
        }

        // ==========================================
        // 8. ADD CATEGORY (ADMIN)
        // ==========================================
        [HttpPost("categories")]
        public IActionResult AddCategory([FromBody] categories newCat)
        {
            if (newCat == null || string.IsNullOrEmpty(newCat.category_name))
                return BadRequest(new { message = "Name is required." });

            bool isAdded = _menuService.AddCategory(newCat);
            return isAdded ? Ok(new { message = "Category added!" }) : BadRequest("Failed to add category.");
        }

        // ==========================================
        // 9. DELETE CATEGORY (ADMIN) 
        // ==========================================
        [HttpDelete("categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            bool isDeleted = _menuService.DeleteCategory(id);

            if (isDeleted)
            {
                return Ok(new { message = "Category and related items deleted successfully." });
            }

            return BadRequest(new { message = "Failed to delete category. Check if other tables are linked." });
        }
    }
}