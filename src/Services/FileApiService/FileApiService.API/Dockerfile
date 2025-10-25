FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/FileApiService/FileApiService.API/FileApiService.API.csproj", "Services/FileApiService/FileApiService.API/"]
RUN dotnet restore "Services/FileApiService/FileApiService.API/FileApiService.API.csproj"
COPY . .
WORKDIR "/src/Services/FileApiService/FileApiService.API"
RUN dotnet build "./FileApiService.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FileApiService.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FileApiService.API.dll"]
