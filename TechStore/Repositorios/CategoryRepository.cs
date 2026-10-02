using TechStore.Data;
using TechStore.Models;
using TechStore.Models.Interface;
using Microsoft.EntityFrameworkCore;

namespace TechStore.Repositorios
{
    public class CategoryRepository : ICategoriesRepository
    {
        private readonly AppDbContext _context;
        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria> AddCategoryAsync(Categoria category)
        {
            if(category == null)
            {
                throw new Exception(nameof(category));
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            if(id <= 0)
            {
                throw new Exception("Id debe ser un valor positivo");
            }

            var result = await _context.Categories.FindAsync(id);
            if (result == null)
            {
                throw new Exception($"Categoria con id {id} no encontrada");
            }

            _context.Categories.Remove(result);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Categoria>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<Categoria> GetCategoryByIdAsync(int id)
        {
            var result = await _context.Categories.FindAsync(id);
            if (result == null)
            {
                throw new Exception($"Categoria con id {id} no encontrada");
            }
            return result;
        }

        public async Task<Categoria> UpdateCategoryAsync(Categoria category)
        {
            if (category == null)
            {
                throw new Exception(nameof(category));
            }
            var categoriaToUpdate = await _context.Categories.FindAsync(category.Id);
            if (categoriaToUpdate == null)
            {
                throw new Exception($"Categoria con id {category.Id} no encontrada");
            }
            categoriaToUpdate.Nombre = category.Nombre;
            categoriaToUpdate.Descripcion = category.Descripcion;
            await _context.SaveChangesAsync();
            return categoriaToUpdate;

        }
    }
}
