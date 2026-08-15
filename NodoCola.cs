public class NodoCola
{
    public Pedido Dato { get; set; }
    public NodoCola? Siguiente { get; set; }

    public NodoCola(Pedido pedido)
    {
        Dato = pedido;
        Siguiente = null;
    }
}