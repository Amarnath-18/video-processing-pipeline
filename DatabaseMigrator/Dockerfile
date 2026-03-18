FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["DatabaseMigrator/DatabaseMigrator.csproj", "DatabaseMigrator/"]
RUN dotnet restore "DatabaseMigrator/DatabaseMigrator.csproj"

COPY . .
WORKDIR "/src/DatabaseMigrator"
RUN dotnet publish "DatabaseMigrator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "DatabaseMigrator.dll"]
