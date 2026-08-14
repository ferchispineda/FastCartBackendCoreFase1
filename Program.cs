using System;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine("========================================");
        Console.WriteLine("       FASTCART BACKEND CORE");
        Console.WriteLine("========================================");
        Console.WriteLine();

        Producto[] catalogo = GenerarCatalogo(50);

        Console.WriteLine($"Total de productos: {catalogo.Length}");
        Console.WriteLine();

        // Mostrar solo los primeros 5 antes del ordenamiento
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
        Console.WriteLine($"Microsegundos: {sw.Elapsed.TotalMicroseconds:F2} us");
        Console.WriteLine($"Ticks: {sw.ElapsedTicks}");
        Console.WriteLine($"Total de productos procesados: {catalogo.Length}");

        Console.WriteLine();
        Console.WriteLine("Ordenamiento completado correctamente.");
    }

    static Producto[] GenerarCatalogo(int cantidad)
    {
        Producto[] productos = new Producto[cantidad];

        // Semilla fija para obtener resultados reproducibles
        Random random = new Random(42);

        for (int i = 0; i < cantidad; i++)
        {
            productos[i] = new Producto
            {
                SKU = 1001 + i,
                Nombre = $"Producto {i + 1}",

                // Precio entre $10.00 y $9,999.99
                Precio = Math.Round(
                    10 + random.NextDouble() * (9999.99 - 10),
                    2
                ),

                // Stock entre 0 y 500
                Stock = random.Next(0, 501),

                DatosProveedor = new Proveedor
                {
                    IdProveedor = (i % 5) + 1,
                    NombreCorporativo = $"Proveedor {(i % 5) + 1}"
                }
            };
        }

        // Productos con el mismo precio para probar
        // el desempate por SKU ascendente.
        productos[5].Precio = 2500.00;
        productos[15].Precio = 2500.00;
        productos[25].Precio = 2500.00;

        return productos;
    }

    static void MostrarPrimerosCinco(Producto[] catalogo)
    {
        Console.WriteLine(
            $"{"SKU",-8} {"NOMBRE",-18} {"PRECIO",-14} {"STOCK",-8} {"PROVEEDOR",-15}"
        );

        int limite = Math.Min(5, catalogo.Length);

        for (int i = 0; i < limite; i++)
        {
            Producto producto = catalogo[i];

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