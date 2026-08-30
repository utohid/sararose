# Windows (PowerShell) local run: MySQL in Docker, then API + Angular.
$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot
Set-Location $Root

function Require-Command($Name, $Hint) {
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "$Name was not found. $Hint"
    }
}

Require-Command docker "Install Docker Desktop for Windows and ensure it is running."
Require-Command dotnet "Install the .NET 8 SDK from https://dotnet.microsoft.com/download"
Require-Command npm "Install Node.js 20 LTS from https://nodejs.org/"

Write-Host "Starting MySQL (Docker)..."
docker compose up -d

$ready = $false
for ($i = 1; $i -le 40; $i++) {
    docker compose exec -T mysql mysqladmin ping -h 127.0.0.1 -uroot -pSaraRose_Root_2024 --silent 2>$null | Out-Null
    if ($LASTEXITCODE -eq 0) {
        $ready = $true
        break
    }
    Start-Sleep -Seconds 2
}

if (-not $ready) {
    throw "MySQL did not become ready. Check Docker Desktop and run: docker compose logs mysql"
}

Write-Host "Site:  http://127.0.0.1:43123"
Write-Host "API:   http://127.0.0.1:43124"
Write-Host "Swagger: http://127.0.0.1:43124/swagger"

Start-Process powershell -WorkingDirectory (Join-Path $Root "backend") -ArgumentList "-NoExit", "-Command", "dotnet run --urls http://127.0.0.1:43124"
if (-not (Test-Path (Join-Path $Root "frontend\node_modules"))) {
    Write-Host "Installing Angular packages (first run)..."
    Push-Location (Join-Path $Root "frontend")
    npm install
    Pop-Location
}
Start-Process powershell -WorkingDirectory (Join-Path $Root "frontend") -ArgumentList "-NoExit", "-Command", "npm start"

Write-Host "Two new windows opened for the API and the website. Leave them open while you work."
