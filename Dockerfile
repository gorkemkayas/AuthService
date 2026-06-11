FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY AuthService.sln ./
COPY AuthService.API/AuthService.API.csproj AuthService.API/
COPY AuthService.Application/AuthService.Application.csproj AuthService.Application/
COPY AuthService.Domain/AuthService.Domain.csproj AuthService.Domain/
COPY AuthService.Infrastructure/AuthService.Infrastructure.csproj AuthService.Infrastructure/
COPY AuthService.Shared/AuthService.Shared.csproj AuthService.Shared/

RUN dotnet restore AuthService.API/AuthService.API.csproj

COPY . .
RUN dotnet publish AuthService.API/AuthService.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "AuthService.API.dll"]
