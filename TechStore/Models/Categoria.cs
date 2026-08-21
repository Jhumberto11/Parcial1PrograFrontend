namespace TechStore.Models
{
    public class Categoria : Base   
    {
        public string  Nombre {  get; set; }
    
        public string? Descripcion {  get; set; }

        public string? ImageUrl { get; set; }
    }
}
