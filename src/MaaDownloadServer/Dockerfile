FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/MaaServer.Download/MaaServer.Download.csproj", "MaaServer.Download/"]
RUN dotnet restore "src/MaaServer.Download/MaaServer.Download.csproj"
COPY . .
WORKDIR "/src/MaaServer.Download"
RUN dotnet build "MaaServer.Download.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MaaServer.Download.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MaaServer.Download.dll"]
