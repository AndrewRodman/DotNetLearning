Get-Process -Name "TaskApi" -ErrorAction SilentlyContinue | Stop-Process -Force
Remove-Item "$PSScriptRoot\TaskApi\tasks.db*" -Force -ErrorAction SilentlyContinue
Write-Host "Database deleted. Run TaskApi (F5), then Register, then Login."