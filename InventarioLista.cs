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
    /// <param name="auditoria">
    /// Servicio encargado de registrar los movimientos.
    /// </param>
    public InventarioLista(AuditoriaService auditoria)
    {
        ArgumentNullException.ThrowIfNull(auditoria);

        _cabeza = null;
        _auditoria = auditoria;
    }

    /// <summary>
    /// Inserta un producto al inicio de la lista.
    /// </summary>
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
    /// Inserta un producto manteniendo la lista
    /// ordenada de forma ascendente por precio.
    /// </summary>
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
    public void ActualizarPrecio(
        int sku,
        double nuevoPrecio)
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
    /// Disminuye el stock real de un producto.
    /// Este método será utilizado por la cola
    /// de despacho de la Fase 4.
    /// </summary>
    public void DisminuirStock(
        int sku,
        int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad debe ser mayor que cero."
            );
        }

        NodoProducto? actual = _cabeza;

        while (actual != null)
        {
            if (actual.Data.SKU == sku)
            {
                Producto producto = actual.Data;

                if (producto.Stock < cantidad)
                {
                    throw new InvalidOperationException(
                        $"Stock insuficiente para SKU {sku}. " +
                        $"Disponible: {producto.Stock}, " +
                        $"solicitado: {cantidad}."
                    );
                }

                producto.Stock -= cantidad;

                // Se guarda nuevamente el struct
                // dentro del nodo.
                actual.Data = producto;

                _auditoria.RegistrarEvento(
                    "DESPACHO",
                    sku,
                    $"Se descontaron {cantidad} unidades de " +
                    $"'{producto.Nombre}'. " +
                    $"Stock actual: {producto.Stock}."
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
    /// Incrementa el stock real de un producto.
    /// Este método será utilizado por la pila
    /// de devoluciones de la Fase 4.
    /// </summary>
    public void IncrementarStock(
        int sku,
        int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException(
                "La cantidad debe ser mayor que cero."
            );
        }

        NodoProducto? actual = _cabeza;

        while (actual != null)
        {
            if (actual.Data.SKU == sku)
            {
                Producto producto = actual.Data;

                producto.Stock += cantidad;

                // Se guarda nuevamente el struct
                // dentro del nodo.
                actual.Data = producto;

                _auditoria.RegistrarEvento(
                    "DEVOLUCION",
                    sku,
                    $"Se reintegraron {cantidad} unidades de " +
                    $"'{producto.Nombre}'. " +
                    $"Stock actual: {producto.Stock}."
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
    /// Muestra todos los productos almacenados
    /// en la lista.
    /// </summary>
    public void MostrarProductos()
    {
        NodoProducto? actual = _cabeza;

        if (actual == null)
        {
            Console.WriteLine(
                "[Inventario vacío]"
            );

            return;
        }

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