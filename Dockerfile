# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Disable globalization support to reduce dependencies (optional, but recommended for slim images)
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1

# Entry point for the MCP server
ENTRYPOINT ["dotnet", "gemini.dynamic.mcp.dll"]
