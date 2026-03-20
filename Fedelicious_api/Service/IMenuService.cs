using System.Collections.Generic;
using Fedelicious_api.Model;
using Fedelicous_api.Model;

namespace Fedelicious_api.Service
{
    public interface IMenuService
    {
        // ===============================
        // MENU ITEMS
        // ===============================
        IEnumerable<menu_items> GetAllMenuItems();
        menu_items GetMenuItemById(int id);
        bool AddMenuItem(menu_items item);
        bool UpdateMenuItem(menu_items item);
        bool DeleteMenuItem(int id);

        // ===============================
        // CATEGORIES
        // ===============================
        IEnumerable<categories> GetAllCategories();
        IEnumerable<menu_items> GetMenuItemsByCategory(int categoryId);
        bool AddCategory(categories newCat);
        bool DeleteCategory(int id); // Siguraduhin na nandito ito
        }
    }