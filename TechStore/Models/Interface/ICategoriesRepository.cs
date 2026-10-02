namespace TechStore.Models.Interface
{
    public interface ICategoriesRepository
    {

        Task<IEnumerable<Categoria>> GetAllCategoriesAsync();
        Task<Categoria> GetCategoryByIdAsync(int id);
        Task<Categoria> AddCategoryAsync(Categoria category);
        Task<Categoria> UpdateCategoryAsync(Categoria category);
        Task<bool> DeleteCategoryAsync(int id);

    }
}
