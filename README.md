# FastCart Backend Core

Proyecto desarrollado en C# para implementar y evolucionar el núcleo de procesamiento del catálogo de productos de FastCart.

El proyecto está dividido en fases que permiten aplicar diferentes estructuras de datos y analizar su comportamiento en escenarios de inventario.

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

De esta manera, cada modificación realizada sobre el inventario queda registrada automáticamente en la bitácora.

### Ejecución de prueba de la Fase 3

La demostración utiliza 15 productos.

Durante la ejecución se generan:

* 15 eventos de `INSERCION`.
* 1 evento de `ACTUALIZACION`.
* 1 evento de `ELIMINACION`.

Total:

```text
17 eventos
```

Al finalizar se realizan dos recorridos:

```text
Antiguo -> Reciente
```

y:

```text
Reciente -> Antiguo
```

También se comprueba la integridad de la estructura:

```text
Total de registros: 17
Integridad de la lista: CORRECTA
```

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

Se implementaron 6 pruebas unitarias:

1. Registro del primer evento en una lista vacía.
2. Registro de múltiples eventos.
3. Verificación del recorrido cronológico.
4. Verificación del recorrido inverso.
5. Validación de `ArgumentNullException` cuando no se proporciona `AuditoriaService`.
6. Validación de integridad con múltiples nodos.

Resultado esperado:

```text
Resumen de pruebas:
total: 6
con errores: 0
correcto: 6
```

Para ejecutar las pruebas:

```powershell
dotnet test .\Tests\FastCartBackendCore.Tests.csproj
```

---

## Comparación de estructuras utilizadas

| Característica           | Arreglo — Fase 1     | Lista Simple — Fase 2 | Lista Doble — Fase 3   |
| ------------------------ | -------------------- | --------------------- | ---------------------- |
| Tamaño                   | Fijo                 | Dinámico              | Dinámico               |
| Inserción al extremo     | Depende de capacidad | O(1) al inicio        | O(1) en cola           |
| Búsqueda                 | O(n)                 | O(n)                  | O(n)                   |
| Recorrido hacia adelante | Sí                   | Sí                    | Sí                     |
| Recorrido hacia atrás    | No directamente      | No                    | Sí                     |
| Memoria adicional        | Baja                 | 1 referencia por nodo | 2 referencias por nodo |
| Uso principal            | Ordenamiento         | Inventario            | Auditoría              |

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
* Listas simplemente enlazadas
* Listas doblemente enlazadas
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

La Fase 3 utiliza Conventional Commits para documentar la evolución del proyecto.

Entre los tipos de commits utilizados se encuentran:

```text
feat
test
docs
```

La rama `proyecto/fase3-bitacora` se integra a la rama principal mediante Pull Request después de verificar compilación, pruebas y documentación.

---

## Estado de la Fase 3

La implementación actual permite:

* Registrar automáticamente cambios del inventario.
* Navegar el historial en ambas direcciones.
* Mantener el conteo total de eventos.
* Validar la integridad de los punteros.
* Controlar dependencias nulas.
* Ejecutar pruebas unitarias automatizadas.
* Mantener encapsulada la estructura interna de auditoría.
