FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
EXPOSE 12345
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Jane.Server/Jane.Server.csproj", "Jane.Server/"]
COPY ["Jane.Unity/Jane.Unity.ServerShared.csproj", "Jane.Shared/"]
RUN dotnet restore "Jane.Server/Jane.Server.csproj"
COPY . .
WORKDIR "/src/Jane.Server"
RUN dotnet build "Jane.Server.csproj" -c Debug -o /app

FROM build AS publish
RUN dotnet publish "Jane.Server.csproj" -c Debug -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Jane.Server.dll"]