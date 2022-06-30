FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 7138

ENV ASPNETCORE_URLS=http://+:7138

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["PaymentService_API/PaymentService_API.csproj", "PaymentService_API/"]
RUN dotnet restore "PaymentService_API/PaymentService_API.csproj"
COPY . .
WORKDIR "/src/PaymentService_API"
RUN dotnet build "PaymentService_API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentService_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService_API.dll"]
