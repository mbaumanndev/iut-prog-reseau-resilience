#!/bin/bash

dotnet restore
dotnet build -c release --no-restore
dotnet publish src/IutAmiens.ProgReseau.Resilience -c release -o artifacts/resilience --no-build
dotnet publish src/IutAmiens.ProgReseau.SampleApi -c release -o artifacts/sampleapi --no-build
docker-compose up --build
