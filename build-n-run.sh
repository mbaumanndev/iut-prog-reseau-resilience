#!/bin/bash

dotnet restore
dotnet build -c Release --no-restore
dotnet publish src/IutAmiens.ProgReseau.Resilience -c Release -o artifacts/resilience --no-build
dotnet publish src/IutAmiens.ProgReseau.SampleApi -c Release -o artifacts/sampleapi --no-build
docker-compose up --build
