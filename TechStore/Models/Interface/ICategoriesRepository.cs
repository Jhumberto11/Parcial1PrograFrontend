using TechStore.Models;

namespace TechStore.Models.Interface
{
    public interface ICategoriesRepository
    {
        IEnumerable<Categoria> GetAllCategories();
        Categoria GetCategoryById(int id);
        void AddCategory(Categoria categoria);
        void UpdateCategory(Categoria categoria);
        void DeleteCategory(int id);
    }
}