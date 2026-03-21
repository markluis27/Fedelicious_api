using System.Collections.Generic;
using Fedelicious_api.Model;
using Fedelicous_api.Model;

namespace Fedelicious_api.Service
{
    public interface IMenuService
    {
        IEnumerable<menu_items> GetAllMenuItems();
        bool ToggleMenuAvailability(int id);
        menu_items GetMenuItemById(int id);
        IEnumerable<menu_items> GetMenuItemsByCategory(int categoryId);

        bool AddMenuItem(menu_items item);
        bool UpdateMenuItem(menu_items item);
        bool DeleteMenuItem(int id);

        IEnumerable<menu_items> GetAvailableMenuItems();
        IEnumerable<categories> GetAllCategories();
        bool AddCategory(categories newCat);
        bool DeleteCategory(int id);
    }
}