$ErrorActionPreference = "Stop"
Set-Location $PSScriptRoot

$root = Join-Path $env:LOCALAPPDATA "DeliveryExpress"
$serverRoot = Join-Path $root "server"
$datadir = Join-Path $root "data"
$ini = Join-Path $root "my.ini"
New-Item -ItemType Directory -Force -Path $root | Out-Null

function Test-Port([int]$Port) {
    $client = New-Object System.Net.Sockets.TcpClient
    try {
        $iar = $client.BeginConnect("127.0.0.1", $Port, $null, $null)
        if (-not $iar.AsyncWaitHandle.WaitOne(1000, $false)) { return $false }
        $client.EndConnect($iar)
        return $true
    } catch {
        return $false
    } finally {
        $client.Dispose()
    }
}

function Find-DotNet {
    $candidates = @(
        (Join-Path $env:USERPROFILE ".dotnet\dotnet.exe"),
        (Join-Path ${env:ProgramFiles} "dotnet\dotnet.exe")
    )
    foreach ($exe in $candidates) {
        if (-not (Test-Path $exe)) { continue }
        $sdks = & $exe --list-sdks 2>$null
        if ($LASTEXITCODE -eq 0 -and $sdks) { return $exe }
    }
    return $null
}

$mysqld = Get-ChildItem -Path $serverRoot -Filter mysqld.exe -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $mysqld) {
    Write-Host "MySQL nao encontrado em $serverRoot"
    exit 1
}

$base = $mysqld.Directory.Parent.FullName
$iniText = @"
[mysqld]
port=3306
bind-address=127.0.0.1
basedir=$($base -replace '\\','/')
datadir=$($datadir -replace '\\','/')
character-set-server=utf8mb4
collation-server=utf8mb4_unicode_ci
[client]
port=3306
"@
Set-Content -Path $ini -Value $iniText -Encoding ASCII

if (-not (Test-Path (Join-Path $datadir "mysql"))) {
    Write-Host "Inicializando o MySQL..."
    New-Item -ItemType Directory -Force -Path $datadir | Out-Null
    & $mysqld.FullName --defaults-file="$ini" --initialize-insecure
    if ($LASTEXITCODE -ne 0) { throw "Falha ao inicializar o MySQL." }
}

if (-not (Test-Port 3306)) {
    Write-Host "Iniciando o MySQL..."
    Start-Process -FilePath $mysqld.FullName -ArgumentList "--defaults-file=$ini" -WindowStyle Hidden
    $ready = $false
    for ($i = 0; $i -lt 60; $i++) {
        Start-Sleep -Milliseconds 500
        if (Test-Port 3306) { $ready = $true; break }
    }
    if (-not $ready) { throw "MySQL nao abriu a porta 3306. Veja o log em $datadir" }
}

$mysql = Join-Path $mysqld.DirectoryName "mysql.exe"
$sql = @"
CREATE USER IF NOT EXISTS 'delivery'@'127.0.0.1' IDENTIFIED BY 'delivery123';
ALTER USER 'delivery'@'127.0.0.1' IDENTIFIED BY 'delivery123';
CREATE USER IF NOT EXISTS 'delivery'@'localhost' IDENTIFIED BY 'delivery123';
ALTER USER 'delivery'@'localhost' IDENTIFIED BY 'delivery123';
GRANT ALL PRIVILEGES ON *.* TO 'delivery'@'127.0.0.1' WITH GRANT OPTION;
GRANT ALL PRIVILEGES ON *.* TO 'delivery'@'localhost' WITH GRANT OPTION;
FLUSH PRIVILEGES;
"@
$sqlFile = Join-Path $root "bootstrap.sql"
Set-Content -Path $sqlFile -Value $sql -Encoding ASCII

& $mysql --protocol=TCP -h 127.0.0.1 -P 3306 -u root --skip-password --connect-expired-password -e "source $($sqlFile -replace '\\','/')"
if ($LASTEXITCODE -ne 0) {
    & $mysql -u root --skip-password --connect-expired-password -e "source $($sqlFile -replace '\\','/')"
}
if ($LASTEXITCODE -ne 0) {
    & $mysql --protocol=TCP -h 127.0.0.1 -P 3306 -u root "-pdelivery123" -e "source $($sqlFile -replace '\\','/')"
}
if ($LASTEXITCODE -ne 0) { throw "Nao foi possivel preparar o usuario delivery no MySQL." }

$dotnet = Find-DotNet
if (-not $dotnet) { throw "SDK do .NET nao encontrado." }

$env:DOTNET_ROOT = Split-Path $dotnet
$env:PATH = "$(Split-Path $dotnet);$env:PATH"
$env:ASPNETCORE_ENVIRONMENT = "Development"
Write-Host ""
Write-Host "Site: http://localhost:5088"
Write-Host "PIN da cozinha: cozinha123"
Write-Host "Feche esta janela para parar o site. O MySQL continua ligado."
Write-Host ""
Start-Process powershell -ArgumentList "-NoProfile","-WindowStyle","Hidden","-Command","Start-Sleep -Seconds 4; Start-Process 'http://localhost:5088'" -WindowStyle Hidden
& $dotnet run --urls "http://localhost:5088" --no-launch-profile
