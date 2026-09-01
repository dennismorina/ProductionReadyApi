FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["global.json", "."]
COPY ["Directory.Build.props", "."]
COPY ["src/ProductionReadyApi.Domain/ProductionReadyApi.Domain.csproj", "src/ProductionReadyApi.Domain/"]
COPY ["src/ProductionReadyApi.Application/ProductionReadyApi.Application.csproj", "src/ProductionReadyApi.Application/"]
COPY ["src/ProductionReadyApi.Infrastructure/ProductionReadyApi.Infrastructure.csproj", "src/ProductionReadyApi.Infrastructure/"]
COPY ["src/ProductionReadyApi.Api/ProductionReadyApi.Api.csproj", "src/ProductionReadyApi.Api/"]

RUN dotnet restore "src/ProductionReadyApi.Api/ProductionReadyApi.Api.csproj"

COPY . .
RUN dotnet publish "src/ProductionReadyApi.Api/ProductionReadyApi.Api.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductionReadyApi.Api.dll"]
