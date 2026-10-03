using System.ComponentModel.DataAnnotations;


namespace TechStore.Models
{
    public class Categoria : Base
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 60 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(300, ErrorMessage = "La descripcion no puede pasar de 300 caracteres")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ruta de la imagen es obligatoria")]
        [StringLength(300, ErrorMessage = "La ruta de la imagen es muy larga")]
        public string Imagen { get; set; } = string.Empty;
    }
}
