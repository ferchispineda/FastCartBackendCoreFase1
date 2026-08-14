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

No requiere recorrer los demás nodos.

#### InsertarOrdenado

Inserta un producto manteniendo el catálogo ordenado por precio ascendente.

Complejidad temporal:

`O(n)`

El algoritmo recorre los nodos hasta encontrar la posición correcta del nuevo producto.

#### BuscarPorSKU

Realiza una búsqueda secuencial utilizando el SKU del producto.

Complejidad temporal:

`O(n)`

Si el SKU solicitado no existe, se genera una excepción controlada mediante `KeyNotFoundException`.

#### EliminarPorSKU

Localiza un producto por SKU y elimina su nodo mediante el reenlace de referencias.

El nodo eliminado deja de formar parte de la cadena y puede posteriormente ser recuperado por el Garbage Collector de .NET.

### Pruebas de la Fase 2

Se realizaron pruebas mediante consola con **15 productos**.

Se comprobó que:

* Los productos pueden insertarse dinámicamente.
* La lista aumenta su tamaño durante la ejecución.
* Los productos quedan ordenados de menor a mayor precio.
* La búsqueda por SKU localiza correctamente los productos.
* Un SKU inexistente genera una excepción controlada.
* Es posible eliminar un producto mediante su SKU.
* La lista conserva su integridad después de una eliminación.

---

## Comparación entre Fase 1 y Fase 2

| Característica              | Arreglo — Fase 1                    | Lista Enlazada — Fase 2   |
| --------------------------- | ----------------------------------- | ------------------------- |
| Tamaño                      | Fijo                                | Dinámico                  |
| Inserción ordenada          | O(n) + desplazamientos              | O(n) sin desplazamientos  |
| Acceso por índice           | O(1)                                | O(n)                      |
| Uso de memoria              | Reserva según capacidad del arreglo | Memoria asignada por nodo |
| Eliminación                 | Requiere reorganización             | Reenlace de nodos         |
| Gestión de nodos eliminados | No aplica de la misma forma         | Garbage Collector de .NET |

### Uso de memoria

En la Fase 1, el catálogo utiliza un arreglo cuya capacidad se determina al momento de crearlo. Esto puede resultar poco flexible cuando la cantidad de productos cambia durante la ejecución.

En la Fase 2, cada producto se almacena dentro de un nodo creado dinámicamente. Esto permite agregar elementos conforme sean necesarios y eliminar referencias cuando un producto deja de formar parte del catálogo.

La lista enlazada requiere memoria adicional para almacenar la referencia `Siguiente` de cada nodo. A cambio, proporciona mayor flexibilidad para realizar inserciones y eliminaciones sin desplazar los demás productos.

Por esta razón, la lista enlazada resulta apropiada para un catálogo dinámico como FastCart, mientras que un arreglo puede seguir siendo conveniente cuando el número de elementos es conocido y se necesita acceso directo por índice.

---

## Ejecución

Para compilar el proyecto:

```powershell
dotnet build
```

Para ejecutar:

```powershell
dotnet run
```

---

## Tecnologías utilizadas

* C#
* .NET
* Listas simplemente enlazadas
* ShellSort
* Git
* GitHub
* System.Diagnostics.Stopwatch

---

## Control de versiones

El desarrollo se organiza mediante ramas independientes:

* `proyecto/fase1-shellsort`
* `proyecto/fase2-listas`

La Fase 2 se documenta mediante un Pull Request para conservar el historial de cambios y facilitar la revisión del código.
