public class NodoPila
{
    public Devolucion Dato { get; set; }
    public NodoPila? Siguiente { get; set; }

    public NodoPila(Devolucion devolucion)
    {
        Dato = devolucion;
        Siguiente = null;
    }
}