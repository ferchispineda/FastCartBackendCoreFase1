using System;

namespace FastCartBackendCore
{
    /// <summary>
    /// Administra la bitácora de auditoría mediante
    /// una lista doblemente enlazada.
    /// </summary>
    public class AuditoriaService
    {
        private NodoAuditoria? Cabeza;
        private NodoAuditoria? Cola;

        /// <summary>
        /// Obtiene la cantidad total de registros
        /// almacenados en la bitácora.
        /// </summary>
        public int TotalRegistros { get; private set; }

        /// <summary>
        /// Inicializa una nueva bitácora de auditoría vacía.
        /// </summary>
        public AuditoriaService()
        {
            Cabeza = null;
            Cola = null;
            TotalRegistros = 0;
        }

        /// <summary>
        /// Registra un nuevo evento al final de la bitácora.
        /// </summary>
        /// <param name="tipo">Tipo de operación realizada.</param>
        /// <param name="sku">SKU del producto afectado.</param>
        /// <param name="desc">Descripción del movimiento.</param>
        public void RegistrarEvento(
            string tipo,
            int sku,
            string desc)
        {
            LogMovimiento log = new LogMovimiento
            {
                Timestamp = DateTime.UtcNow,
                TipoOperacion = tipo,
                SKUAfectado = sku,
                Descripcion = desc
            };

            NodoAuditoria nuevoNodo =
                new NodoAuditoria(log);

            if (Cola == null)
            {
                Cabeza = nuevoNodo;
                Cola = nuevoNodo;
                TotalRegistros++;
                return;
            }

            Cola.Siguiente = nuevoNodo;
            nuevoNodo.Anterior = Cola;
            Cola = nuevoNodo;

            TotalRegistros++;
        }

        /// <summary>
        /// Imprime el historial desde el evento más antiguo
        /// hasta el evento más reciente.
        /// </summary>
        public void ImprimirHistorialCronologico()
        {
            if (Cabeza == null)
            {
                Console.WriteLine(
                    "[Bitácora vacía - no se han registrado eventos]"
                );

                return;
            }

            Console.WriteLine();
            Console.WriteLine(
                "=== HISTORIAL CRONOLÓGICO (Antiguo -> Reciente) ==="
            );

            NodoAuditoria? actual = Cabeza;
            int contador = 1;

            while (actual != null)
            {
                Console.WriteLine(
                    $"[{contador}] " +
                    $"{actual.Dato.Timestamp:yyyy-MM-dd HH:mm:ss} UTC"
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

            Console.WriteLine(
                $"Total de eventos: {TotalRegistros}"
            );
        }

        /// <summary>
        /// Imprime el historial desde el evento más reciente
        /// hasta el evento más antiguo.
        /// </summary>
        public void ImprimirHistorialInverso()
        {
            if (Cola == null)
            {
                Console.WriteLine(
                    "[Bitácora vacía - no se han registrado eventos]"
                );

                return;
            }

            if (!ValidarIntegridad())
            {
                throw new InvalidOperationException(
                    "La lista presenta inconsistencias " +
                    "en sus enlaces bidireccionales."
                );
            }

            Console.WriteLine();
            Console.WriteLine(
                "=== HISTORIAL INVERSO (Reciente -> Antiguo) ==="
            );

            NodoAuditoria? actual = Cola;
            int contador = 1;

            while (actual != null)
            {
                Console.WriteLine(
                    $"[{contador}] " +
                    $"{actual.Dato.Timestamp:yyyy-MM-dd HH:mm:ss} UTC"
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

            Console.WriteLine(
                $"Total de eventos: {TotalRegistros}"
            );
        }

        /// <summary>
        /// Verifica que los enlaces de la lista sean consistentes
        /// tanto hacia adelante como hacia atrás.
        /// </summary>
        /// <returns>
        /// True si la estructura es válida; de lo contrario, false.
        /// </returns>
        public bool ValidarIntegridad()
        {
            if (TotalRegistros == 0)
            {
                return Cabeza == null && Cola == null;
            }

            if (Cabeza == null || Cola == null)
            {
                return false;
            }

            if (Cabeza.Anterior != null)
            {
                return false;
            }

            if (Cola.Siguiente != null)
            {
                return false;
            }

            int conteoAdelante = 0;
            int conteoAtras = 0;

            NodoAuditoria? actual = Cabeza;
            NodoAuditoria? anterior = null;

            while (actual != null)
            {
                if (actual.Anterior != anterior)
                {
                    return false;
                }

                conteoAdelante++;
                anterior = actual;
                actual = actual.Siguiente;
            }

            actual = Cola;
            NodoAuditoria? siguiente = null;

            while (actual != null)
            {
                if (actual.Siguiente != siguiente)
                {
                    return false;
                }

                conteoAtras++;
                siguiente = actual;
                actual = actual.Anterior;
            }

            return
                conteoAdelante == conteoAtras &&
                conteoAdelante == TotalRegistros;
        }
    }
}