# start.ps1 - launches backend + frontend and opens browser

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$ApiPath = Join-Path $Root "src\Congratulator.Api"
$ClientPath = Join-Path $Root "client"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Pozdravlyator - starting up" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- Check tools ---

Write-Host "Checking dotnet... " -NoNewline
try {
    $v = & dotnet --version 2>&1
    Write-Host "OK ($v)" -ForegroundColor Green
} catch {
    Write-Host "NOT FOUND" -ForegroundColor Red
    Write-Host "Install .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
    pause; exit 1
}

Write-Host "Checking node... " -NoNewline
try {
    $v = & node --version 2>&1
    Write-Host "OK ($v)" -ForegroundColor Green
} catch {
    Write-Host "NOT FOUND" -ForegroundColor Red
    Write-Host "Install Node.js: https://nodejs.org"
    pause; exit 1
}

# --- Kill old processes on ports 5080 / 5173 ---

Write-Host "Freeing ports 5080 and 5173... " -NoNewline
foreach ($port in @(5080, 5173)) {
    $found = netstat -ano 2>$null |
        Select-String ":$port\s" |
        ForEach-Object { ($_ -split '\s+')[-1] } |
        Sort-Object -Unique
    foreach ($p in $found) {
        if ($p -match '^\d+$' -and $p -ne '0') {
            Stop-Process -Id $p -Force -ErrorAction SilentlyContinue
        }
    }
}
Write-Host "done" -ForegroundColor Green

# --- npm install if node_modules missing ---

if (-not (Test-Path (Join-Path $ClientPath "node_modules"))) {
    Write-Host ""
    Write-Host "Running npm install (first time only)..." -ForegroundColor Yellow
    Push-Location $ClientPath
    & npm install
    Pop-Location
}

# --- Start backend in a new window ---

Write-Host ""
Write-Host "Starting backend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$ApiPath'; Write-Host 'BACKEND - http://localhost:5080' -ForegroundColor Cyan; dotnet run"
)

# --- Wait for backend ---

Write-Host "Waiting for backend on http://localhost:5080 ." -NoNewline
$ready = $false
for ($i = 0; $i -lt 30; $i++) {
    Start-Sleep -Seconds 2
    Write-Host "." -NoNewline
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:5080/swagger/index.html" -TimeoutSec 2 -ErrorAction Stop
        if ($r.StatusCode -eq 200) { $ready = $true; break }
    } catch { }
}
if ($ready) { Write-Host " ready!" -ForegroundColor Green }
else { Write-Host " still starting, continuing anyway..." -ForegroundColor Yellow }

# --- Start frontend in a new window ---

Write-Host "Starting frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList @(
    "-NoExit", "-Command",
    "cd '$ClientPath'; Write-Host 'FRONTEND - http://localhost:5173' -ForegroundColor Magenta; npm run dev"
)

# --- Wait for frontend ---

Write-Host "Waiting for frontend on http://localhost:5173 ." -NoNewline
$ready = $false
for ($i = 0; $i -lt 20; $i++) {
    Start-Sleep -Seconds 2
    Write-Host "." -NoNewline
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:5173" -TimeoutSec 2 -ErrorAction Stop
        if ($r.StatusCode -eq 200) { $ready = $true; break }
    } catch { }
}
if ($ready) { Write-Host " ready!" -ForegroundColor Green }
else { Write-Host " still starting..." -ForegroundColor Yellow }

# --- Open browser ---

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  App:     http://localhost:5173" -ForegroundColor White
Write-Host "  Swagger: http://localhost:5080/swagger" -ForegroundColor White
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Start-Sleep -Seconds 2
Start-Process "http://localhost:5173"

Write-Host "Done! Close the two PowerShell windows to stop the servers." -ForegroundColor Green
Write-Host ""
pause
