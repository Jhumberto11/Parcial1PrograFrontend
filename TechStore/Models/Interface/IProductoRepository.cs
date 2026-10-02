namespace TechStore.Models.Interface
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetAllProductsAsync();
        Task<Producto> GetProductByIdAsync(int id);
        Task<Producto> AddProductAsync(Producto product);
        Task<Producto> UpdateProductAsync(Producto product);
        Task<bool> DeleteProductAsync(int id);
    }
}
