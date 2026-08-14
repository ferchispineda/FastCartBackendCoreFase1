using System;
using System.Diagnostics;
using System.Collections.Generic;
using FastCartBackendCore;

class Program
{
    static void Main()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("       FASTCART BACKEND CORE");
        Console.WriteLine("========================================");
        Console.WriteLine();

        EjecutarFase1();

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("     FASE 2 - LISTA ENLAZADA");
        Console.WriteLine("========================================");
        Console.WriteLine();

        EjecutarFase2();
    }

    static void EjecutarFase1()
    {
        Console.WriteLine("FASE 1 - ORDENAMIENTO SHELLSORT");
        Console.WriteLine("----------------------------------------");

        Producto[] catalogo = GenerarCatalogo(50);

        Console.WriteLine($"Total de productos: {catalogo.Length}");
        Console.WriteLine();

        Console.WriteLine("PRIMEROS 5 PRODUCTOS ANTES DEL ORDENAMIENTO");
        Console.WriteLine("--------------------------------------------");

        MostrarPrimerosCinco(catalogo);

        Stopwatch sw = Stopwatch.StartNew();

        OrdenamientoService.ShellSort(catalogo);

        sw.Stop();

        Console.WriteLine();
        Console.WriteLine("PRIMEROS 5 PRODUCTOS DESPUES DEL SHELLSORT");
        Console.WriteLine("--------------------------------------------");

        MostrarPrimerosCinco(catalogo);

        Console.WriteLine();
        Console.WriteLine("RESULTADOS DE RENDIMIENTO");
        Console.WriteLine("--------------------------------------------");

        Console.WriteLine($"Tiempo: {sw.ElapsedMilliseconds} ms");
        Console.WriteLine(
            $"Microsegundos: {sw.Elapsed.TotalMicroseconds:F2} us"
        );
        Console.WriteLine($"Ticks: {sw.ElapsedTicks}");
        Console.WriteLine(
            $"Total de productos procesados: {catalogo.Length}"
        );
    }

    static void EjecutarFase2()
    {
        AuditoriaService auditoria = new AuditoriaService();

        InventarioLista inventario =
            new InventarioLista(auditoria);

        Producto[] productos =
        {
            CrearProducto(2001, "Laptop", 18500.00, 12, 1),
            CrearProducto(2002, "Mouse", 350.00, 40, 2),
            CrearProducto(2003, "Teclado", 850.00, 25, 3),
            CrearProducto(2004, "Monitor", 4200.00, 10, 4),
            CrearProducto(2005, "Audifonos", 1250.00, 30, 5),
            CrearProducto(2006, "Webcam", 950.00, 20, 1),
            CrearProducto(2007, "Memoria USB", 280.00, 50, 2),
            CrearProducto(2008, "SSD", 1600.00, 18, 3),
            CrearProducto(2009, "Tablet", 7200.00, 8, 4),
            CrearProducto(2010, "Impresora", 3100.00, 7, 5),
            CrearProducto(2011, "Bocinas", 1100.00, 22, 1),
            CrearProducto(2012, "Router", 1450.00, 15, 2),
            CrearProducto(2013, "Disco Duro", 1800.00, 17, 3),
            CrearProducto(2014, "Adaptador", 220.00, 35, 4),
            CrearProducto(2015, "Microfono", 2100.00, 11, 5)
        };

        Console.WriteLine(
            "Insertando 15 productos dinamicamente..."
        );
        Console.WriteLine();

        foreach (Producto producto in productos)
        {
            inventario.InsertarOrdenado(producto);

            Console.WriteLine(
                $"Insertado: SKU {producto.SKU} - " +
                $"{producto.Nombre} - ${producto.Precio:F2}"
            );
        }

        Console.WriteLine();
        Console.WriteLine("CATALOGO ORDENADO POR PRECIO");
        Console.WriteLine("----------------------------------------");

        inventario.MostrarProductos();

        Console.WriteLine();
        Console.WriteLine("PRUEBA DE BUSQUEDA POR SKU");
        Console.WriteLine("----------------------------------------");

        try
        {
            Producto encontrado =
                inventario.BuscarPorSKU(2008);

            Console.WriteLine(
                $"Producto encontrado: {encontrado.Nombre} " +
                $"- ${encontrado.Precio:F2}"
            );
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(ex.Message);
        }

        Console.WriteLine();
        Console.WriteLine("PRUEBA DE SKU INEXISTENTE");
        Console.WriteLine("----------------------------------------");

        try
        {
            inventario.BuscarPorSKU(9999);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine(
                $"Excepcion controlada: {ex.Message}"
            );
        }

        Console.WriteLine();
        Console.WriteLine("PRUEBA DE ACTUALIZACION");
        Console.WriteLine("----------------------------------------");

        inventario.ActualizarPrecio(2008, 1750.00);

        Producto actualizado =
            inventario.BuscarPorSKU(2008);

        Console.WriteLine(
            $"Nuevo precio del producto {actualizado.Nombre}: " +
            $"${actualizado.Precio:F2}"
        );

        Console.WriteLine();
        Console.WriteLine("PRUEBA DE ELIMINACION");
        Console.WriteLine("----------------------------------------");

        inventario.EliminarPorSKU(2005);

        Console.WriteLine(
            "Producto SKU 2005 eliminado."
        );

        Console.WriteLine();
        Console.WriteLine(
            "CATALOGO DESPUES DE LA ACTUALIZACION Y ELIMINACION"
        );
        Console.WriteLine("----------------------------------------");

        inventario.MostrarProductos();

        Console.WriteLine();
        Console.WriteLine("========================================");
        Console.WriteLine("   FASE 3 - BITACORA DE AUDITORIA");
        Console.WriteLine("========================================");

        Console.WriteLine();
        Console.WriteLine("RECORRIDO CRONOLOGICO");
        Console.WriteLine("----------------------------------------");

        auditoria.ImprimirHistorialCronologico();

        Console.WriteLine();
        Console.WriteLine("RECORRIDO INVERSO");
        Console.WriteLine("----------------------------------------");

        auditoria.ImprimirHistorialInverso();
    }

    static Producto CrearProducto(
        int sku,
        string nombre,
        double precio,
        int stock,
        int idProveedor)
    {
        return new Producto
        {
            SKU = sku,
            Nombre = nombre,
            Precio = precio,
            Stock = stock,

            DatosProveedor = new Proveedor
            {
                IdProveedor = idProveedor,
                NombreCorporativo =
                    $"Proveedor {idProveedor}"
            }
        };
    }

    static Producto[] GenerarCatalogo(int cantidad)
    {
        Producto[] productos =
            new Producto[cantidad];

        Random random = new Random(42);

        for (int i = 0; i < cantidad; i++)
        {
            productos[i] = new Producto
            {
                SKU = 1001 + i,

                Nombre =
                    $"Producto {i + 1}",

                Precio = Math.Round(
                    10 +
                    random.NextDouble() *
                    (9999.99 - 10),
                    2
                ),

                Stock =
                    random.Next(0, 501),

                DatosProveedor = new Proveedor
                {
                    IdProveedor =
                        (i % 5) + 1,

                    NombreCorporativo =
                        $"Proveedor {(i % 5) + 1}"
                }
            };
        }

        productos[5].Precio = 2500.00;
        productos[15].Precio = 2500.00;
        productos[25].Precio = 2500.00;

        return productos;
    }

    static void MostrarPrimerosCinco(
        Producto[] catalogo)
    {
        Console.WriteLine(
            $"{"SKU",-8} " +
            $"{"NOMBRE",-18} " +
            $"{"PRECIO",-14} " +
            $"{"STOCK",-8} " +
            $"{"PROVEEDOR",-15}"
        );

        int limite =
            Math.Min(5, catalogo.Length);

        for (int i = 0; i < limite; i++)
        {
            Producto producto =
                catalogo[i];

            Console.WriteLine(
                $"{producto.SKU,-8} " +
                $"{producto.Nombre,-18} " +
                $"${producto.Precio,-13:F2} " +
                $"{producto.Stock,-8} " +
                $"{producto.DatosProveedor.NombreCorporativo,-15}"
            );
        }
    }
}