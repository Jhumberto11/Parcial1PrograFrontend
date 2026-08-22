namespace TechStore.Models
{
    public class Producto : Base
    {
        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public int Stock { get; set; }

        public string Imagen { get; set; } = string.Empty;

        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        // Sirve para escoger cuales se muestran en la pagina de inicio
        public bool Destacado { get; set; }

        // El estado se calcula a partir del stock, no se guarda aparte
        public string Estado
        {
            get { return Stock > 0 ? "Disponible" : "Agotado"; }
        }

        public bool HayExistencias
        {
            get { return Stock > 0; }
        }
    }
}
