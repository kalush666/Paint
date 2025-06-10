@echo off
echo Starting MongoDB container...
docker-compose up -d

@echo off
echo Starting Server...
start dotnet run --project Server

timeout /t 2 >nul

echo Starting Client...
start dotnet run --project Client
