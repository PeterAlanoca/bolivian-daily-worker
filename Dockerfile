# Dockerfile para BolivianDaily Worker (.NET 10)

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["BolivianDaily.Worker/BolivianDaily.Worker.csproj", "BolivianDaily.Worker/"]
COPY ["BolivianDaily.Application/BolivianDaily.Application.csproj", "BolivianDaily.Application/"]
COPY ["BolivianDaily.Domain/BolivianDaily.Domain.csproj", "BolivianDaily.Domain/"]
COPY ["BolivianDaily.Infrastructure/BolivianDaily.Infrastructure.csproj", "BolivianDaily.Infrastructure/"]

RUN dotnet restore "BolivianDaily.Worker/BolivianDaily.Worker.csproj"

# Copiar todo el código y construir
COPY . .
WORKDIR "/src/BolivianDaily.Worker"
RUN dotnet build "BolivianDaily.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BolivianDaily.Worker.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Generar imagen final
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BolivianDaily.Worker.dll"]
