using System;
using System.Collections.Generic;

public class InventarioLista
{
    private NodoProducto? _cabeza;

    /// <summary>
    /// Inicializa una nueva lista de inventario vacía.
    /// </summary>
    public InventarioLista()
    {
        _cabeza = null;
    }

    /// <summary>
    /// Inserta un producto al inicio de la lista.
    /// </summary>
    /// <param name="producto">Producto que se agregará al inventario.</param>
    public void InsertarInicio(Producto producto)
    {
        NodoProducto nuevo = new NodoProducto(producto);

        nuevo.Siguiente = _cabeza;
        _cabeza = nuevo;
    }

    /// <summary>
    /// Inserta un producto manteniendo la lista ordenada
    /// de forma ascendente por precio.
    /// </summary>
    /// <param name="producto">Producto que se agregará al inventario.</param>
    public void InsertarOrdenado(Producto producto)
    {
        NodoProducto nuevo = new NodoProducto(producto);

        if (_cabeza == null || producto.Precio < _cabeza.Data.Precio)
        {
            nuevo.Siguiente = _cabeza;
            _cabeza = nuevo;
            return;
        }

        NodoProducto actual = _cabeza;

        while (actual.Siguiente != null &&
               actual.Siguiente.Data.Precio <= producto.Precio)
        {
            actual = actual.Siguiente;
        }

        nuevo.Siguiente = actual.Siguiente;
        actual.Siguiente = nuevo;
    }

    /// <summary>
    /// Busca un producto mediante su SKU.
    /// </summary>
    /// <param name="sku">SKU del producto que se desea localizar.</param>
    /// <returns>Producto correspondiente al SKU indicado.</returns>
    /// <exception cref="KeyNotFoundException">
    /// Se genera cuando el SKU no existe en el inventario.
    /// </exception>
    public Producto BuscarPorSKU(int sku)
    {
        NodoProducto? actual = _cabeza;

        while (actual != null)
        {
            if (actual.Data.SKU == sku)
            {
                return actual.Data;
            }

            actual = actual.Siguiente;
        }

        throw new KeyNotFoundException(
            $"SKU {sku} no encontrado.");
    }

    /// <summary>
    /// Elimina un producto del inventario mediante su SKU.
    /// </summary>
    /// <param name="sku">SKU del producto que se desea eliminar.</param>
    public void EliminarPorSKU(int sku)
    {
        if (_cabeza == null)
        {
            return;
        }

        if (_cabeza.Data.SKU == sku)
        {
            _cabeza = _cabeza.Siguiente;
            return;
        }

        NodoProducto anterior = _cabeza;

        while (anterior.Siguiente != null)
        {
            if (anterior.Siguiente.Data.SKU == sku)
            {
                anterior.Siguiente = anterior.Siguiente.Siguiente;
                return;
            }

            anterior = anterior.Siguiente;
        }
    }

    /// <summary>
    /// Muestra todos los productos almacenados en la lista.
    /// </summary>
    public void MostrarProductos()
    {
        NodoProducto? actual = _cabeza;

        while (actual != null)
        {
            Console.WriteLine(
                $"SKU: {actual.Data.SKU} | " +
                $"Nombre: {actual.Data.Nombre} | " +
                $"Precio: ${actual.Data.Precio:F2} | " +
                $"Stock: {actual.Data.Stock}");

            actual = actual.Siguiente;
        }
    }
}