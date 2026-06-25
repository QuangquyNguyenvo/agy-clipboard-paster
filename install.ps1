# agy-clipboard-paster installer script for Windows
Write-Host "Installing agy-clipboard-paster native C# wrapper..." -ForegroundColor Cyan

# 1. Find agy.exe
$agyPath = (Get-Command agy -ErrorAction SilentlyContinue).Source
if (-not $agyPath) {
    $defaultPath = Join-Path $env:USERPROFILE "AppData\Local\agy\bin\agy.exe"
    if (Test-Path $defaultPath) {
        $agyPath = $defaultPath
    }
}

if (-not $agyPath) {
    Write-Error "Could not find agy.exe installation path. Please make sure Antigravity CLI is installed."
    Exit
}

$agyDir = Split-Path $agyPath
$realAgyPath = Join-Path $agyDir "agy_real.exe"

# 2. Backup original agy.exe
if (-not (Test-Path $realAgyPath)) {
    Rename-Item -Path $agyPath -NewName "agy_real.exe" -Force
    Write-Host "Backed up original agy.exe to agy_real.exe" -ForegroundColor Green
}

# 3. Use local C# source if available, otherwise download from GitHub
$tempCs = Join-Path $env:TEMP "agy_wrapper_temp.cs"
$useLocal = $false
if ($PSScriptRoot) {
    $localCs = Join-Path $PSScriptRoot "src\agy_wrapper.cs"
    if (Test-Path $localCs) {
        $useLocal = $true
    }
}

if ($useLocal) {
    Copy-Item -Path $localCs -Destination $tempCs -Force
    Write-Host "Using local C# wrapper source code..." -ForegroundColor Cyan
} else {
    $url = "https://raw.githubusercontent.com/QuangquyNguyenvo/agy-clipboard-paster/main/src/agy_wrapper.cs"
    Write-Host "Downloading C# wrapper source code..." -ForegroundColor Cyan
    try {
        # Enable TLS 1.2 for secure handshake in older PowerShell environments
        [Net.ServicePointManager]::SecurityProtocol = [Net.ServicePointManager]::SecurityProtocol -bor [Net.SecurityProtocolType]::Tls12
        Invoke-WebRequest -Uri $url -OutFile $tempCs -UseBasicParsing -ErrorAction Stop
    } catch {
        Write-Error "Failed to download C# wrapper source code from GitHub: $_"
        Exit
    }
}

if (-not (Test-Path $tempCs)) {
    Write-Error "Source file $tempCs could not be prepared."
    Exit
}

# 4. Compile C# wrapper
$cscCompiler = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $cscCompiler)) {
    Write-Error "C# compiler csc.exe not found on this system."
    Remove-Item -Path $tempCs -Force
    Exit
}

# Cleanup any older unlocked agy_old_*.exe files
try {
    Get-ChildItem -Path $agyDir -Filter "agy_old_*.exe" -ErrorAction SilentlyContinue | ForEach-Object {
        Remove-Item -Path $_.FullName -Force -ErrorAction SilentlyContinue
    }
} catch {}

# Rename running agy.exe to release lock
$tempOldPath = $null
if (Test-Path $agyPath) {
    try {
        $tempOldName = "agy_old_$(Get-Date -Format 'yyyyMMddHHmmss').exe"
        $tempOldPath = Join-Path $agyDir $tempOldName
        Rename-Item -Path $agyPath -NewName $tempOldName -Force -ErrorAction Stop
        Write-Host "Renamed running agy.exe to release process lock..." -ForegroundColor Cyan
    } catch {
        Write-Warning "Could not rename running agy.exe. Attempting direct overwrite..."
    }
}

Write-Host "Compiling wrapper..." -ForegroundColor Cyan
& $cscCompiler /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:"$agyPath" "$tempCs"

if ($LASTEXITCODE -ne 0) {
    Write-Error "Compilation failed with exit code $LASTEXITCODE"
    # Rollback rename if compile failed
    if ($tempOldPath -and (Test-Path $tempOldPath)) {
        Rename-Item -Path $tempOldPath -NewName "agy.exe" -Force -ErrorAction SilentlyContinue
    }
    Remove-Item -Path $tempCs -Force
    Exit
}

# Cleanup
Remove-Item -Path $tempCs -Force
Write-Host "Successfully installed agy-clipboard-paster! You can now use Alt+V in agy." -ForegroundColor Green
