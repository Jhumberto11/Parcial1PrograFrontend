using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechStore.Models{
    public class Producto : Base
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(2000, ErrorMessage = "La descripción no puede pasar de 2000 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor que 0")]
        [Column(TypeName = "decimal(18,2)")]        
        public decimal Precio { get; set; }

        [Range(0, 100000, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }

        [StringLength(300, ErrorMessage = "La ruta de la imagen es muy larga")]
        public string? Imagen { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debes elegir una categoría")]
        public int CategoriaId { get; set; }

        public Categoria? Categoria { get; set; }

        // Sirve para escoger cuales se muestran en la pagina de inicio
        public bool Destacado { get; set; } = false;

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
