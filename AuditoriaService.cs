using System;

namespace FastCartBackendCore
{
    public class AuditoriaService
    {
        private NodoAuditoria? Cabeza;
        private NodoAuditoria? Cola;

        public AuditoriaService()
        {
            Cabeza = null;
            Cola = null;
        }

        public void RegistrarEvento(string tipo, int sku, string desc)
        {
            LogMovimiento log = new LogMovimiento
            {
                Timestamp = DateTime.UtcNow,
                TipoOperacion = tipo,
                SKUAfectado = sku,
                Descripcion = desc
            };

            NodoAuditoria nuevoNodo = new NodoAuditoria(log);

            if (Cola == null)
            {
                Cabeza = nuevoNodo;
                Cola = nuevoNodo;
                return;
            }

            Cola.Siguiente = nuevoNodo;
            nuevoNodo.Anterior = Cola;
            Cola = nuevoNodo;
        }

        public void ImprimirHistorialCronologico()
        {
            if (Cabeza == null)
            {
                Console.WriteLine("[Bitácora vacía - no se han registrado eventos]");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== HISTORIAL CRONOLÓGICO (Antiguo -> Reciente) ===");

            NodoAuditoria? actual = Cabeza;
            int contador = 1;

            while (actual != null)
            {
                Console.WriteLine(
                    $"[{contador}] {actual.Dato.Timestamp:yyyy-MM-dd HH:mm:ss} UTC"
                );

                Console.WriteLine(
                    $"Operación : {actual.Dato.TipoOperacion}"
                );

                Console.WriteLine(
                    $"SKU       : {actual.Dato.SKUAfectado}"
                );

                Console.WriteLine(
                    $"Detalle   : {actual.Dato.Descripcion}"
                );

                Console.WriteLine();

                actual = actual.Siguiente;
                contador++;
            }

            Console.WriteLine($"Total de eventos: {contador - 1}");
        }

        public void ImprimirHistorialInverso()
        {
            if (Cola == null)
            {
                Console.WriteLine("[Bitácora vacía - no se han registrado eventos]");
                return;
            }

            Console.WriteLine();
            Console.WriteLine("=== HISTORIAL INVERSO (Reciente -> Antiguo) ===");

            NodoAuditoria? actual = Cola;
            int contador = 1;

            while (actual != null)
            {
                Console.WriteLine(
                    $"[{contador}] {actual.Dato.Timestamp:yyyy-MM-dd HH:mm:ss} UTC"
                );

                Console.WriteLine(
                    $"Operación : {actual.Dato.TipoOperacion}"
                );

                Console.WriteLine(
                    $"SKU       : {actual.Dato.SKUAfectado}"
                );

                Console.WriteLine(
                    $"Detalle   : {actual.Dato.Descripcion}"
                );

                Console.WriteLine();

                actual = actual.Anterior;
                contador++;
            }

            Console.WriteLine($"Total de eventos: {contador - 1}");
        }
    }
}