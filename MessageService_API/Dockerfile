FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 7015

ENV ASPNETCORE_URLS=http://+:7015

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["MessageService_API/MessageService_API.csproj", "MessageService_API/"]
RUN dotnet restore "MessageService_API/MessageService_API.csproj"
COPY . .
WORKDIR "/src/MessageService_API"
RUN dotnet build "MessageService_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MessageService_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MessageService_API.dll"]
