FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 7123

ENV ASPNETCORE_URLS=http://+:7123

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["GameAccessService_API/GameAccessService_API.csproj", "GameAccessService_API/"]
RUN dotnet restore "GameAccessService_API/GameAccessService_API.csproj"
COPY . .
WORKDIR "/src/GameAccessService_API"
RUN dotnet build "GameAccessService_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameAccessService_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameAccessService_API.dll"]
