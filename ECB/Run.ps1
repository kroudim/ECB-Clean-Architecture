# PowerShell script to dynamically run ECB.API and ECB.Gateway.Service projects on specific ports

# Get the script directory (assumed to be solution root)
$root = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Find the csproj files
$apiProj = Get-ChildItem -Path $root -Recurse -Filter *ECB.Api.csproj | Select-Object -First 1
$serviceProj = Get-ChildItem -Path $root -Recurse -Filter *ECB.Infrastructure.Gateway.Services.csproj | Select-Object -First 1

if (-not $apiProj) {
    Write-Error "ECB.Api project (*.ECB.Api.csproj) not found."
    exit 1
}
if (-not $serviceProj) {
    Write-Error "ECB.Infrastructure.Gateway.Services project (*.ECB.Infrastructure.Gateway.Services.csproj) not found."
    exit 1
}

# Start API on desired port
Start-Process "dotnet" "run --project `"$($apiProj.FullName)`" --urls `"https://localhost:7169`""
Start-Sleep -Seconds 10
# Start WebApp on desired port
Start-Process "dotnet" "run --project `"$($serviceProj.FullName)`""

Write-Host "Both API and Service have been started in separate windows."
Write-Host "Press any key to exit this script (the applications will keep running)..."
[void][System.Console]::ReadKey($true)