#!/bin/bash

# Crear archivo .env con las variables de Railway
echo "DEFAULT_CONNECTION=\"$DEFAULT_CONNECTION\"" > .env
echo "DIRECT_CONNECTION=\"$DIRECT_CONNECTION\"" >> .env
echo "JWT_SECRET=\"$JWT_SECRET\"" >> .env
echo "JWT_EXPIRY_MINUTES=$JWT_EXPIRY_MINUTES" >> .env
echo "LOGGING_ENABLED=$LOGGING_ENABLED" >> .env

# Imprimir variables para depuración (sin mostrar valores sensibles)
echo "Variables de entorno configuradas:"
echo "DEFAULT_CONNECTION=[HIDDEN]"
echo "DIRECT_CONNECTION=[HIDDEN]"
echo "JWT_SECRET=[HIDDEN]"
echo "JWT_EXPIRY_MINUTES=$JWT_EXPIRY_MINUTES"
echo "LOGGING_ENABLED=$LOGGING_ENABLED"

# Iniciar la aplicación
dotnet BookReviews.API.dll