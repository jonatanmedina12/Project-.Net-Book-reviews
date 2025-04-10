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
        /// <param name="filePath">Ruta al archivo .env. Por defecto, busca automáticamente.</param>
        public static void Load(string filePath = null)
        {
            try
            {
                // Detectar si estamos en Railway
                bool isRailway = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_SERVICE_NAME")) ||
                                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RAILWAY_STATIC_URL"));

                // Buscar el archivo .env
                var envFilePath = filePath ?? FindEnvFile(isRailway);

                Console.WriteLine($"Buscando archivo .env en: {envFilePath}");

                // Verificar si el archivo existe
                if (!File.Exists(envFilePath))
                {
                    Console.WriteLine($"Archivo .env no encontrado en {envFilePath}. Se usarán las variables de entorno del sistema o los valores por defecto.");
                    return;
                }

                Console.WriteLine($"Archivo .env encontrado. Cargando variables desde: {envFilePath}");

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

                        // Establecer la variable de entorno solo si no está ya definida
                        if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                        {
                            Environment.SetEnvironmentVariable(key, value);
                            Console.WriteLine($"Variable {key} cargada desde archivo .env");
                        }
                        else
                        {
                            Console.WriteLine($"Variable {key} ya definida en el entorno, no se sobrescribirá");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar el archivo .env: {ex.Message}");
            }
        }

        /// <summary>
        /// Encuentra el archivo .env en diferentes ubicaciones según el entorno
        /// </summary>
        /// <param name="isRailway">Indica si estamos en Railway</param>
        /// <returns>Ruta al archivo .env</returns>
        public static string FindEnvFile(bool isRailway) // Cambiado de private a public
        {
            // Lista de posibles ubicaciones para el archivo .env
            List<string> possibleLocations = new List<string>();

            // En Railway, buscar primero en la raíz del contenedor
            if (isRailway)
            {
                possibleLocations.Add("/app/.env");
                possibleLocations.Add("/app");
                possibleLocations.Add("/");
            }

            // Ubicaciones para entorno local (Windows u otros)
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            possibleLocations.Add(baseDir);

            // Navegar hacia arriba para encontrar el directorio del proyecto
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
                    // Añadir este directorio con prioridad
                    possibleLocations.Insert(0, Path.Combine(currentDir, ".env"));
                    break;
                }
            }

            // Buscar el archivo .env en las ubicaciones posibles
            foreach (var location in possibleLocations)
            {
                string pathToCheck;
                if (location.EndsWith(".env", StringComparison.OrdinalIgnoreCase))
                {
                    pathToCheck = location;
                }
                else
                {
                    pathToCheck = Path.Combine(location, ".env");
                }

                if (File.Exists(pathToCheck))
                {
                    return pathToCheck;
                }
            }

            // Si no encontramos el archivo, devolvemos la ruta por defecto
            return isRailway ? "/app/.env" : Path.Combine(baseDir, ".env");
        }
    }
}