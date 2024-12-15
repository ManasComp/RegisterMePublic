# Stage 1: Base Image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# Stage 2: Build Image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore as distinct layers
COPY ["global.json", "Directory.Build.props", "Directory.Packages.props", "./"]
COPY ["src/WebGui/WebGui.csproj", "src/WebGui/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["src/DAL.Migrations.Postgres/DAL.Migrations.Postgres.csproj", "src/DAL.Migrations.Postgres/"]
COPY ["src/DAL.Migrations.SqlServer/DAL.Migrations.SqlServer.csproj", "src/DAL.Migrations.SqlServer/"]
RUN dotnet restore "src/WebGui/WebGui.csproj"

# Copy the remaining source files and build
COPY . .
WORKDIR "/src/src/WebGui"
RUN dotnet build "WebGui.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3: Publish Image
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebGui.csproj" -c $BUILD_CONFIGURATION -o /app/publish

# Stage 4: Final Image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebGui.dll"]