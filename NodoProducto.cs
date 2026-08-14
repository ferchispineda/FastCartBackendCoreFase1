public class NodoProducto
{
    public Producto Data { get; set; }
    public NodoProducto? Siguiente { get; set; }

    /// <summary>
    /// Inicializa un nuevo nodo con un producto.
    /// </summary>
    /// <param name="producto">Producto almacenado en el nodo.</param>
    public NodoProducto(Producto producto)
    {
        Data = producto;
        Siguiente = null;
    }
}