namespace TechStore.Models
{
    // Como no usamos base de datos, los datos se guardan en listas en memoria.
    // Se declaran como static para que no se vuelvan a crear en cada peticion.
    public static class Datos
    {
        public static List<Categoria> Categorias = new List<Categoria>()
        {
            new Categoria
            {
                Id = 1,
                Nombre = "Computadoras",
                Descripcion = "Laptops y computadoras de escritorio para estudiar, trabajar y jugar.",
                Imagen = "/images/computadoras.jpg"
            },
            new Categoria
            {
                Id = 2,
                Nombre = "Telefonos",
                Descripcion = "Smartphones de diferentes marcas y precios para cada necesidad.",
                Imagen = "/images/telefonos.jpg"
            },
            new Categoria
            {
                Id = 3,
                Nombre = "Accesorios de Computadora",
                Descripcion = "Teclados, mouses, hubs y todo lo que le hace falta a tu equipo.",
                Imagen = "/images/accesorios.jpg"
            },
            new Categoria
            {
                Id = 4,
                Nombre = "Accesorios de Telefono",
                Descripcion = "Cargadores, soportes y accesorios para tu telefono.",
                Imagen = "/images/cat-accesorios-telefono.jpg"
            },
            new Categoria
            {
                Id = 5,
                Nombre = "Audio",
                Descripcion = "Audifonos y parlantes con buen sonido y bateria de larga duracion.",
                Imagen = "/images/cat-audio.jpg"
            },
            new Categoria
            {
                Id = 6,
                Nombre = "Accesorios para Carro",
                Descripcion = "Cargadores y soportes pensados para usar en el vehiculo.",
                Imagen = "/images/cat-carro.jpg"
            }
        };

        public static List<Producto> Productos = new List<Producto>()
        {
            new Producto
            {
                Id = 1,
                Nombre = "Laptop Lenovo IdeaPad",
                Descripcion = "Laptop ideal para estudiantes, trabajo de oficina y uso diario.",
                Precio = 649.99m,
                Stock = 8,
                Imagen = "/images/laptop-lenovo.jpg",
                CategoriaId = 1,
                Destacado = true
            },
            new Producto
            {
                Id = 2,
                Nombre = "Laptop ASUS VivoBook",
                Descripcion = "Computadora portatil con buen rendimiento para trabajo y entretenimiento.",
                Precio = 729.99m,
                Stock = 5,
                Imagen = "/images/laptop-asus.jpg",
                CategoriaId = 1
            },
            new Producto
            {
                Id = 3,
                Nombre = "PC de Escritorio HP",
                Descripcion = "Equipo de escritorio confiable para oficina, estudio y uso en casa.",
                Precio = 549.99m,
                Stock = 3,
                Imagen = "/images/pc-escritorio.jpg",
                CategoriaId = 1
            },
            new Producto
            {
                Id = 4,
                Nombre = "Samsung Galaxy S24",
                Descripcion = "Smartphone moderno con excelente pantalla, camara y rendimiento.",
                Precio = 799.99m,
                Stock = 10,
                Imagen = "/images/samsung-s24.jpg",
                CategoriaId = 2,
                Destacado = true
            },
            new Producto
            {
                Id = 5,
                Nombre = "iPhone 15",
                Descripcion = "Telefono Apple con excelente rendimiento, camara y calidad de construccion.",
                Precio = 899.99m,
                Stock = 4,
                Imagen = "/images/iphone-15.jpg",
                CategoriaId = 2,
                Destacado = true
            },
            new Producto
            {
                Id = 6,
                Nombre = "Xiaomi Redmi Note 13",
                Descripcion = "Telefono con buena bateria y camara a un precio accesible.",
                Precio = 249.99m,
                Stock = 12,
                Imagen = "/images/xiaomi-redmi.jpg",
                CategoriaId = 2
            },
            new Producto
            {
                Id = 7,
                Nombre = "Mouse Logitech G203",
                Descripcion = "Mouse gaming con sensor preciso y diseno comodo para uso prolongado.",
                Precio = 29.99m,
                Stock = 15,
                Imagen = "/images/mouse-logitech.jpg",
                CategoriaId = 3
            },
            new Producto
            {
                Id = 8,
                Nombre = "Teclado Mecanico Redragon",
                Descripcion = "Teclado mecanico con iluminacion RGB ideal para gaming y trabajo.",
                Precio = 54.99m,
                Stock = 7,
                Imagen = "/images/teclado-redragon.jpg",
                CategoriaId = 3,
                Destacado = true
            },
            new Producto
            {
                Id = 9,
                Nombre = "Hub USB 4 Puertos",
                Descripcion = "Adaptador USB con cuatro puertos para conectar multiples dispositivos.",
                Precio = 24.99m,
                Stock = 11,
                Imagen = "/images/hub-usb.jpg",
                CategoriaId = 3
            },
            new Producto
            {
                Id = 10,
                Nombre = "Cargador USB-C 25W",
                Descripcion = "Cargador rapido USB-C compatible con diferentes modelos de telefonos.",
                Precio = 19.99m,
                Stock = 25,
                Imagen = "/images/cargador-usbc.jpg",
                CategoriaId = 4
            },
            new Producto
            {
                Id = 11,
                Nombre = "Soporte para Telefono",
                Descripcion = "Soporte ajustable para colocar el telefono comodamente sobre el escritorio.",
                Precio = 12.99m,
                Stock = 14,
                Imagen = "/images/soporte-telefono.jpg",
                CategoriaId = 4
            },
            new Producto
            {
                Id = 12,
                Nombre = "Audifonos Bluetooth JBL",
                Descripcion = "Audifonos inalambricos con sonido claro y bateria de larga duracion.",
                Precio = 44.99m,
                Stock = 9,
                Imagen = "/images/audifonos-jbl.jpg",
                CategoriaId = 5,
                Destacado = true
            },
            new Producto
            {
                Id = 13,
                Nombre = "Parlante Portatil Bose",
                Descripcion = "Parlante compacto con buen volumen para llevar a cualquier lugar.",
                Precio = 129.99m,
                Stock = 6,
                Imagen = "/images/parlante-bose.jpg",
                CategoriaId = 5
            },
            new Producto
            {
                Id = 14,
                Nombre = "Cargador para Carro USB",
                Descripcion = "Cargador para vehiculo con puertos USB para telefonos y accesorios.",
                Precio = 14.99m,
                Stock = 18,
                Imagen = "/images/cargador-carro.jpg",
                CategoriaId = 6
            },
            new Producto
            {
                Id = 15,
                Nombre = "Soporte de Telefono para Carro",
                Descripcion = "Soporte ajustable para colocar el telefono de forma comoda en el vehiculo.",
                Precio = 16.99m,
                Stock = 0,
                Imagen = "/images/soporte-carro.jpg",
                CategoriaId = 6
            }
        };

        // Le asigna a cada producto el objeto de su categoria para poder mostrar el nombre
        static Datos()
        {
            foreach (var producto in Productos)
            {
                producto.Categoria = Categorias.FirstOrDefault(c => c.Id == producto.CategoriaId);
            }
        }
    }
}
