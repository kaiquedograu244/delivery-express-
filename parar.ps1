$ErrorActionPreference = "Stop"
$root = Join-Path $env:LOCALAPPDATA "DeliveryExpress"
$mysqladmin = Get-ChildItem -Path (Join-Path $root "server") -Filter mysqladmin.exe -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $mysqladmin) {
    Write-Host "MySQL nao encontrado."
    exit 1
}
& $mysqladmin.FullName --protocol=TCP -h 127.0.0.1 -P 3306 -u delivery "-pdelivery123" shutdown
Write-Host "MySQL encerrado. Feche a janela do site se ela ainda estiver aberta."
