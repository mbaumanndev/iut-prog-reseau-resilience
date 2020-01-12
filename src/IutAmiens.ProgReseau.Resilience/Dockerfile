FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine
WORKDIR /app
COPY artifacts/resilience/ ./
ENTRYPOINT ["dotnet", "IutAmiens.ProgReseau.Resilience.dll"]