# Base runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
# For DataProtection keys
RUN mkdir -p /app/keys && chmod -R 700 /app/keys

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["retail-management/retail-management.csproj", "retail-management/"]
RUN dotnet add "retail-management/retail-management.csproj" package DotNetEnv
RUN dotnet restore "retail-management/retail-management.csproj"

# Copy and build
COPY . .
WORKDIR "/src/retail-management"
RUN dotnet publish "retail-management.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
VOLUME ["/app/keys"]
ENTRYPOINT ["dotnet", "retail-management.dll"]
