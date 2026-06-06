# Stops any running TaskApi instances so Visual Studio can rebuild.
Get-Process -Name "TaskApi" -ErrorAction SilentlyContinue | Stop-Process -Force
Write-Host "TaskApi stopped. You can now build/run in Visual Studio."