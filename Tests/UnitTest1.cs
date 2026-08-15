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