using TechStore.Models;
using TechStore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TechStore.Controllers{
    [Authorize(Roles = "Administrador, Vendedor")]
    public class ReportesController : Controller{

        private readonly ProductService _productService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(ProductService productService, ILogger<ReportesController> logger){
           _productService = productService;
           _logger = logger;
        }

        public async Task<IActionResult> Index(){
            var productos = await ObtenerProductos();
            return View(productos.Count);
        }

        public async Task<IActionResult> ProductosPdf(){
            _logger.LogInformation("Generando el reporte PDF de productos");
            var productos = await ObtenerProductos();
            return File(GenerarReportePdf(productos), "application/pdf");
        }

        public async Task<IActionResult> DescargarProductosPdf(){
            _logger.LogInformation("Descargando el reporte PDF de productos");
            var productos = await ObtenerProductos();
            return File(GenerarReportePdf(productos), "application/pdf", "ReporteProductos.pdf");
        }

        public async Task<IActionResult> ExportarProductosExcel(){
            _logger.LogInformation("Exportando el reporte de productos a Excel");
            var productos = await ObtenerProductos();
            return File(GenerarReporteExcel(productos),"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","ReporteProductos.xlsx");
        }

        private async Task<List<Producto>> ObtenerProductos(){
            var productos = await _productService.GetAllProductsAsync();
            return productos.OrderBy(p => p.Nombre).ToList();
        }

        private static byte[] GenerarReporteExcel(IReadOnlyCollection<Producto> productos){
            using var workbook = new XLWorkbook();
            var hoja = workbook.Worksheets.Add("Reporte de Productos");

            hoja.Cell(1, 1).Value = "Reporte de productos: TechStore";
            hoja.Range(1, 1, 1, 7).Merge();
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 16;
            hoja.Cell(1, 1).Style.Font.FontColor = XLColor.FromHtml("#1565C0");
            hoja.Cell(2, 1).Value = $"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}";
            hoja.Range(2, 1, 2, 7).Merge();
            hoja.Cell(2, 1).Style.Font.FontColor = XLColor.Gray;

            var encabezados = new[]{
                "ID",
                "Nombre",
                "Descripción",
                "Precio",
                "Categoría",
                "Stock",
                "Estado" };
            for (int i = 0; i < encabezados.Length; i++){
                hoja.Cell(4, i + 1).Value = encabezados[i];
            }

            var encabezadoTabla = hoja.Range(4, 1, 4, 7);
            encabezadoTabla.Style.Font.Bold = true;
            encabezadoTabla.Style.Font.FontColor = XLColor.White;
            encabezadoTabla.Style.Fill.BackgroundColor = XLColor.FromHtml("#1565C0");
            encabezadoTabla.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            var fila = 5;
            foreach (var p in productos){
                hoja.Cell(fila, 1).Value = p.Id;
                hoja.Cell(fila, 2).Value = p.Nombre;
                hoja.Cell(fila, 3).Value = p.Descripcion;
                hoja.Cell(fila, 4).Value = p.Precio;
                hoja.Cell(fila, 5).Value = p.Categoria?.Nombre ?? "Sin categoría";
                hoja.Cell(fila, 6).Value = p.Stock;
                hoja.Cell(fila, 7).Value = p.Estado;
                fila++;
            }
            var bordesTabla = hoja.Range(4, 1, Math.Max(fila - 1, 4), 7);
            bordesTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            bordesTabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            bordesTabla.Style.Border.OutsideBorderColor = XLColor.LightGray;
            bordesTabla.Style.Border.InsideBorderColor = XLColor.LightGray;
            bordesTabla.Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            hoja.Range(5, 1, Math.Max(fila - 1, 5), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Range(5, 6, Math.Max(fila - 1, 5), 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Cell(fila + 1, 6).Value = "Total de productos:";
            hoja.Cell(fila + 1, 6).Style.Font.Bold = true;
            hoja.Cell(fila + 1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            hoja.Cell(fila + 1, 7).Value = productos.Count;
            hoja.Cell(fila + 1, 7).Style.Font.Bold = true;
            hoja.Columns(1, 7).AdjustToContents();
            hoja.Column(3).Width = 50;
            hoja.Column(3).Style.Alignment.WrapText = true;
            hoja.Column(4).Style.NumberFormat.Format = "$#,##0.00";
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static byte[] GenerarReportePdf(IReadOnlyCollection<Producto> productos){
            return Document.Create(document => document.Page(page =>{
                page.Size(PageSizes.Letter.Landscape());
                page.Margin(36);
                page.PageColor(Colors.Grey.Lighten5);
                page.Header().Column(column =>{
                    column.Item().Text("TECHSTORE").FontSize(11).Bold()
                    .FontColor(Colors.Blue.Darken2);
                    column.Item().Text("Reporte de productos").FontSize(26).Bold()
                    .FontColor(Colors.Grey.Darken3);
                    column.Item().Text($"Generado el {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10)
                    .FontColor(Colors.Grey.Darken1);
                    column.Item().PaddingTop(14).LineHorizontal(2).LineColor(Colors.Blue.Medium);
                });

                page.Content().PaddingVertical(22).Column(column =>{
                    column.Item().Background(Colors.Blue.Darken2).Padding(14).Row(row =>{
                        row.RelativeItem().Text("Productos Registrados").FontSize(11).Bold()
                        .FontColor(Colors.White);
                        row.ConstantItem(45).AlignRight().Text(productos.Count.ToString())
                        .FontSize(18).Bold()
                        .FontColor(Colors.White);
                    });

                    column.Item().PaddingTop(18).Table(table =>{
                        table.ColumnsDefinition(columns =>{
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(4);
                            columns.RelativeColumn(1);
                            columns.RelativeColumn(1.5f);
                            columns.RelativeColumn(0.8f);
                            columns.RelativeColumn(1.2f);
                        });

                        table.Header(header =>{
                            header.Cell().Element(HeaderCellStyle).Text("NOMBRE");
                            header.Cell().Element(HeaderCellStyle).Text("DESCRIPCIÓN");
                            header.Cell().Element(HeaderCellStyle).Text("PRECIO");
                            header.Cell().Element(HeaderCellStyle).Text("CATEGORÍA");
                            header.Cell().Element(HeaderCellStyle).Text("STOCK");
                            header.Cell().Element(HeaderCellStyle).Text("ESTADO");
                        });

                        foreach (var p in productos){
                            table.Cell().Element(BodyCellStyle).Text(p.Nombre);
                            table.Cell().Element(BodyCellStyle).Text(p.Descripcion);
                            table.Cell().Element(BodyCellStyle).Text($"${p.Precio:N2}");
                            table.Cell().Element(BodyCellStyle).Text(p.Categoria?.Nombre ?? "Sin categoría");
                            table.Cell().Element(BodyCellStyle).Text(p.Stock.ToString());
                            table.Cell().Element(BodyCellStyle).Text(p.Estado);
                        }
                    });
                });

                page.Footer().AlignCenter().Text(text =>{
                    text.Span("TechStore – Página ");
                    text.CurrentPageNumber();
                    text.Span(" de ");
                    text.TotalPages();
                });
            })).GeneratePdf();

            static IContainer HeaderCellStyle(IContainer container) =>  container
            .Background(Colors.Blue.Darken2)
            .PaddingVertical(9)
            .PaddingHorizontal(8)
            .DefaultTextStyle(text => text.FontSize(9).Bold().FontColor(Colors.White));

            static IContainer BodyCellStyle(IContainer container) => container
            .Background(Colors.White)
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(9)
            .PaddingHorizontal(8)
            .DefaultTextStyle(text =>text.FontSize(9).FontColor(Colors.Grey.Darken3));
        }
    }
}
