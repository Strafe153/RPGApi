# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY ../*.sln ./
COPY ../WebApi/*.csproj ./WebApi/
COPY ../Domain/*.csproj ./Domain/
COPY ../DataAccess/*.csproj ./DataAccess/
COPY ../Application/*.csproj ./Application/
COPY ../Tests/Application.Tests/*.csproj ./Tests/Application.Tests/
COPY ../Tests/WebApi.Tests/*.csproj ./Tests/WebApi.Tests/
RUN dotnet restore
    
# Copy everything else and build
COPY ../WebApi/ ./WebApi/
COPY ../Domain/ ./Domain/
COPY ../DataAccess/ ./DataAccess/
COPY ../Application/ ./Application/
COPY ../Tests/Application.Tests/ ./Tests/Application.Tests/
COPY ../Tests/WebApi.Tests/ ./Tests/WebApi.Tests/
WORKDIR /app/WebApi
RUN dotnet publish -c Release -o /app/out
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]