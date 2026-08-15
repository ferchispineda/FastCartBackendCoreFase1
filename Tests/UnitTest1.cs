using System;
using System.IO;
using Xunit;
using FastCartBackendCore;

namespace FastCartBackendCore.Tests;

public class AuditoriaServiceTests
{
    [Fact]
    public void RegistrarEvento_ListaVacia_TotalRegistrosEsUno()
    {
        AuditoriaService auditoria = new AuditoriaService();

        auditoria.RegistrarEvento(
            "INSERCION",
            1001,
            "Producto agregado."
        );

        Assert.Equal(1, auditoria.TotalRegistros);
        Assert.True(auditoria.ValidarIntegridad());
    }

    [Fact]
    public void RegistrarEvento_MultiplesEventos_TotalCorrecto()
    {
        AuditoriaService auditoria = new AuditoriaService();

        auditoria.RegistrarEvento(
            "INSERCION",
            1001,
            "Producto 1 agregado."
        );

        auditoria.RegistrarEvento(
            "ACTUALIZACION",
            1001,
            "Producto 1 actualizado."
        );

        auditoria.RegistrarEvento(
            "ELIMINACION",
            1001,
            "Producto 1 eliminado."
        );

        Assert.Equal(3, auditoria.TotalRegistros);
        Assert.True(auditoria.ValidarIntegridad());
    }

    [Fact]
    public void ImprimirHistorialCronologico_MuestraEventosEnOrden()
    {
        AuditoriaService auditoria = new AuditoriaService();

        auditoria.RegistrarEvento(
            "INSERCION",
            1001,
            "Primer evento"
        );

        auditoria.RegistrarEvento(
            "ACTUALIZACION",
            1002,
            "Segundo evento"
        );

        StringWriter salida = new StringWriter();
        TextWriter salidaOriginal = Console.Out;

        try
        {
            Console.SetOut(salida);

            auditoria.ImprimirHistorialCronologico();
        }
        finally
        {
            Console.SetOut(salidaOriginal);
        }

        string resultado = salida.ToString();

        int posicionPrimero =
            resultado.IndexOf("Primer evento");

        int posicionSegundo =
            resultado.IndexOf("Segundo evento");

        Assert.True(posicionPrimero >= 0);
        Assert.True(posicionSegundo >= 0);
        Assert.True(posicionPrimero < posicionSegundo);
    }

    [Fact]
    public void ImprimirHistorialInverso_MuestraEventosEnOrdenInverso()
    {
        AuditoriaService auditoria = new AuditoriaService();

        auditoria.RegistrarEvento(
            "INSERCION",
            1001,
            "Primer evento"
        );

        auditoria.RegistrarEvento(
            "ACTUALIZACION",
            1002,
            "Segundo evento"
        );

        StringWriter salida = new StringWriter();
        TextWriter salidaOriginal = Console.Out;

        try
        {
            Console.SetOut(salida);

            auditoria.ImprimirHistorialInverso();
        }
        finally
        {
            Console.SetOut(salidaOriginal);
        }

        string resultado = salida.ToString();

        int posicionPrimero =
            resultado.IndexOf("Primer evento");

        int posicionSegundo =
            resultado.IndexOf("Segundo evento");

        Assert.True(posicionPrimero >= 0);
        Assert.True(posicionSegundo >= 0);
        Assert.True(posicionSegundo < posicionPrimero);
    }

    [Fact]
    public void InventarioLista_AuditoriaNula_LanzaArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(
            () => new InventarioLista(null!)
        );
    }

    [Fact]
    public void ValidarIntegridad_ListaConVariosEventos_RetornaTrue()
    {
        AuditoriaService auditoria = new AuditoriaService();

        for (int i = 1; i <= 10; i++)
        {
            auditoria.RegistrarEvento(
                "INSERCION",
                2000 + i,
                $"Evento {i}"
            );
        }

        Assert.Equal(10, auditoria.TotalRegistros);
        Assert.True(auditoria.ValidarIntegridad());
    }
}

public class Fase4Tests
{
    [Fact]
    public void EncolarPedido_AgregaPedido_TotalEsUno()
    {
        ColaDespacho cola = new ColaDespacho();

        Pedido pedido = new Pedido(
            100,
            2001,
            2,
            "Cliente Prueba"
        );

        cola.EncolarPedido(pedido);

        Assert.Equal(1, cola.TotalEncolados);
        Assert.False(cola.EstaVacia());
    }

    [Fact]
    public void DespacharPedido_DisminuyeStockDelInventario()
    {
        AuditoriaService auditoria =
            new AuditoriaService();

        InventarioLista inventario =
            new InventarioLista(auditoria);

        Producto producto = CrearProducto(
            2001,
            "Laptop",
            18500.00,
            12
        );

        inventario.InsertarOrdenado(producto);

        ColaDespacho cola =
            new ColaDespacho();

        Pedido pedido =
            new Pedido(
                100,
                2001,
                2,
                "Cliente Prueba"
            );

        cola.EncolarPedido(pedido);

        Pedido? despachado =
            cola.DespacharPedido(inventario);

        Producto actualizado =
            inventario.BuscarPorSKU(2001);

        Assert.NotNull(despachado);
        Assert.Equal(10, actualizado.Stock);
        Assert.Equal(0, cola.TotalEncolados);
        Assert.True(cola.EstaVacia());
    }

    [Fact]
    public void ColaDespacho_RespetaOrdenFIFO()
    {
        AuditoriaService auditoria =
            new AuditoriaService();

        InventarioLista inventario =
            new InventarioLista(auditoria);

        inventario.InsertarOrdenado(
            CrearProducto(
                2001,
                "Laptop",
                18500.00,
                20
            )
        );

        ColaDespacho cola =
            new ColaDespacho();

        cola.EncolarPedido(
            new Pedido(
                1,
                2001,
                1,
                "Cliente A"
            )
        );

        cola.EncolarPedido(
            new Pedido(
                2,
                2001,
                1,
                "Cliente B"
            )
        );

        Pedido? primero =
            cola.DespacharPedido(inventario);

        Pedido? segundo =
            cola.DespacharPedido(inventario);

        Assert.NotNull(primero);
        Assert.NotNull(segundo);

        Assert.Equal(
            1,
            primero!.IdPedido
        );

        Assert.Equal(
            2,
            segundo!.IdPedido
        );
    }

    [Fact]
    public void PushDevolucion_AgregaDevolucion_TotalEsUno()
    {
        PilaDevoluciones pila =
            new PilaDevoluciones();

        Devolucion devolucion =
            new Devolucion(
                500,
                2001,
                2,
                "Producto devuelto"
            );

        pila.PushDevolucion(devolucion);

        Assert.Equal(
            1,
            pila.TotalDevoluciones
        );

        Assert.False(
            pila.EstaVacia()
        );
    }

    [Fact]
    public void PopDevolucion_IncrementaStockDelInventario()
    {
        AuditoriaService auditoria =
            new AuditoriaService();

        InventarioLista inventario =
            new InventarioLista(auditoria);

        inventario.InsertarOrdenado(
            CrearProducto(
                2001,
                "Laptop",
                18500.00,
                10
            )
        );

        PilaDevoluciones pila =
            new PilaDevoluciones();

        pila.PushDevolucion(
            new Devolucion(
                500,
                2001,
                2,
                "Producto devuelto"
            )
        );

        Devolucion? procesada =
            pila.PopDevolucion(inventario);

        Producto actualizado =
            inventario.BuscarPorSKU(2001);

        Assert.NotNull(procesada);
        Assert.Equal(12, actualizado.Stock);
        Assert.Equal(0, pila.TotalDevoluciones);
        Assert.True(pila.EstaVacia());
    }

    [Fact]
    public void PilaDevoluciones_RespetaOrdenLIFO()
    {
        AuditoriaService auditoria =
            new AuditoriaService();

        InventarioLista inventario =
            new InventarioLista(auditoria);

        inventario.InsertarOrdenado(
            CrearProducto(
                2001,
                "Laptop",
                18500.00,
                10
            )
        );

        PilaDevoluciones pila =
            new PilaDevoluciones();

        pila.PushDevolucion(
            new Devolucion(
                1,
                2001,
                1,
                "Primera devolución"
            )
        );

        pila.PushDevolucion(
            new Devolucion(
                2,
                2001,
                1,
                "Segunda devolución"
            )
        );

        Devolucion? primeraProcesada =
            pila.PopDevolucion(inventario);

        Devolucion? segundaProcesada =
            pila.PopDevolucion(inventario);

        Assert.NotNull(primeraProcesada);
        Assert.NotNull(segundaProcesada);

        Assert.Equal(
            2,
            primeraProcesada!.IdDevolucion
        );

        Assert.Equal(
            1,
            segundaProcesada!.IdDevolucion
        );
    }

    [Fact]
    public void DespacharPedido_StockInsuficiente_LanzaExcepcion()
    {
        AuditoriaService auditoria =
            new AuditoriaService();

        InventarioLista inventario =
            new InventarioLista(auditoria);

        inventario.InsertarOrdenado(
            CrearProducto(
                2001,
                "Laptop",
                18500.00,
                1
            )
        );

        ColaDespacho cola =
            new ColaDespacho();

        cola.EncolarPedido(
            new Pedido(
                100,
                2001,
                5,
                "Cliente Prueba"
            )
        );

        Assert.Throws<InvalidOperationException>(
            () => cola.DespacharPedido(inventario)
        );

        Producto producto =
            inventario.BuscarPorSKU(2001);

        Assert.Equal(
            1,
            producto.Stock
        );

        Assert.Equal(
            1,
            cola.TotalEncolados
        );
    }

    private static Producto CrearProducto(
        int sku,
        string nombre,
        double precio,
        int stock)
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
                    IdProveedor = 1,

                    NombreCorporativo =
                        "Proveedor Prueba"
                }
        };
    }
}