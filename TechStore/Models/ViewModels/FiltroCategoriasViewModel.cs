namespace TechStore.Models.ViewModels
{
    public class FiltroCategoriasViewModel
    {
        public List<Categoria> Categorias { get; set; } = new();
        public int? CategoriaSeleccionada { get; set; }
    }
}
