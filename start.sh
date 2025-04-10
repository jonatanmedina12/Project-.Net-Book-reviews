#!/bin/bash

# Crear directorio /app/.env si no existe
mkdir -p /app

# Crear archivo .env con las variables de Railway
echo "DEFAULT_CONNECTION=\"$DEFAULT_CONNECTION\"" > /app/.env
echo "DIRECT_CONNECTION=\"$DIRECT_CONNECTION\"" >> /app/.env
echo "JWT_SECRET=\"$JWT_SECRET\"" >> /app/.env
echo "JWT_EXPIRY_MINUTES=$JWT_EXPIRY_MINUTES" >> /app/.env
echo "LOGGING_ENABLED=$LOGGING_ENABLED" >> /app/.env

# Imprimir mensaje para debug
echo "Archivo .env creado en /app/.env"

# Iniciar la aplicación
dotnet BookReviews.API.dll