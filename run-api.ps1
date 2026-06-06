# Stops old instances, then starts the API.
Get-Process -Name "TaskApi" -ErrorAction SilentlyContinue | Stop-Process -Force
Set-Location "$PSScriptRoot\TaskApi"
Write-Host "Starting TaskApi at https://localhost:7154/api/tasks"
Write-Host "Press Ctrl+C to stop."
dotnet run --launch-profile https