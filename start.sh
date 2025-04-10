#!/bin/bash

# Crear el archivo .env con las variables de entorno
echo "DEFAULT_CONNECTION=\"$DEFAULT_CONNECTION\"" > /app/.env
echo "DIRECT_CONNECTION=\"$DIRECT_CONNECTION\"" >> /app/.env
echo "JWT_SECRET=\"$JWT_SECRET\"" >> /app/.env
echo "JWT_EXPIRY_MINUTES=$JWT_EXPIRY_MINUTES" >> /app/.env
echo "LOGGING_ENABLED=$LOGGING_ENABLED" >> /app/.env

# Mostrar información de depuración
echo "Contenido del archivo .env creado:"
cat /app/.env

# Iniciar la aplicación
exec dotnet BookReviews.API.dll