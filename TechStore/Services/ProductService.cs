using Microsoft.EntityFrameworkCore;
using TechStore.Models;
using TechStore.Models.Interface;

namespace TechStore.Services
{
    public class ProductService
    {
        private readonly IProductoRepository _productoRepository;
        public ProductService(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        public async Task<Producto> AddProductAsync(Producto product)
        {
            if (product == null)
            {
                throw new Exception(nameof(product));
            }
            return await _productoRepository.AddProductAsync(product);
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            if (id <= 0)
            {
                throw new Exception("Id debe ser un valor positivo");
            }

            var result = await _productoRepository.GetProductByIdAsync(id);
            if (result == null)
            {
                throw new Exception($"Producto con id {id} no encontrado");
            }
            return await _productoRepository.DeleteProductAsync(id);
        }


        public async Task<IEnumerable<Producto>> GetAllProductsAsync()
        {
            var list = await _productoRepository.GetAllProductsAsync();
            if(list == null || !list.Any())
            {
                throw new Exception("No se encontraron productos");
            }
            return list;
        }   

        public async Task<Producto> GetProductByIdAsync(int id)
        {
            var product = await _productoRepository.GetProductByIdAsync(id);
            if (product == null)
            {
                throw new Exception($"Producto con id {id} no encontrado");
            }
            return product;
        }

        public async Task<Producto> UpdateProductAsync(Producto productToUpdate)
        {
            if (productToUpdate == null)
            {
                throw new Exception(nameof(productToUpdate));
            }

            await _productoRepository.UpdateProductAsync(productToUpdate);
            return productToUpdate;


        }

    }
}
