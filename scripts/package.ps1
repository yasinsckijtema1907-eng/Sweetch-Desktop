param([string]$Version = '1.0.0')
$ErrorActionPreference = 'Stop'
$root = Split-Path $PSScriptRoot -Parent
$publish = Join-Path $root 'publish'
if (!(Test-Path (Join-Path $publish 'SweetchDesktop.exe'))) { throw 'Build the application first.' }
$artifacts = Join-Path $root 'artifacts'
New-Item -ItemType Directory -Force $artifacts | Out-Null
$zip = Join-Path $artifacts "SweetchDesktop-$Version-win-x64.zip"
Compress-Archive -Path "$publish/*" -DestinationPath $zip -Force
$hash = (Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  $([IO.Path]::GetFileName($zip))" | Set-Content -Encoding ascii "$zip.sha256"
Write-Output $zip
