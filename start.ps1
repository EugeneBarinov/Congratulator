# start.ps1 — запускает backend и frontend, ждёт готовности, открывает браузер

$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$ApiPath = Join-Path $Root "src\Congratulator.Api"
$ClientPath = Join-Path $Root "client"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Поздравлятор — запуск" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# --- Проверка инструментов ---

Write-Host "Проверяю наличие dotnet..." -NoNewline
try {
    $dotnetVersion = & dotnet --version 2>&1
    Write-Host " OK ($dotnetVersion)" -ForegroundColor Green
} catch {
    Write-Host " НЕ НАЙДЕН" -ForegroundColor Red
    Write-Host "Установи .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    pause
    exit 1
}

Write-Host "Проверяю наличие node..." -NoNewline
try {
    $nodeVersion = & node --version 2>&1
    Write-Host " OK ($nodeVersion)" -ForegroundColor Green
} catch {
    Write-Host " НЕ НАЙДЕН" -ForegroundColor Red
    Write-Host "Установи Node.js: https://nodejs.org" -ForegroundColor Yellow
    pause
    exit 1
}

# --- Убиваем старые процессы на нужных портах ---

Write-Host ""
Write-Host "Освобождаю порты 5080 и 5173..." -NoNewline
$ports = @(5080, 5173)
foreach ($port in $ports) {
    $pids = netstat -ano 2>$null | Select-String ":$port\s" | ForEach-Object {
        ($_ -split '\s+')[-1]
    } | Sort-Object -Unique
    foreach ($p in $pids) {
        if ($p -match '^\d+$' -and $p -ne '0') {
            Stop-Process -Id $p -Force -ErrorAction SilentlyContinue
        }
    }
}
Write-Host " готово" -ForegroundColor Green

# --- npm install если нет node_modules ---

if (-not (Test-Path (Join-Path $ClientPath "node_modules"))) {
    Write-Host ""
    Write-Host "Устанавливаю npm-пакеты (первый раз, займёт минуту)..." -ForegroundColor Yellow
    Push-Location $ClientPath
    & npm install
    Pop-Location
}

# --- Запуск backend в отдельном окне ---

Write-Host ""
Write-Host "Запускаю backend (ASP.NET Core)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", `
    "cd '$ApiPath'; Write-Host 'BACKEND' -ForegroundColor Cyan; dotnet run"

# --- Ждём пока backend поднимется ---

Write-Host "Жду готовности backend на http://localhost:5080 ..." -NoNewline
$maxWait = 30
$waited = 0
$backendReady = $false

while ($waited -lt $maxWait) {
    Start-Sleep -Seconds 2
    $waited += 2
    Write-Host "." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5080/swagger/index.html" `
            -TimeoutSec 2 -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            $backendReady = $true
            break
        }
    } catch { }
}

if ($backendReady) {
    Write-Host " готов!" -ForegroundColor Green
} else {
    Write-Host " не ответил за $maxWait сек." -ForegroundColor Yellow
    Write-Host "Backend может ещё запускаться — продолжаю..." -ForegroundColor Yellow
}

# --- Запуск frontend в отдельном окне ---

Write-Host "Запускаю frontend (Vite)..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", `
    "cd '$ClientPath'; Write-Host 'FRONTEND' -ForegroundColor Magenta; npm run dev"

# --- Ждём пока frontend поднимется ---

Write-Host "Жду готовности frontend на http://localhost:5173 ..." -NoNewline
$waited = 0
$frontendReady = $false

while ($waited -lt 20) {
    Start-Sleep -Seconds 2
    $waited += 2
    Write-Host "." -NoNewline
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:5173" `
            -TimeoutSec 2 -ErrorAction Stop
        if ($response.StatusCode -eq 200) {
            $frontendReady = $true
            break
        }
    } catch { }
}

if ($frontendReady) {
    Write-Host " готов!" -ForegroundColor Green
} else {
    Write-Host " ещё запускается..." -ForegroundColor Yellow
}

# --- Открываем браузер ---

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Открываю браузер..." -ForegroundColor Cyan
Write-Host "  Приложение: http://localhost:5173" -ForegroundColor White
Write-Host "  Swagger API: http://localhost:5080/swagger" -ForegroundColor White
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

Start-Sleep -Seconds 2
Start-Process "http://localhost:5173"

Write-Host "Готово! Оба окна с backend и frontend работают отдельно." -ForegroundColor Green
Write-Host "Чтобы остановить — закрой оба открывшихся окна PowerShell." -ForegroundColor Gray
Write-Host ""
pause
