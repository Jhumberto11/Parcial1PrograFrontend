using Microsoft.AspNetCore.Mvc;
using TechStore.Models;
using System.Security.Cryptography.X509Certificates;

namespace TechStore.Controllers
{

    public class ProductosController : Controller
    {
        public IActionResult Index()
        {

            return View(products);

        }

        public ICollection<Producto> products = new List<Producto>()
        {
            new Producto
            {
                Nombre = "Laptop Lenovo IdeaPad",
                Precio = 649.99m,
                Descripcion = "Laptop ideal para estudiantes, trabajo de oficina y uso diario.",
                Cantidad = 8,
                UrlImagen = "/img/laptop-lenovo.jpg",
                Categoria = Categorias.Computadoras
            }, new Producto
            {
                Nombre = "Laptop ASUS VivoBook",
                Precio = 729.99m,
                Descripcion = "Computadora portátil con buen rendimiento para trabajo y entretenimiento.",
                Cantidad = 5,
                UrlImagen = "/img/laptop-asus.jpg",
                Categoria = Categorias.Computadoras
            },

            new Producto
            {
                Nombre = "Samsung Galaxy S24",
                Precio = 799.99m,
                Descripcion = "Smartphone moderno con excelente pantalla, cámara y rendimiento.",
                Cantidad = 10,
                UrlImagen = "/img/samsung-s24.jpg",
                Categoria = Categorias.Telefonos
            },

            new Producto
            {
                Nombre = "iPhone 15",
                Precio = 899.99m,
                Descripcion = "Teléfono Apple con excelente rendimiento, cámara y calidad de construcción.",
                Cantidad = 4,
                UrlImagen = "/img/iphone-15.jpg",
                Categoria = Categorias.Telefonos
            },

            new Producto
            {
                Nombre = "Mouse Logitech G203",
                Precio = 29.99m,
                Descripcion = "Mouse gaming con sensor preciso y diseño cómodo para uso prolongado.",
                Cantidad = 15,
                UrlImagen = "/img/mouse-logitech.jpg",
                Categoria = Categorias.Mouses
            },

            new Producto
            {
                Nombre = "Mouse Inalámbrico HP",
                Precio = 19.99m,
                Descripcion = "Mouse inalámbrico compacto para computadoras portátiles y de escritorio.",
                Cantidad = 12,
                UrlImagen = "/img/mouse-hp.jpg",
                Categoria = Categorias.Mouses
            },

            new Producto
            {
                Nombre = "Teclado Mecánico Redragon",
                Precio = 54.99m,
                Descripcion = "Teclado mecánico con iluminación RGB ideal para gaming y trabajo.",
                Cantidad = 7,
                UrlImagen = "/img/teclado-redragon.jpg",
                Categoria = Categorias.Teclados
            },

            new Producto
            {
                Nombre = "Teclado Logitech K120",
                Precio = 18.99m,
                Descripcion = "Teclado sencillo y confiable para oficina, estudio y uso cotidiano.",
                Cantidad = 20,
                UrlImagen = "/img/teclado-logitech.jpg",
                Categoria = Categorias.Teclados
            },

            new Producto
            {
                Nombre = "Audífonos Bluetooth JBL",
                Precio = 44.99m,
                Descripcion = "Audífonos inalámbricos con sonido claro y batería de larga duración.",
                Cantidad = 9,
                UrlImagen = "/img/audifonos-jbl.jpg",
                Categoria = Categorias.AccesoriosPersonales
            },

            new Producto
            {
                Nombre = "Cargador USB-C 25W",
                Precio = 19.99m,
                Descripcion = "Cargador rápido USB-C compatible con diferentes modelos de teléfonos.",
                Cantidad = 25,
                UrlImagen = "/img/cargador-usbc.jpg",
                Categoria = Categorias.AccesoriosTelefonos
            },

            new Producto
            {
                Nombre = "Soporte para Teléfono",
                Precio = 12.99m,
                Descripcion = "Soporte ajustable para colocar el teléfono cómodamente sobre el escritorio.",
                Cantidad = 14,
                UrlImagen = "/img/soporte-telefono.jpg",
                Categoria = Categorias.AccesoriosTelefonos
            },

            new Producto
            {
                Nombre = "Hub USB 4 Puertos",
                Precio = 24.99m,
                Descripcion = "Adaptador USB con cuatro puertos para conectar múltiples dispositivos.",
                Cantidad = 11,
                UrlImagen = "/img/hub-usb.jpg",
                Categoria = Categorias.AccesoriosComputadoras
            },

            new Producto
            {
                Nombre = "Base Refrigerante para Laptop",
                Precio = 34.99m,
                Descripcion = "Base con ventiladores para ayudar a mantener una temperatura adecuada.",
                Cantidad = 6,
                UrlImagen = "/img/base-laptop.jpg",
                Categoria = Categorias.AccesoriosComputadoras
            },

            new Producto
            {
                Nombre = "Cargador para Carro USB",
                Precio = 14.99m,
                Descripcion = "Cargador para vehículo con puertos USB para teléfonos y accesorios.",
                Cantidad = 18,
                UrlImagen = "/img/cargador-carro.jpg",
                Categoria = Categorias.AccesoriosCarros
            },

            new Producto
            {
                Nombre = "Soporte de Teléfono para Carro",
                Precio = 16.99m,
                Descripcion = "Soporte ajustable para colocar el teléfono de forma cómoda en el vehículo.",
                Cantidad = 0,
                UrlImagen = "/img/soporte-carro.jpg",
                Categoria = Categorias.AccesoriosCarros
            }
        };
    } 
}
            

        
            

    

