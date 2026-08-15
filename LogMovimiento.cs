using System;

namespace FastCartBackendCore
{
    public struct LogMovimiento
    {
        public DateTime Timestamp;
        public string TipoOperacion;
        public int SKUAfectado;
        public string Descripcion;
    }
}