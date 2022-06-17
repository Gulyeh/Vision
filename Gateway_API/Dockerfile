FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 7271

ENV ASPNETCORE_URLS=http://+:7271

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["Gateway_API/Gateway_API.csproj", "Gateway_API/"]
RUN dotnet restore "Gateway_API/Gateway_API.csproj"
COPY . .
WORKDIR "/src/Gateway_API"
RUN dotnet build "Gateway_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Gateway_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Gateway_API.dll"]
