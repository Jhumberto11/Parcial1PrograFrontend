using TechStore.Data;
using TechStore.Models;
using TechStore.Models.Interface;

namespace TechStore.Services
{
    public class CategoryService : ICategoriesRepository
    {
        private readonly AppDbContext _context;

        // Inyectamos AppDbContext aquí 
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Categoria> GetAllCategories()
        {
            return _context.Categories.ToList();
        }

        public Categoria GetCategoryById(int id)
        {
            return _context.Categories.Find(id);
        }

        public void AddCategory(Categoria categoria)
        {
            _context.Categories.Add(categoria);
            _context.SaveChanges();
        }

        public void UpdateCategory(Categoria categoria)
        {
            _context.Categories.Update(categoria);
            _context.SaveChanges();
        }

        public void DeleteCategory(int id)
        {
            var categoria = _context.Categories.Find(id);
            if (categoria != null)
            {
                _context.Categories.Remove(categoria);
                _context.SaveChanges();
            }
        }
    }
}