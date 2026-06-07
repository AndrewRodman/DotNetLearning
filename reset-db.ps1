Get-Process -Name "TaskApi" -ErrorAction SilentlyContinue | Stop-Process -Force

Push-Location "$PSScriptRoot\TaskApi"
try {
    dotnet ef database drop --force
}
finally {
    Pop-Location
}

Write-Host "Database dropped. Run TaskApi (F5) to recreate via migrations, then Register, then Login."