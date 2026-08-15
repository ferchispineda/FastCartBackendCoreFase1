namespace FastCartBackendCore
{
    public class NodoAuditoria
    {
        public LogMovimiento Dato;
        public NodoAuditoria? Siguiente;
        public NodoAuditoria? Anterior;

        public NodoAuditoria(LogMovimiento dato)
        {
            Dato = dato;
            Siguiente = null;
            Anterior = null;
        }
    }
}