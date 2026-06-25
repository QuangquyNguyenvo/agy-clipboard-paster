# agy-clipboard-paster uninstaller script for Windows
Write-Host "Uninstalling agy-clipboard-paster wrapper..." -ForegroundColor Cyan

# Find agy.exe path
$defaultPath = Join-Path $env:USERPROFILE "AppData\Local\agy\bin\agy.exe"
$agyDir = Split-Path $defaultPath
$realAgyPath = Join-Path $agyDir "agy_real.exe"

if (Test-Path $realAgyPath) {
    $tempOldPath = $null
    if (Test-Path $defaultPath) {
        try {
            $tempOldName = "agy_old_wrapper_$(Get-Date -Format 'yyyyMMddHHmmss').exe"
            $tempOldPath = Join-Path $agyDir $tempOldName
            Rename-Item -Path $defaultPath -NewName $tempOldName -Force -ErrorAction Stop
            Write-Host "Renamed running agy.exe wrapper to release process lock..." -ForegroundColor Cyan
        } catch {
            Write-Warning "Could not rename running agy.exe wrapper. Attempting direct removal..."
            Remove-Item -Path $defaultPath -Force -ErrorAction SilentlyContinue
        }
    }
    Rename-Item -Path $realAgyPath -NewName "agy.exe" -Force
    Write-Host "Restored original agy.exe successfully." -ForegroundColor Green

    # Cleanup renamed wrapper if possible
    if ($tempOldPath -and (Test-Path $tempOldPath)) {
        Remove-Item -Path $tempOldPath -Force -ErrorAction SilentlyContinue
    }
} else {
    Write-Host "Wrapper not found or already uninstalled." -ForegroundColor Yellow
}
