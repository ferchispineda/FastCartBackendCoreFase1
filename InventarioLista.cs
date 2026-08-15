using System;
using System.Collections.Generic;
using FastCartBackendCore;

public class InventarioLista
{
    private NodoProducto? _cabeza;
    private readonly AuditoriaService _auditoria;

    /// <summary>
    /// Inicializa una nueva lista de inventario vacía
    /// y recibe el servicio de auditoría.
    /// </summary>
    /// <param name="auditoria">Servicio encargado de registrar los movimientos.</param>
    /// <exception cref="ArgumentNullException">
    /// Se genera cuando el servicio de auditoría es nulo.
    /// </exception>
    public InventarioLista(AuditoriaService auditoria)
    {
        ArgumentNullException.ThrowIfNull(auditoria);

        _cabeza = null;
        _auditoria = auditoria;
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

        _auditoria.RegistrarEvento(
            "INSERCION",
            producto.SKU,
            $"Producto '{producto.Nombre}' agregado al inicio del inventario."
        );
    }

    /// <summary>
    /// Inserta un producto manteniendo la lista ordenada
    /// de forma ascendente por precio.
    /// </summary>
    /// <param name="producto">Producto que se agregará al inventario.</param>
    public void InsertarOrdenado(Producto producto)
    {
        NodoProducto nuevo = new NodoProducto(producto);

        if (_cabeza == null ||
            producto.Precio < _cabeza.Data.Precio)
        {
            nuevo.Siguiente = _cabeza;
            _cabeza = nuevo;

            _auditoria.RegistrarEvento(
                "INSERCION",
                producto.SKU,
                $"Producto '{producto.Nombre}' agregado al inventario."
            );

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

        _auditoria.RegistrarEvento(
            "INSERCION",
            producto.SKU,
            $"Producto '{producto.Nombre}' agregado al inventario."
        );
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
            $"SKU {sku} no encontrado."
        );
    }

    /// <summary>
    /// Actualiza el precio de un producto mediante su SKU.
    /// </summary>
    /// <param name="sku">SKU del producto que se actualizará.</param>
    /// <param name="nuevoPrecio">Nuevo precio del producto.</param>
    /// <exception cref="KeyNotFoundException">
    /// Se genera cuando el SKU no existe en el inventario.
    /// </exception>
    public void ActualizarPrecio(int sku, double nuevoPrecio)
    {
        NodoProducto? actual = _cabeza;

        while (actual != null)
        {
            if (actual.Data.SKU == sku)
            {
                Producto producto = actual.Data;

                double precioAnterior = producto.Precio;

                producto.Precio = nuevoPrecio;

                actual.Data = producto;

                _auditoria.RegistrarEvento(
                    "ACTUALIZACION",
                    sku,
                    $"Precio de '{producto.Nombre}' actualizado de " +
                    $"${precioAnterior:F2} a ${nuevoPrecio:F2}."
                );

                return;
            }

            actual = actual.Siguiente;
        }

        throw new KeyNotFoundException(
            $"SKU {sku} no encontrado."
        );
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
            string nombre = _cabeza.Data.Nombre;

            _cabeza = _cabeza.Siguiente;

            _auditoria.RegistrarEvento(
                "ELIMINACION",
                sku,
                $"Producto '{nombre}' eliminado del inventario."
            );

            return;
        }

        NodoProducto anterior = _cabeza;

        while (anterior.Siguiente != null)
        {
            if (anterior.Siguiente.Data.SKU == sku)
            {
                string nombre =
                    anterior.Siguiente.Data.Nombre;

                anterior.Siguiente =
                    anterior.Siguiente.Siguiente;

                _auditoria.RegistrarEvento(
                    "ELIMINACION",
                    sku,
                    $"Producto '{nombre}' eliminado del inventario."
                );

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
                $"Stock: {actual.Data.Stock}"
            );

            actual = actual.Siguiente;
        }
    }
}