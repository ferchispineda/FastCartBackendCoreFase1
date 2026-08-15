using System;

public class PilaDevoluciones
{
    private NodoPila? Top;

    public int TotalDevoluciones { get; private set; }

    public PilaDevoluciones()
    {
        Top = null;
        TotalDevoluciones = 0;
    }

    /// <summary>
    /// Indica si la pila se encuentra vacía.
    /// </summary>
    public bool EstaVacia()
    {
        return Top == null;
    }

    /// <summary>
    /// Inserta una devolución en la cima de la pila.
    /// Complejidad O(1).
    /// </summary>
    public void PushDevolucion(
        Devolucion nuevaDevolucion)
    {
        ArgumentNullException.ThrowIfNull(
            nuevaDevolucion
        );

        if (nuevaDevolucion.Cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad de la devolución debe ser mayor que cero."
            );
        }

        NodoPila nuevoNodo =
            new NodoPila(nuevaDevolucion);

        nuevoNodo.Siguiente = Top;
        Top = nuevoNodo;

        TotalDevoluciones++;

        Console.WriteLine(
            $"[PILA] Devolución #{nuevaDevolucion.IdDevolucion} " +
            $"registrada correctamente. " +
            $"SKU: {nuevaDevolucion.SKU} | " +
            $"Cantidad: {nuevaDevolucion.Cantidad}"
        );
    }

    /// <summary>
    /// Extrae la devolución más reciente
    /// y reintegra el stock al inventario.
    /// </summary>
    public Devolucion? PopDevolucion(
        InventarioLista inventario)
    {
        ArgumentNullException.ThrowIfNull(
            inventario
        );

        if (EstaVacia())
        {
            Console.WriteLine(
                "[PILA] No hay devoluciones pendientes por procesar."
            );

            return null;
        }

        Devolucion devolucionProcesada =
            Top!.Dato;

        // Primero se actualiza el inventario.
        // Si ocurre un error, la devolución
        // permanece en la pila.
        inventario.IncrementarStock(
            devolucionProcesada.SKU,
            devolucionProcesada.Cantidad
        );

        // El inventario se actualizó correctamente,
        // por lo tanto se retira el nodo de la pila.
        Top = Top.Siguiente;

        TotalDevoluciones--;

        Console.WriteLine(
            $"[PILA] Devolución " +
            $"#{devolucionProcesada.IdDevolucion} " +
            $"procesada correctamente. " +
            $"Se reintegraron " +
            $"{devolucionProcesada.Cantidad} unidades."
        );

        return devolucionProcesada;
    }

    /// <summary>
    /// Muestra las devoluciones desde la más reciente
    /// hasta la más antigua.
    /// </summary>
    public void MostrarPila()
    {
        if (EstaVacia())
        {
            Console.WriteLine(
                "[PILA VACÍA]"
            );

            return;
        }

        Console.WriteLine();
        Console.WriteLine(
            "=== PILA DE DEVOLUCIONES LIFO ==="
        );

        NodoPila? actual = Top;
        int posicion = 1;

        while (actual != null)
        {
            Console.WriteLine(
                $"[{posicion}] " +
                $"Devolución #{actual.Dato.IdDevolucion} | " +
                $"SKU: {actual.Dato.SKU} | " +
                $"Cantidad: {actual.Dato.Cantidad} | " +
                $"Motivo: {actual.Dato.Motivo}"
            );

            actual = actual.Siguiente;
            posicion++;
        }

        Console.WriteLine(
            $"Total en pila: {TotalDevoluciones}"
        );
    }
}