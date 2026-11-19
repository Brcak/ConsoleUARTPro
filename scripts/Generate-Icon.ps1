$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$encoded = (Get-Content (Join-Path $repo 'assets/console-uart-pro-icon-48.png.base64') -Raw).Trim()
$png = [Convert]::FromBase64String($encoded)
$iconPath = Join-Path $repo 'src/ConsoleUARTPro/ConsoleUARTPro.ico'
$fs = [System.IO.File]::Create($iconPath)
try {
  $w = [System.IO.BinaryWriter]::new($fs)
  try {
    $w.Write([UInt16]0)
    $w.Write([UInt16]1)
    $w.Write([UInt16]1)
    $w.Write([byte]48)
    $w.Write([byte]48)
    $w.Write([byte]0)
    $w.Write([byte]0)
    $w.Write([UInt16]1)
    $w.Write([UInt16]32)
    $w.Write([UInt32]$png.Length)
    $w.Write([UInt32]22)
    $w.Write($png)
  } finally { $w.Dispose() }
} finally { $fs.Dispose() }
Write-Host "Created Windows icon: $iconPath"
