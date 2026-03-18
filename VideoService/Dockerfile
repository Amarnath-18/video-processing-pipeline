FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["VideoService/VideoService.csproj", "VideoService/"]
RUN dotnet restore "VideoService/VideoService.csproj"

COPY . .
WORKDIR "/src/VideoService"
RUN dotnet publish "VideoService.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "VideoService.dll"]
