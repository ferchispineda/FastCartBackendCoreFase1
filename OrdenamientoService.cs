public static class OrdenamientoService
{
    public static void ShellSort(Producto[] catalogo)
    {
        int n = catalogo.Length;

        // Calcular el gap inicial usando la secuencia de Knuth
        int gap = 1;

        while (gap < n / 3)
        {
            gap = gap * 3 + 1;
        }

        while (gap >= 1)
        {
            // Insertion Sort utilizando la brecha actual
            for (int i = gap; i < n; i++)
            {
                Producto temp = catalogo[i];
                int j = i;

                // Ordenar por Precio DESC y SKU ASC en caso de empate
                while (j >= gap && EsMayor(catalogo[j - gap], temp))
                {
                    catalogo[j] = catalogo[j - gap];
                    j -= gap;
                }

                catalogo[j] = temp;
            }

            // Reducir la brecha
            gap = gap / 3;
        }
    }

    private static bool EsMayor(Producto a, Producto b)
    {
        if (a.Precio != b.Precio)
        {
            return a.Precio < b.Precio;
        }

        return a.SKU > b.SKU;
    }
}