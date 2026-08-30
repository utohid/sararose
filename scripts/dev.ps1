# Windows (PowerShell) local run: MySQL, API, then Angular.
$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot
Set-Location $Root

function Require-Command($Name, $Hint) {
    if (-not (Get-Command $Name -ErrorAction SilentlyContinue)) {
        throw "$Name was not found. $Hint"
    }
}

Require-Command dotnet "Install the .NET 8 SDK from https://dotnet.microsoft.com/download"
Require-Command npm "Install Node.js 20 LTS from https://nodejs.org/"

Write-Host "Site:  http://127.0.0.1:43123"
Write-Host "API:   http://127.0.0.1:43124"
Write-Host "Swagger: http://127.0.0.1:43124/swagger"
Write-Host "Ensure MySQL is running and the sararose database exists (see README)."

Start-Process powershell -WorkingDirectory (Join-Path $Root "backend") -ArgumentList "-NoExit", "-Command", "dotnet run --urls http://127.0.0.1:43124"
if (-not (Test-Path (Join-Path $Root "frontend\node_modules"))) {
    Write-Host "Installing Angular packages (first run)..."
    Push-Location (Join-Path $Root "frontend")
    npm install
    Pop-Location
}
Start-Process powershell -WorkingDirectory (Join-Path $Root "frontend") -ArgumentList "-NoExit", "-Command", "npm start"

Write-Host "Two new windows opened for the API and the website. Leave them open while you work."
