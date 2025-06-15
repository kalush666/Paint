@echo off
echo Starting MongoDB container...
docker-compose up -d

echo Starting Server...
start cmd /k "dotnet run --project Server"

timeout /t 2 >nul

echo Starting Client 1...
start cmd /k "dotnet run --project Client\Client.csproj"

timeout /t 1 >nul

echo Starting Client 2...
start cmd /k "dotnet run --project Client\Client.csproj"
