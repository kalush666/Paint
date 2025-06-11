@echo off
echo Starting MongoDB container...
docker-compose up -d

echo Starting Server...
start cmd /k "dotnet run --project Server"

timeout /t 2 >nul

echo Starting Client...
start cmd /k "dotnet run --project Client\Client.csproj"