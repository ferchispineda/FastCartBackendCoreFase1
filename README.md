# FastCart Backend Core

Proyecto desarrollado en C# para implementar y evolucionar el núcleo de procesamiento del catálogo de productos de FastCart.

El proyecto está dividido en fases que permiten aplicar diferentes estructuras de datos y analizar su comportamiento en escenarios de inventario, auditoría, despacho y devoluciones.

---

## Fase 1 — Ordenamiento con ShellSort

### Objetivo

La Fase 1 establece la base técnica del sistema mediante:

* Modelado de datos con `struct`.
* Implementación nativa del algoritmo ShellSort.
* Ordenamiento por precio descendente.
* Desempate por SKU ascendente.
* Medición de rendimiento mediante `Stopwatch`.
* Pruebas con un lote de 50 productos simulados.

### Estructuras utilizadas

#### Proveedor

La estructura `Proveedor` almacena:

* ID del proveedor.
* Nombre corporativo.

#### Producto

La estructura `Producto` contiene:

* SKU.
* Nombre.
* Precio.
* Stock.
* Datos del proveedor.

### Algoritmo ShellSort

Se implementó ShellSort de forma iterativa y sin utilizar LINQ, `Array.Sort()` ni `.Sort()`.

Se utiliza la secuencia de Knuth para calcular las brechas:

`1, 4, 13, 40, 121...`

El criterio de ordenamiento es:

1. Precio descendente.
2. SKU ascendente cuando existe empate de precio.

### Datos de prueba

El programa genera 50 productos simulados con:

* SKU únicos desde 1001.
* Precios entre $10.00 y $9,999.99.
* Stock entre 0 y 500.
* Productos con precios iguales para verificar el desempate por SKU.

---

## Fase 2 — Arquitectura Dinámica del Catálogo Maestro

### Objetivo

La Fase 2 migra el catálogo de productos de una estructura basada en arreglos a una **Lista Simplemente Enlazada**, desarrollada manualmente en C#.

Esta estructura permite que el catálogo crezca dinámicamente durante la ejecución sin depender de un tamaño fijo.

### NodoProducto

Se implementó la clase `NodoProducto`, encargada de representar cada elemento de la lista enlazada.

Cada nodo contiene:

* `Data`: almacena la estructura `Producto`.
* `Siguiente`: referencia al siguiente nodo de la lista.

Cuando `Siguiente` es `null`, significa que se alcanzó el final de la lista.

### InventarioLista

La clase `InventarioLista` administra la estructura enlazada mediante la referencia `_cabeza`.

Se implementaron las siguientes operaciones:

#### InsertarInicio

Inserta un producto al comienzo de la lista.

Complejidad temporal:

`O(1)`

#### InsertarOrdenado

Inserta un producto manteniendo el catálogo ordenado por precio ascendente.

Complejidad temporal:

`O(n)`

#### BuscarPorSKU

Realiza una búsqueda secuencial utilizando el SKU del producto.

Complejidad temporal:

`O(n)`

Si el SKU solicitado no existe, se genera una excepción controlada mediante `KeyNotFoundException`.

#### ActualizarPrecio

Actualiza el precio de un producto existente a partir de su SKU.

La operación también genera automáticamente un registro en la bitácora de auditoría implementada en la Fase 3.

#### EliminarPorSKU

Localiza un producto por SKU y elimina su nodo mediante el reenlace de referencias.

El nodo eliminado deja de formar parte de la cadena y posteriormente puede ser recuperado por el Garbage Collector de .NET.

#### DisminuirStock

Disminuye el stock real de un producto mediante su SKU.

Este método es utilizado por la cola de despacho de la Fase 4.

#### IncrementarStock

Incrementa el stock real de un producto mediante su SKU.

Este método es utilizado por la pila de devoluciones de la Fase 4.

### Pruebas de la Fase 2

Se realizaron pruebas mediante consola con 15 productos.

Se comprobó que:

* Los productos pueden insertarse dinámicamente.
* La lista aumenta su tamaño durante la ejecución.
* Los productos quedan ordenados de menor a mayor precio.
* La búsqueda por SKU localiza correctamente los productos.
* Un SKU inexistente genera una excepción controlada.
* Es posible actualizar el precio de un producto.
* Es posible eliminar un producto mediante su SKU.
* La lista conserva su integridad después de una eliminación.

---

## Fase 3 — Bitácora de Auditoría con Lista Doblemente Enlazada

### Objetivo

La Fase 3 incorpora un motor de auditoría capaz de registrar automáticamente los cambios realizados sobre el inventario.

La bitácora utiliza una **Lista Doblemente Enlazada**, permitiendo recorrer los eventos tanto en orden cronológico como en orden inverso.

La integración entre `InventarioLista` y `AuditoriaService` se realiza mediante inyección de dependencias.

### LogMovimiento

Se implementó el `struct LogMovimiento`, encargado de almacenar la información de cada evento.

Cada registro contiene:

* `Timestamp`: fecha y hora UTC del evento.
* `TipoOperacion`: tipo de operación realizada.
* `SKUAfectado`: SKU del producto involucrado.
* `Descripcion`: detalle legible del cambio realizado.

### NodoAuditoria

La clase `NodoAuditoria` representa cada elemento de la lista doblemente enlazada.

Cada nodo contiene:

* `Dato`: registro de tipo `LogMovimiento`.
* `Siguiente`: referencia al siguiente evento.
* `Anterior`: referencia al evento anterior.

Las referencias se declaran como anulables debido a que:

* La cabeza de la lista no tiene nodo anterior.
* La cola de la lista no tiene nodo siguiente.

### AuditoriaService

La clase `AuditoriaService` administra la bitácora mediante dos referencias internas:

* `Cabeza`: primer evento registrado.
* `Cola`: evento más reciente.

Ambas referencias permanecen privadas para evitar modificaciones externas de la estructura.

### RegistrarEvento

Inserta cada nuevo evento al final de la lista.

Gracias al puntero `Cola`, la operación se realiza en:

`O(1)`

Cada registro contiene una marca temporal obtenida mediante:

```csharp
DateTime.UtcNow
```

### Historial cronológico

El método:

```csharp
ImprimirHistorialCronologico()
```

recorre la estructura desde `Cabeza` hasta `Cola` utilizando el puntero `Siguiente`.

Complejidad temporal:

`O(n)`

El resultado muestra los eventos desde el más antiguo hasta el más reciente.

### Historial inverso

El método:

```csharp
ImprimirHistorialInverso()
```

recorre la estructura desde `Cola` hasta `Cabeza` utilizando el puntero `Anterior`.

Complejidad temporal:

`O(n)`

La lista doblemente enlazada permite realizar este recorrido sin utilizar arreglos, pilas ni estructuras auxiliares adicionales.

### TotalRegistros

La propiedad:

```csharp
TotalRegistros
```

mantiene el número de eventos almacenados en la bitácora.

El valor únicamente puede modificarse desde `AuditoriaService`.

### Validación de integridad

Se implementó el método:

```csharp
ValidarIntegridad()
```

para comprobar que:

* `Cabeza.Anterior` sea `null`.
* `Cola.Siguiente` sea `null`.
* Los enlaces `Siguiente` y `Anterior` sean recíprocos.
* El recorrido hacia adelante tenga el mismo número de nodos que el recorrido hacia atrás.
* El número de nodos coincida con `TotalRegistros`.

### Integración con InventarioLista

`InventarioLista` recibe una instancia de `AuditoriaService` mediante su constructor:

```csharp
InventarioLista inventario =
    new InventarioLista(auditoria);
```

El constructor valida que la dependencia no sea nula mediante:

```csharp
ArgumentNullException.ThrowIfNull(auditoria);
```

Las siguientes operaciones generan automáticamente un registro:

* `INSERCION`
* `ACTUALIZACION`
* `ELIMINACION`
* `DESPACHO`
* `DEVOLUCION`

De esta manera, cada modificación realizada sobre el inventario queda registrada automáticamente en la bitácora.

### Ejecución de prueba de la Fase 3

La demostración utiliza 15 productos.

Durante la ejecución inicial se generan eventos de inserción correspondientes a cada producto del catálogo.

El historial puede visualizarse en dos sentidos:

```text
Antiguo -> Reciente
```

y:

```text
Reciente -> Antiguo
```

También se comprueba la integridad de la estructura mediante `ValidarIntegridad()`.

---

## Pruebas Unitarias — Fase 3

Las pruebas se encuentran en:

```text
Tests/
```

Se utiliza:

* xUnit
* Microsoft.NET.Test.Sdk
* coverlet.collector

Se implementaron 6 pruebas unitarias para la Fase 3:

1. Registro del primer evento en una lista vacía.
2. Registro de múltiples eventos.
3. Verificación del recorrido cronológico.
4. Verificación del recorrido inverso.
5. Validación de `ArgumentNullException` cuando no se proporciona `AuditoriaService`.
6. Validación de integridad con múltiples nodos.

---

## Fase 4 — Motor de Despacho con Cola FIFO y Pila LIFO

### Objetivo

La Fase 4 integra un motor de despacho logístico utilizando estructuras dinámicas implementadas manualmente mediante nodos enlazados.

Se implementaron dos estructuras principales:

* Cola dinámica FIFO para gestionar pedidos.
* Pila dinámica LIFO para gestionar devoluciones.

Ambas estructuras se integran con `InventarioLista` y `AuditoriaService`, permitiendo modificar el stock real de los productos y registrar automáticamente los movimientos realizados.

No se utilizan `Queue<T>`, `Stack<T>` ni LINQ para implementar las operaciones estructurales.

### Pedido

La clase `Pedido` representa la información almacenada dentro de la cola.

Cada pedido contiene:

* ID del pedido.
* SKU.
* Cantidad.
* Cliente.
* Fecha y hora de registro.

### NodoCola

La clase `NodoCola` representa cada elemento de la cola dinámica.

Cada nodo contiene:

* `Dato`: objeto de tipo `Pedido`.
* `Siguiente`: referencia al siguiente nodo.

Cuando `Siguiente` es `null`, el nodo representa el final de la cola.

### ColaDespacho — FIFO

La clase `ColaDespacho` administra los pedidos pendientes mediante dos referencias:

* `Frente`: primer pedido pendiente.
* `Fin`: último pedido agregado.

La estructura fue implementada manualmente mediante nodos enlazados.

### EncolarPedido

El método:

```csharp
EncolarPedido()
```

inserta un nuevo pedido al final de la cola mediante el puntero `Fin`.

Complejidad temporal:

```text
O(1)
```

### DespacharPedido

El método:

```csharp
DespacharPedido()
```

procesa el pedido ubicado en `Frente`, respetando el principio:

```text
FIFO — First In, First Out
```

Antes de retirar el nodo de la cola se actualiza el stock real mediante:

```csharp
InventarioLista.DisminuirStock()
```

Si el SKU no existe o no hay stock suficiente, se genera una excepción controlada y el pedido permanece en la cola.

Cuando el despacho se realiza correctamente:

* Se disminuye el stock.
* Se elimina el nodo ubicado en `Frente`.
* Se actualiza el puntero `Frente`.
* Si la cola queda vacía, `Fin` también se establece en `null`.
* Se registra el movimiento `DESPACHO` en la bitácora.

---

## Devolucion

La clase `Devolucion` representa la información almacenada dentro de la pila.

Cada devolución contiene:

* ID de devolución.
* SKU.
* Cantidad.
* Motivo.
* Fecha y hora de registro.

### NodoPila

La clase `NodoPila` representa cada elemento de la pila dinámica.

Cada nodo contiene:

* `Dato`: objeto de tipo `Devolucion`.
* `Siguiente`: referencia al nodo inferior de la pila.

### PilaDevoluciones — LIFO

La clase `PilaDevoluciones` administra las devoluciones mediante un único puntero:

```text
Top
```

La estructura fue implementada manualmente mediante nodos enlazados.

### PushDevolucion

El método:

```csharp
PushDevolucion()
```

inserta una devolución en la cima de la pila.

Complejidad temporal:

```text
O(1)
```

### PopDevolucion

El método:

```csharp
PopDevolucion()
```

procesa la devolución ubicada en `Top`, respetando el principio:

```text
LIFO — Last In, First Out
```

Al procesar una devolución, las unidades son reintegradas al inventario mediante:

```csharp
InventarioLista.IncrementarStock()
```

Cuando la devolución se procesa correctamente:

* Se incrementa el stock.
* Se elimina el nodo ubicado en `Top`.
* `Top` avanza al siguiente nodo.
* Se registra el movimiento `DEVOLUCION` en la bitácora.

---

## Integración de la Fase 4 con el inventario

Para permitir que la cola y la pila modifiquen el inventario real se incorporaron los métodos:

```csharp
DisminuirStock()
IncrementarStock()
```

`DisminuirStock()` se utiliza al despachar pedidos.

`IncrementarStock()` se utiliza al procesar devoluciones.

Debido a que `Producto` está implementado como `struct`, después de modificar el stock se asigna nuevamente el producto actualizado al nodo correspondiente.

---

## Integración de la Fase 4 con la bitácora

Los movimientos logísticos generan automáticamente eventos de auditoría.

Los nuevos tipos de operación son:

```text
DESPACHO
DEVOLUCION
```

Esto permite conservar la trazabilidad de los cambios de stock realizados por la cola y la pila.

---

## Menú Maestro — Integración de las cuatro fases

Se implementó un menú interactivo como punto de entrada único del sistema.

El menú permite acceder a funcionalidades de las cuatro fases.

### Fase 1

```text
[1] Ejecutar demostración ShellSort
```

### Fase 2

```text
[2] Agregar producto
[3] Buscar producto por SKU
[4] Eliminar producto
[5] Mostrar catálogo
```

### Fase 3

```text
[6] Ver historial de bitácora
[7] Validar integridad de bitácora
```

### Fase 4

```text
[8] Encolar nuevo pedido
[9] Despachar pedido FIFO
[10] Registrar devolución LIFO
[11] Procesar devolución
[12] Ver estado de cola y pila
```

### Salida

```text
[0] Salir
```

---

## Pruebas Unitarias — Fase 4

Se agregaron 7 pruebas automatizadas para verificar:

1. Inserción de pedidos en la cola.
2. Disminución real del stock durante un despacho.
3. Orden FIFO de los pedidos.
4. Inserción de devoluciones en la pila.
5. Reintegración del stock durante una devolución.
6. Orden LIFO de las devoluciones.
7. Control de stock insuficiente durante un despacho.

En conjunto con las 6 pruebas de la Fase 3, actualmente se ejecutan:

```text
Total: 13
Correctas: 13
Con errores: 0
Omitidas: 0
```

Para ejecutar las pruebas:

```powershell
dotnet test .\Tests\FastCartBackendCore.Tests.csproj
```

---

## Flujo de integración comprobado

Durante las pruebas manuales se verificó el siguiente flujo:

```text
Stock inicial SKU 2001: 12

Pedido:
SKU: 2001
Cantidad: 2

Despacho FIFO:
12 -> 10

Devolución:
SKU: 2001
Cantidad: 2

Procesamiento LIFO:
10 -> 12
```

La bitácora registró correctamente:

```text
DESPACHO
DEVOLUCION
```

Con esto se comprobó la integración entre:

```text
InventarioLista
      ↓
ColaDespacho / PilaDevoluciones
      ↓
AuditoriaService
```

---

## Comparación de estructuras utilizadas

| Característica           | Arreglo — Fase 1     | Lista Simple — Fase 2 | Lista Doble — Fase 3   | Cola / Pila — Fase 4     |
| ------------------------ | -------------------- | --------------------- | ---------------------- | ------------------------ |
| Tamaño                   | Fijo                 | Dinámico              | Dinámico               | Dinámico                 |
| Inserción                | Depende de capacidad | O(1) al inicio        | O(1) en cola           | O(1)                     |
| Búsqueda                 | O(n)                 | O(n)                  | O(n)                   | Depende del inventario   |
| Recorrido hacia adelante | Sí                   | Sí                    | Sí                     | Sí                       |
| Recorrido hacia atrás    | No directamente      | No                    | Sí                     | No                       |
| Memoria adicional        | Baja                 | 1 referencia por nodo | 2 referencias por nodo | 1 referencia por nodo    |
| Uso principal            | Ordenamiento         | Inventario            | Auditoría              | Despachos y devoluciones |

---

## Ejecución

Para restaurar dependencias:

```powershell
dotnet restore
```

Para compilar:

```powershell
dotnet build
```

Para ejecutar:

```powershell
dotnet run
```

Para compilar en modo Release:

```powershell
dotnet build -c Release
```

Para ejecutar las pruebas unitarias:

```powershell
dotnet test .\Tests\FastCartBackendCore.Tests.csproj
```

---

## Tecnologías utilizadas

* C#
* .NET 10
* ShellSort
* Arreglos
* Listas simplemente enlazadas
* Listas doblemente enlazadas
* Cola dinámica FIFO
* Pila dinámica LIFO
* xUnit
* Inyección de dependencias
* Git
* GitHub
* `System.Diagnostics.Stopwatch`

---

## Control de versiones

El desarrollo se organiza mediante ramas independientes:

* `proyecto/fase1-shellsort`
* `proyecto/fase2-listas`
* `proyecto/fase3-bitacora`
* `proyecto/fase4-pilas-colas`

Las fases utilizan commits incrementales para documentar la evolución del proyecto.

Entre los tipos de Conventional Commits utilizados se encuentran:

```text
feat
test
docs
fix
refactor
```

La Fase 4 incluye commits separados para:

* Actualización del stock.
* Implementación de la cola FIFO.
* Implementación de la pila LIFO.
* Integración del menú maestro.
* Pruebas automatizadas.
* Documentación.

La rama `proyecto/fase4-pilas-colas` será integrada a `main` mediante Pull Request después de verificar compilación, pruebas y documentación.

---

## Estado actual del proyecto

La implementación actual permite:

* Ordenar productos mediante ShellSort.
* Administrar un inventario mediante una lista simplemente enlazada.
* Buscar productos mediante SKU.
* Actualizar productos.
* Eliminar productos.
* Registrar automáticamente cambios del inventario.
* Navegar el historial de auditoría en ambas direcciones.
* Mantener el conteo total de eventos.
* Validar la integridad de los punteros de la bitácora.
* Gestionar pedidos mediante una cola FIFO dinámica.
* Gestionar devoluciones mediante una pila LIFO dinámica.
* Disminuir stock automáticamente durante los despachos.
* Reintegrar stock automáticamente durante las devoluciones.
* Registrar despachos y devoluciones en la bitácora.
* Ejecutar las cuatro fases desde un menú maestro.
* Controlar errores por SKU inexistente.
* Controlar errores por stock insuficiente.
* Ejecutar 13 pruebas automatizadas correctamente.
* Mantener un historial de desarrollo mediante Git y GitHub.

---

## Resultado de validación actual

La ejecución de las pruebas automatizadas produjo:

```text
Resumen de pruebas:
total: 13
con errores: 0
correcto: 13
omitido: 0
```

La compilación del proyecto también finalizó correctamente.

Por lo tanto, la integración funcional de las cuatro fases se encuentra operativa.
