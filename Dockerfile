# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["inventory_system.csproj", "./"]
RUN dotnet restore

# Copy the rest of the code
COPY . .

# Build the project
RUN dotnet build "inventory_system.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "inventory_system.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose the port your app runs on
EXPOSE 80
EXPOSE 443

# Set the entry point for the application
ENTRYPOINT ["dotnet", "inventory_system.dll"]