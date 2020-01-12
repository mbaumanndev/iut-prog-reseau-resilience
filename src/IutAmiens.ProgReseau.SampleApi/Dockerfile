FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-alpine
WORKDIR /app
COPY artifacts/sampleapi/ ./
ENTRYPOINT ["dotnet", "IutAmiens.ProgReseau.SampleApi.dll"]