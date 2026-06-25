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

# 3. Download agy_wrapper.cs source from GitHub
$url = "https://raw.githubusercontent.com/QuangquyNguyenvo/agy-clipboard-paster/main/src/agy_wrapper.cs"
$tempCs = Join-Path $env:TEMP "agy_wrapper_temp.cs"
Write-Host "Downloading C# wrapper source code..." -ForegroundColor Cyan
Invoke-WebRequest -Uri $url -OutFile $tempCs -UseBasicParsing

# 4. Compile C# wrapper
$cscCompiler = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe"
if (-not (Test-Path $cscCompiler)) {
    Write-Error "C# compiler csc.exe not found on this system."
    Remove-Item -Path $tempCs -Force
    Exit
}

Write-Host "Compiling wrapper..." -ForegroundColor Cyan
& $cscCompiler /r:System.Windows.Forms.dll /r:System.Drawing.dll /out:"$agyPath" "$tempCs"

# Cleanup
Remove-Item -Path $tempCs -Force
Write-Host "Successfully installed agy-clipboard-paster! You can now use Alt+V in agy." -ForegroundColor Green
