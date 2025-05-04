# Use official ASP.NET runtime image for .NET 8
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Build the app using the SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["retail-management.csproj", "./"]
RUN dotnet restore "retail-management.csproj"

# Copy the remaining source code
COPY . .

# Publish the app
RUN dotnet publish "retail-management.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "retail-management.dll"]
