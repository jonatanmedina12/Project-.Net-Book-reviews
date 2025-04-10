namespace BookReviews.API.Utilities
{
    /// <summary>
    /// Utilidad para cargar variables de entorno desde un archivo .env o usar las proporcionadas por Railway
    /// </summary>
    public static class DotEnv
    {
        /// <summary>
        /// Carga variables de entorno desde el archivo .env o usa las variables de Railway
        /// </summary>
        /// <param name="filePath">Ruta al archivo .env. Por defecto, busca en la carpeta raíz de la solución.</param>
        public static void Load(string filePath = null)
        {
            // Verificar si estamos en Railway (comprobando una de sus variables específicas)
            bool isRailway = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));

            if (isRailway)
            {
                Console.WriteLine("Detectado entorno Railway. Las variables de entorno ya están configuradas por Railway.");
                // En Railway las variables ya están disponibles, no necesitamos hacer nada
                return;
            }

            // Estamos en entorno de desarrollo local (Windows u otro)
            Console.WriteLine("Entorno de desarrollo local detectado. Buscando archivo .env...");

            // Obtener la ruta al archivo .env
            var envFilePath = filePath ?? FindEnvFile();

            // Verificar si el archivo existe
            if (!File.Exists(envFilePath))
            {
                Console.WriteLine($"Archivo .env no encontrado en {envFilePath}. Se usarán las variables de entorno del sistema o los valores por defecto.");
                return;
            }

            Console.WriteLine($"Cargando variables de entorno desde: {envFilePath}");

            // Leer y procesar el archivo .env
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

        /// <summary>
        /// Encuentra el archivo .env buscando en varias ubicaciones posibles
        /// </summary>
        /// <returns>Ruta al archivo .env o ruta por defecto si no se encuentra</returns>
        private static string FindEnvFile()
        {
            // Lista de posibles ubicaciones para el archivo .env
            List<string> possibleLocations = new List<string>();

            // 1. En el directorio raíz del proyecto (para desarrollo)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            possibleLocations.Add(baseDir);

            // 2. Navegar hacia arriba para encontrar el directorio del proyecto
            string currentDir = baseDir;
            for (int i = 0; i < 5; i++) // Subir hasta 5 niveles
            {
                var parentDir = Directory.GetParent(currentDir)?.FullName;
                if (parentDir == null) break;

                currentDir = parentDir;
                possibleLocations.Add(currentDir);

                // Si encontramos el directorio "Project-.Net-Book-reviews", es nuestra mejor opción
                if (Path.GetFileName(currentDir).Equals("Project-.Net-Book-reviews", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
            }

            // 3. Verificar si estamos en un contenedor Docker (para Railway y otros entornos containerizados)
            if (Directory.Exists("/app"))
            {
                possibleLocations.Add("/app");
            }

            // Buscar el archivo .env en las ubicaciones posibles
            foreach (var location in possibleLocations)
            {
                var envPath = Path.Combine(location, ".env");
                if (File.Exists(envPath))
                {
                    return envPath;
                }
            }

            // Si no encontramos el archivo, devolvemos la ruta en el directorio raíz del proyecto
            // (incluso si no existe, lo reportaremos después)
            return Path.Combine(baseDir, ".env");
        }
    }
}