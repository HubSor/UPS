#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 2007
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY "." "."
WORKDIR /src/ProductsMicro
RUN dotnet restore "ProductsMicro.csproj"
RUN dotnet build "ProductsMicro.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductsMicro.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductsMicro.dll"]