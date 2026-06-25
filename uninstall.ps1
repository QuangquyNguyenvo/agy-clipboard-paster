# agy-clipboard-paster uninstaller script for Windows
Write-Host "Uninstalling agy-clipboard-paster wrapper..." -ForegroundColor Cyan

# Find agy.exe path
$defaultPath = Join-Path $env:USERPROFILE "AppData\Local\agy\bin\agy.exe"
$agyDir = Split-Path $defaultPath
$realAgyPath = Join-Path $agyDir "agy_real.exe"

if (Test-Path $realAgyPath) {
    if (Test-Path $defaultPath) {
        Remove-Item -Path $defaultPath -Force
    }
    Rename-Item -Path $realAgyPath -NewName "agy.exe" -Force
    Write-Host "Restored original agy.exe successfully." -ForegroundColor Green
} else {
    Write-Host "Wrapper not found or already uninstalled." -ForegroundColor Yellow
}
