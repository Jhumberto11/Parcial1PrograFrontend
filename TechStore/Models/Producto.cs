namespace TechStore.Models
{
    public class Producto : Base
    {
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public bool Disponible
        {
            get { return Cantidad > 0; }
        }

        public Categoria Categoria { get; set; }

    }
    public enum Categorias
    {
        AccesoriosCarros,
        AccesoriosComputadoras,
        AccesoriosPersonales,
        Computadoras,
        Telefonos,
        AccesoriosTelefonos,
        Mouses,
        Teclados
    }
}
