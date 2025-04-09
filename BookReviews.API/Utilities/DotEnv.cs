namespace BookReviews.API.Utilities
{
    /// <summary>
    /// Utilidad para cargar variables de entorno desde un archivo .env
    /// </summary>
    public static class DotEnv
    {
        /// <summary>
        /// Carga variables de entorno desde el archivo .env
        /// </summary>
        /// <param name="filePath">Ruta al archivo .env. Por defecto, busca en el directorio raíz del proyecto.</param>
        public static void Load(string filePath = null)
        {
            // Obtener la ruta al archivo .env
            var envFilePath = filePath ?? Path.Combine(Directory.GetCurrentDirectory(), ".env");

            // Verificar si el archivo existe
            if (!File.Exists(envFilePath))
            {
                Console.WriteLine($"Archivo .env no encontrado en {envFilePath}. Se usarán las variables de entorno del sistema o los valores por defecto.");
                return;
            }

            // Leer todas las líneas del archivo
            foreach (var line in File.ReadAllLines(envFilePath))
            {
                // Ignorar líneas vacías o comentarios
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                {
                    continue;
                }

                // Separar clave y valor
                var parts = line.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length == 2)
                {
                    var key = parts[0].Trim();
                    var value = parts[1].Trim();

                    // Eliminar comillas si están presentes
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    // Establecer la variable de entorno
                    Environment.SetEnvironmentVariable(key, value);
                }
            }
        }
    }
}
