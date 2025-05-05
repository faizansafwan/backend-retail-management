# Use official ASP.NET runtime image as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build stage with SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["retail-management/retail-management.csproj", "retail-management/"]
RUN dotnet restore "retail-management/retail-management.csproj"

# Copy remaining project files and build
COPY . .
WORKDIR "/src/retail-management"
RUN dotnet publish "retail-management.csproj" -c Release -o /app/publish

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "retail-management.dll"]
