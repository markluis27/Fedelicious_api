using System;
using System.Collections.Generic;
using Fedelicious_api.Model;
using Fedelicious_api.Service;
using Fedelicous_api.Model;
using Microsoft.AspNetCore.Mvc;

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

        // =========================================
        // 1. GET ALL MENU (ADMIN PURPOSE)
        // =========================================
        [HttpGet("admin")]
        public IActionResult GetAllMenusAdmin()
        {
            try
            {
                var menus = _menuService.GetAllMenuItems();
                return Ok(menus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting menu items.", error = ex.Message });
            }
        }

        // =========================================
        // 2. GET AVAILABLE MENU (CUSTOMER VIEW)
        // =========================================
        [HttpGet("available")]
        public IActionResult GetAvailableMenus()
        {
            try
            {
                var menus = _menuService.GetAvailableMenuItems();
                return Ok(menus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting available menu items.", error = ex.Message });
            }
        }

        // =========================================
        // 3. GET MENU BY ID
        // =========================================
        [HttpGet("{id}")]
        public IActionResult GetMenuById(int id)
        {
            try
            {
                var item = _menuService.GetMenuItemById(id);

                if (item == null)
                    return NotFound(new { message = "Menu item not found." });

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting menu item.", error = ex.Message });
            }
        }

        // =========================================
        // 4. GET BY CATEGORY
        // =========================================
        [HttpGet("category/{categoryId}")]
        public IActionResult GetMenuByCategory(int categoryId)
        {
            try
            {
                var items = _menuService.GetMenuItemsByCategory(categoryId);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting category menu.", error = ex.Message });
            }
        }

        // =========================================
        // 5. ADD MENU ITEM
        // =========================================
        [HttpPost]
        public IActionResult AddMenu([FromBody] menu_items newItem)
        {
            try
            {
                if (newItem == null)
                    return BadRequest(new { message = "Invalid menu data." });

                bool added = _menuService.AddMenuItem(newItem);

                if (!added)
                    return BadRequest(new { message = "Failed to add menu item." });

                return Ok(new { message = "Menu item added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding menu item.", error = ex.Message });
            }
        }

        // =========================================
        // 6. UPDATE MENU ITEM
        // =========================================
        [HttpPut("{id}")]
        public IActionResult UpdateMenu(int id, [FromBody] menu_items updatedItem)
        {
            try
            {
                if (updatedItem == null)
                    return BadRequest(new { message = "Invalid menu data." });

                updatedItem.menu_id = id;

                bool updated = _menuService.UpdateMenuItem(updatedItem);

                if (!updated)
                    return BadRequest(new { message = "Failed to update menu item." });

                return Ok(new { message = "Menu item updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating menu item.", error = ex.Message });
            }
        }

        // =========================================
        // 7. DELETE MENU ITEM
        // =========================================
        [HttpDelete("{id}")]
        public IActionResult DeleteMenu(int id)
        {
            try
            {
                bool deleted = _menuService.DeleteMenuItem(id);

                if (!deleted)
                    return BadRequest(new { message = "Failed to delete menu item." });

                return Ok(new { message = "Menu item deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting menu item.", error = ex.Message });
            }
        }

        // =========================================
        // 8. CATEGORIES
        // =========================================
        [HttpGet("categories")]
        public IActionResult GetCategories()
        {
            try
            {
                var categories = _menuService.GetAllCategories();
                return Ok(categories ?? new List<categories>());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error getting categories.", error = ex.Message });
            }
        }

        [HttpPost("categories")]
        public IActionResult AddCategory([FromBody] categories newCategory)
        {
            try
            {
                if (newCategory == null || string.IsNullOrWhiteSpace(newCategory.category_name))
                    return BadRequest(new { message = "Invalid category data." });

                bool added = _menuService.AddCategory(newCategory);

                if (!added)
                    return BadRequest(new { message = "Failed to add category." });

                return Ok(new { message = "Category added successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding category.", error = ex.Message });
            }
        }

        [HttpDelete("categories/{id}")]
        public IActionResult DeleteCategory(int id)
        {
            try
            {
                bool deleted = _menuService.DeleteCategory(id);

                if (!deleted)
                    return BadRequest(new { message = "Failed to delete category." });

                return Ok(new { message = "Category deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting category.", error = ex.Message });
            }
        }

        // =========================================
        // 9. TOGGLE AVAILABILITY (IMPORTANT FEATURE)
        // =========================================
        [HttpPut("toggle-availability/{id}")]
        public IActionResult ToggleAvailability(int id)
        {
            try
            {
                bool result = _menuService.ToggleMenuAvailability(id);

                if (!result)
                    return NotFound(new { message = "Menu item not found." });

                return Ok(new { message = "Menu availability updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating availability.", error = ex.Message });
            }
        }
    }
}