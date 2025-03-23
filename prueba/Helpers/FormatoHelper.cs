namespace prueba.Helpers
{
    public static class FormatoHelper
    {
        public static string FormatearMoneda(decimal valor)
        {
            return $"${valor:N2}";
        }
    }
}