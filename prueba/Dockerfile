
# Etapa 1: Construcción de la aplicación
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 7500

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["prueba/prueba.csproj", "prueba/"]
RUN dotnet restore "prueba/prueba.csproj"
COPY . .
WORKDIR "/src/prueba"
RUN dotnet build "prueba.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "prueba.csproj" -c Release -o /app/publish

# Etapa 2: Construcción del contenedor final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copiar el archivo appsettings.json
COPY prueba/appsettings.json /app/
COPY prueba/dating.db /app/

# Configurar variables de entorno
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "prueba.dll"]
