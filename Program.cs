using System;
using System.Diagnostics;
using System.Collections.Generic;
using FastCartBackendCore;

class Program
{
    static AuditoriaService auditoria =
        new AuditoriaService();

    static InventarioLista inventario =
        new InventarioLista(auditoria);

    static ColaDespacho cola =
        new ColaDespacho();

    static PilaDevoluciones pila =
        new PilaDevoluciones();

    static void Main()
    {
        Console.Title =
            "FastCart Backend Core - Motor Logistico v4.0";

        CargarInventarioInicial();

        bool ejecutando = true;

        while (ejecutando)
        {
            MostrarMenu();

            Console.Write("Seleccione una opción: ");
            string? opcion = Console.ReadLine();

            Console.WriteLine();

            try
            {
                switch (opcion)
                {
                    case "1":
                        EjecutarFase1();
                        break;

                    case "2":
                        AgregarProducto();
                        break;

                    case "3":
                        BuscarProducto();
                        break;

                    case "4":
                        EliminarProducto();
                        break;

                    case "5":
                        MostrarCatalogo();
                        break;

                    case "6":
                        MostrarBitacora();
                        break;

                    case "7":
                        ValidarBitacora();
                        break;

                    case "8":
                        EncolarPedido();
                        break;

                    case "9":
                        DespacharPedido();
                        break;

                    case "10":
                        RegistrarDevolucion();
                        break;

                    case "11":
                        ProcesarDevolucion();
                        break;

                    case "12":
                        MostrarEstadoLogistico();
                        break;

                    case "0":
                        ejecutando = false;

                        Console.WriteLine(
                            "Saliendo de FastCart Backend Core..."
                        );

                        break;

                    default:
                        Console.WriteLine(
                            "Opción no válida."
                        );
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine(
                    "ERROR: Debe ingresar un valor numérico válido."
                );
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine(
                    $"ERROR: {ex.Message}"
                );
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine(
                    $"ERROR: {ex.Message}"
                );
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(
                    $"ERROR: {ex.Message}"
                );
            }

            if (ejecutando)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Presione ENTER para regresar al menú..."
                );

                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    static void MostrarMenu()
    {
        Console.WriteLine(
            "=================================================="
        );

        Console.WriteLine(
            "          FASTCART BACKEND CORE v4.0"
        );

        Console.WriteLine(
            "=================================================="
        );

        Console.WriteLine(
            "FASE 1 - ORDENAMIENTO"
        );

        Console.WriteLine(
            "[1] Ejecutar demostración ShellSort"
        );

        Console.WriteLine();

        Console.WriteLine(
            "FASE 2 - LISTA DINÁMICA DE INVENTARIO"
        );

        Console.WriteLine(
            "[2] Agregar producto"
        );

        Console.WriteLine(
            "[3] Buscar producto por SKU"
        );

        Console.WriteLine(
            "[4] Eliminar producto"
        );

        Console.WriteLine(
            "[5] Mostrar catálogo"
        );

        Console.WriteLine();

        Console.WriteLine(
            "FASE 3 - BITÁCORA DE AUDITORÍA"
        );

        Console.WriteLine(
            "[6] Ver historial de bitácora"
        );

        Console.WriteLine(
            "[7] Validar integridad de bitácora"
        );

        Console.WriteLine();

        Console.WriteLine(
            "FASE 4 - PILAS Y COLAS DINÁMICAS"
        );

        Console.WriteLine(
            "[8] Encolar nuevo pedido"
        );

        Console.WriteLine(
            "[9] Despachar pedido FIFO"
        );

        Console.WriteLine(
            "[10] Registrar devolución LIFO"
        );

        Console.WriteLine(
            "[11] Procesar devolución"
        );

        Console.WriteLine(
            "[12] Ver estado de cola y pila"
        );

        Console.WriteLine();

        Console.WriteLine(
            "[0] Salir"
        );

        Console.WriteLine(
            "=================================================="
        );
    }

    // =====================================================
    // FASE 1
    // =====================================================

    static void EjecutarFase1()
    {
        Console.WriteLine(
            "========================================"
        );

        Console.WriteLine(
            "FASE 1 - ORDENAMIENTO SHELLSORT"
        );

        Console.WriteLine(
            "========================================"
        );

        Producto[] catalogo =
            GenerarCatalogo(50);

        Console.WriteLine(
            $"Total de productos: {catalogo.Length}"
        );

        Console.WriteLine();

        Console.WriteLine(
            "PRIMEROS 5 PRODUCTOS ANTES DEL ORDENAMIENTO"
        );

        Console.WriteLine(
            "--------------------------------------------"
        );

        MostrarPrimerosCinco(catalogo);

        Stopwatch sw =
            Stopwatch.StartNew();

        OrdenamientoService.ShellSort(catalogo);

        sw.Stop();

        Console.WriteLine();

        Console.WriteLine(
            "PRIMEROS 5 PRODUCTOS DESPUÉS DEL SHELLSORT"
        );

        Console.WriteLine(
            "--------------------------------------------"
        );

        MostrarPrimerosCinco(catalogo);

        Console.WriteLine();

        Console.WriteLine(
            "RESULTADOS DE RENDIMIENTO"
        );

        Console.WriteLine(
            "--------------------------------------------"
        );

        Console.WriteLine(
            $"Tiempo: {sw.ElapsedMilliseconds} ms"
        );

        Console.WriteLine(
            $"Microsegundos: " +
            $"{sw.Elapsed.TotalMicroseconds:F2} us"
        );

        Console.WriteLine(
            $"Ticks: {sw.ElapsedTicks}"
        );

        Console.WriteLine(
            $"Total de productos procesados: " +
            $"{catalogo.Length}"
        );
    }

    // =====================================================
    // FASE 2
    // =====================================================

    static void CargarInventarioInicial()
    {
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

        foreach (Producto producto in productos)
        {
            inventario.InsertarOrdenado(producto);
        }
    }

    static void AgregarProducto()
    {
        Console.WriteLine(
            "=== AGREGAR PRODUCTO ==="
        );

        Console.Write(
            "SKU: "
        );

        int sku =
            int.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "Nombre: "
        );

        string nombre =
            Console.ReadLine() ?? "";

        Console.Write(
            "Precio: "
        );

        double precio =
            double.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "Stock: "
        );

        int stock =
            int.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "ID del proveedor: "
        );

        int idProveedor =
            int.Parse(Console.ReadLine() ?? "");

        Producto producto =
            CrearProducto(
                sku,
                nombre,
                precio,
                stock,
                idProveedor
            );

        inventario.InsertarOrdenado(producto);

        Console.WriteLine();

        Console.WriteLine(
            "Producto agregado correctamente."
        );
    }

    static void BuscarProducto()
    {
        Console.WriteLine(
            "=== BUSCAR PRODUCTO ==="
        );

        Console.Write(
            "Ingrese el SKU: "
        );

        int sku =
            int.Parse(Console.ReadLine() ?? "");

        Producto producto =
            inventario.BuscarPorSKU(sku);

        Console.WriteLine();

        Console.WriteLine(
            $"SKU: {producto.SKU}"
        );

        Console.WriteLine(
            $"Nombre: {producto.Nombre}"
        );

        Console.WriteLine(
            $"Precio: ${producto.Precio:F2}"
        );

        Console.WriteLine(
            $"Stock: {producto.Stock}"
        );

        Console.WriteLine(
            $"Proveedor: " +
            $"{producto.DatosProveedor.NombreCorporativo}"
        );
    }

    static void EliminarProducto()
    {
        Console.WriteLine(
            "=== ELIMINAR PRODUCTO ==="
        );

        Console.Write(
            "Ingrese el SKU: "
        );

        int sku =
            int.Parse(Console.ReadLine() ?? "");

        inventario.EliminarPorSKU(sku);

        Console.WriteLine();

        Console.WriteLine(
            $"Operación de eliminación finalizada para SKU {sku}."
        );
    }

    static void MostrarCatalogo()
    {
        Console.WriteLine(
            "=== CATÁLOGO ACTUAL ==="
        );

        Console.WriteLine();

        inventario.MostrarProductos();
    }

    // =====================================================
    // FASE 3
    // =====================================================

    static void MostrarBitacora()
    {
        Console.WriteLine(
            "=== BITÁCORA DE AUDITORÍA ==="
        );

        auditoria.ImprimirHistorialCronologico();

        Console.WriteLine();

        auditoria.ImprimirHistorialInverso();
    }

    static void ValidarBitacora()
    {
        Console.WriteLine(
            "=== VALIDACIÓN DE INTEGRIDAD ==="
        );

        Console.WriteLine();

        Console.WriteLine(
            $"Total de registros: " +
            $"{auditoria.TotalRegistros}"
        );

        Console.WriteLine(
            $"Integridad: " +
            $"{(auditoria.ValidarIntegridad()
                ? "CORRECTA"
                : "INCORRECTA")}"
        );
    }

    // =====================================================
    // FASE 4 - COLA FIFO
    // =====================================================

    static void EncolarPedido()
    {
        Console.WriteLine(
            "=== ENCOLAR PEDIDO ==="
        );

        Console.Write(
            "ID del pedido: "
        );

        int idPedido =
            int.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "SKU: "
        );

        int sku =
            int.Parse(Console.ReadLine() ?? "");

        // Validamos que el SKU exista
        // antes de registrar el pedido.
        inventario.BuscarPorSKU(sku);

        Console.Write(
            "Cantidad: "
        );

        int cantidad =
            int.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "Cliente: "
        );

        string cliente =
            Console.ReadLine() ?? "";

        Pedido pedido =
            new Pedido(
                idPedido,
                sku,
                cantidad,
                cliente
            );

        cola.EncolarPedido(pedido);
    }

    static void DespacharPedido()
    {
        Console.WriteLine(
            "=== DESPACHAR PEDIDO FIFO ==="
        );

        Pedido? pedido =
            cola.DespacharPedido(inventario);

        if (pedido != null)
        {
            Producto producto =
                inventario.BuscarPorSKU(
                    pedido.SKU
                );

            Console.WriteLine();

            Console.WriteLine(
                $"Stock restante del SKU " +
                $"{producto.SKU}: {producto.Stock}"
            );
        }
    }

    // =====================================================
    // FASE 4 - PILA LIFO
    // =====================================================

    static void RegistrarDevolucion()
    {
        Console.WriteLine(
            "=== REGISTRAR DEVOLUCIÓN ==="
        );

        Console.Write(
            "ID de devolución: "
        );

        int idDevolucion =
            int.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "SKU: "
        );

        int sku =
            int.Parse(Console.ReadLine() ?? "");

        // Se comprueba que el producto exista.
        inventario.BuscarPorSKU(sku);

        Console.Write(
            "Cantidad: "
        );

        int cantidad =
            int.Parse(Console.ReadLine() ?? "");

        Console.Write(
            "Motivo: "
        );

        string motivo =
            Console.ReadLine() ?? "";

        Devolucion devolucion =
            new Devolucion(
                idDevolucion,
                sku,
                cantidad,
                motivo
            );

        pila.PushDevolucion(
            devolucion
        );
    }

    static void ProcesarDevolucion()
    {
        Console.WriteLine(
            "=== PROCESAR DEVOLUCIÓN LIFO ==="
        );

        Devolucion? devolucion =
            pila.PopDevolucion(
                inventario
            );

        if (devolucion != null)
        {
            Producto producto =
                inventario.BuscarPorSKU(
                    devolucion.SKU
                );

            Console.WriteLine();

            Console.WriteLine(
                $"Stock actualizado del SKU " +
                $"{producto.SKU}: {producto.Stock}"
            );
        }
    }

    static void MostrarEstadoLogistico()
    {
        Console.WriteLine(
            "=== ESTADO DEL MOTOR LOGÍSTICO ==="
        );

        Console.WriteLine();

        cola.MostrarCola();

        Console.WriteLine();

        pila.MostrarPila();

        Console.WriteLine();

        Console.WriteLine(
            $"Pedidos pendientes: " +
            $"{cola.TotalEncolados}"
        );

        Console.WriteLine(
            $"Devoluciones pendientes: " +
            $"{pila.TotalDevoluciones}"
        );
    }

    // =====================================================
    // MÉTODOS AUXILIARES
    // =====================================================

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

            DatosProveedor =
                new Proveedor
                {
                    IdProveedor =
                        idProveedor,

                    NombreCorporativo =
                        $"Proveedor {idProveedor}"
                }
        };
    }

    static Producto[] GenerarCatalogo(
        int cantidad)
    {
        Producto[] productos =
            new Producto[cantidad];

        Random random =
            new Random(42);

        for (int i = 0; i < cantidad; i++)
        {
            productos[i] =
                new Producto
                {
                    SKU =
                        1001 + i,

                    Nombre =
                        $"Producto {i + 1}",

                    Precio =
                        Math.Round(
                            10 +
                            random.NextDouble() *
                            (9999.99 - 10),
                            2
                        ),

                    Stock =
                        random.Next(0, 501),

                    DatosProveedor =
                        new Proveedor
                        {
                            IdProveedor =
                                (i % 5) + 1,

                            NombreCorporativo =
                                $"Proveedor {(i % 5) + 1}"
                        }
                };
        }

        productos[5].Precio =
            2500.00;

        productos[15].Precio =
            2500.00;

        productos[25].Precio =
            2500.00;

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
            Math.Min(
                5,
                catalogo.Length
            );

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