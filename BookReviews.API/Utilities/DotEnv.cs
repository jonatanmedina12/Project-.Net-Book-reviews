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
            try
            {
                // Verificar si estamos en Railway (comprobando una de sus variables específicas)
                bool isRailway = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));

                if (isRailway)
                {
                    Console.WriteLine("Detectado entorno Railway. Las variables de entorno ya están configuradas por Railway.");

                    // Imprimir las variables de entorno disponibles para depuración
                    Console.WriteLine("Variables de entorno disponibles:");
                    Console.WriteLine($"DEFAULT_CONNECTION: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION"))}");
                    Console.WriteLine($"DIRECT_CONNECTION: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DIRECT_CONNECTION"))}");
                    Console.WriteLine($"JWT_SECRET: {!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET"))}");
                    Console.WriteLine($"JWT_EXPIRY_MINUTES: {Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES")}");
                    Console.WriteLine($"LOGGING_ENABLED: {Environment.GetEnvironmentVariable("LOGGING_ENABLED")}");

                    // Crear un archivo .env en Railway para debugging
                    try
                    {
                        string envContent =
                            $"DEFAULT_CONNECTION=\"{Environment.GetEnvironmentVariable("DEFAULT_CONNECTION")}\"\n" +
                            $"DIRECT_CONNECTION=\"{Environment.GetEnvironmentVariable("DIRECT_CONNECTION")}\"\n" +
                            $"JWT_SECRET=\"{Environment.GetEnvironmentVariable("JWT_SECRET")}\"\n" +
                            $"JWT_EXPIRY_MINUTES={Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES")}\n" +
                            $"LOGGING_ENABLED={Environment.GetEnvironmentVariable("LOGGING_ENABLED")}\n";

                        File.WriteAllText(".env", envContent);
                        Console.WriteLine("Contenido del archivo .env creado:");
                        Console.WriteLine(envContent.Replace(Environment.GetEnvironmentVariable("DEFAULT_CONNECTION") ?? "", "")
                                                 .Replace(Environment.GetEnvironmentVariable("DIRECT_CONNECTION") ?? "", "")
                                                 .Replace(Environment.GetEnvironmentVariable("JWT_SECRET") ?? "", ""));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al crear archivo .env para debugging: {ex.Message}");
                    }

                    return;
                }

                // El resto del código para entorno local...
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en DotEnv.Load: {ex.Message}");
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