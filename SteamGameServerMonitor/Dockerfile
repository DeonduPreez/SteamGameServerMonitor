FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["SteamGameServerMonitor/SteamGameServerMonitor.csproj", "SteamGameServerMonitor/"]
RUN dotnet restore "SteamGameServerMonitor/SteamGameServerMonitor.csproj"
COPY . .
WORKDIR "/src/SteamGameServerMonitor"
RUN dotnet build "SteamGameServerMonitor.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SteamGameServerMonitor.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SteamGameServerMonitor.dll"]
