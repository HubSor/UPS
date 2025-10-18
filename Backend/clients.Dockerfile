#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 2007
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY "." "."
WORKDIR /src/ClientsMicro
RUN dotnet restore "ClientsMicro.csproj"
RUN dotnet build "ClientsMicro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ClientsMicro.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClientsMicro.dll"]