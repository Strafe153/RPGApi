# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app
    
# Copy csproj and restore as distinct layers
COPY *.sln .
COPY ./WebApi/*.csproj ./WebApi/
COPY ./Application/*.csproj ./Application/
COPY ./Core/*.csproj ./Core/
COPY ./DataAccess/*.csproj ./DataAccess/
COPY ./Tests/Application.Tests/*.csproj ./Tests/Application.Tests/
COPY ./Tests/WebApi.Tests/*.csproj ./Tests/WebApi.Tests/
RUN dotnet restore
    
# Copy everything else and build
COPY ./ ./
RUN dotnet publish -c Release -o out
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WebApi.dll"]