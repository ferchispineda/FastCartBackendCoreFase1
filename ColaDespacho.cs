using System;

public class ColaDespacho
{
    private NodoCola? Frente;
    private NodoCola? Fin;

    public int TotalEncolados { get; private set; }

    public ColaDespacho()
    {
        Frente = null;
        Fin = null;
        TotalEncolados = 0;
    }

    /// <summary>
    /// Indica si la cola se encuentra vacía.
    /// </summary>
    public bool EstaVacia()
    {
        return Frente == null;
    }

    /// <summary>
    /// Inserta un pedido al final de la cola.
    /// Complejidad O(1).
    /// </summary>
    public void EncolarPedido(Pedido nuevoPedido)
    {
        ArgumentNullException.ThrowIfNull(nuevoPedido);

        if (nuevoPedido.Cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad del pedido debe ser mayor que cero."
            );
        }

        NodoCola nuevoNodo = new NodoCola(nuevoPedido);

        if (EstaVacia())
        {
            Frente = nuevoNodo;
            Fin = nuevoNodo;
        }
        else
        {
            Fin!.Siguiente = nuevoNodo;
            Fin = nuevoNodo;
        }

        TotalEncolados++;

        Console.WriteLine(
            $"[COLA] Pedido #{nuevoPedido.IdPedido} " +
            $"encolado correctamente. " +
            $"SKU: {nuevoPedido.SKU} | " +
            $"Cantidad: {nuevoPedido.Cantidad}"
        );
    }

    /// <summary>
    /// Extrae el pedido del frente de la cola
    /// y actualiza el stock real del inventario.
    /// </summary>
    public Pedido? DespacharPedido(
        InventarioLista inventario)
    {
        ArgumentNullException.ThrowIfNull(inventario);

        if (EstaVacia())
        {
            Console.WriteLine(
                "[COLA] No hay pedidos pendientes por despachar."
            );

            return null;
        }

        Pedido pedidoDespachado = Frente!.Dato;

        // Primero se intenta modificar el inventario.
        // Si no existe el SKU o no hay stock suficiente,
        // el pedido permanece en la cola.
        inventario.DisminuirStock(
            pedidoDespachado.SKU,
            pedidoDespachado.Cantidad
        );

        // El inventario se actualizó correctamente,
        // por lo tanto ahora sí retiramos el nodo.
        Frente = Frente.Siguiente;

        if (Frente == null)
        {
            Fin = null;
        }

        TotalEncolados--;

        Console.WriteLine(
            $"[COLA] Pedido #{pedidoDespachado.IdPedido} " +
            $"despachado correctamente para " +
            $"{pedidoDespachado.Cliente}."
        );

        return pedidoDespachado;
    }

    /// <summary>
    /// Muestra los pedidos actualmente
    /// almacenados en la cola.
    /// </summary>
    public void MostrarCola()
    {
        if (EstaVacia())
        {
            Console.WriteLine(
                "[COLA VACÍA]"
            );

            return;
        }

        Console.WriteLine();
        Console.WriteLine(
            "=== COLA DE DESPACHO FIFO ==="
        );

        NodoCola? actual = Frente;
        int posicion = 1;

        while (actual != null)
        {
            Console.WriteLine(
                $"[{posicion}] " +
                $"Pedido #{actual.Dato.IdPedido} | " +
                $"SKU: {actual.Dato.SKU} | " +
                $"Cantidad: {actual.Dato.Cantidad} | " +
                $"Cliente: {actual.Dato.Cliente}"
            );

            actual = actual.Siguiente;
            posicion++;
        }

        Console.WriteLine(
            $"Total en cola: {TotalEncolados}"
        );
    }
}