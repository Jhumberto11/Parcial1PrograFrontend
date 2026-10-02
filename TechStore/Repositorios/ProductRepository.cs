using TechStore.Data;
using TechStore.Models;
using TechStore.Models.Interface;
using Microsoft.EntityFrameworkCore;
namespace TechStore.Repositorios
{
    public class ProductRepository : IProductoRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Producto> AddProductAsync(Producto product)
        {
           

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
           
            var result = await _context.Products.FindAsync(id);  
            _context.Products.Remove(result);
            await _context.SaveChangesAsync();
            return true;

        }

        public async Task<IEnumerable<Producto>> GetAllProductsAsync()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Producto> GetProductByIdAsync(int id)
        {
            var result = await _context.Products.FindAsync(id);
            if (result == null)
            {
                throw new Exception($"Producto con id {id} no encontrado");
            }
            return result;
        }

        public async Task<Producto> UpdateProductAsync(Producto product)
        {
            

            var productToUpdate = await _context.Products.FindAsync(product.Id);
            if (productToUpdate == null)
            {
                throw new Exception($"Producto con id {product.Id} no encontrado");
            }

            productToUpdate.Nombre = product.Nombre;
            productToUpdate.Descripcion = product.Descripcion;
            productToUpdate.Precio = product.Precio;
            productToUpdate.Stock = product.Stock;

            _context.Update(productToUpdate);
            await _context.SaveChangesAsync();

            return productToUpdate;

        }
    }
}
