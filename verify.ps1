[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [string]$MSBuildPath,
    [switch]$BuildOnly
)
$ErrorActionPreference = 'Stop'
if ($env:OS -ne 'Windows_NT') { throw 'WPF requires Windows. Run this script in Windows PowerShell.' }
if (-not $MSBuildPath) {
    $existing = Get-Command msbuild.exe -ErrorAction SilentlyContinue
    if ($existing) { $MSBuildPath = $existing.Source }
    else {
        $vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
        if (Test-Path $vswhere) {
            $MSBuildPath = & $vswhere -latest -products '*' -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
        }
    }
}
if (-not $MSBuildPath -or -not (Test-Path $MSBuildPath)) {
    throw 'MSBuild not found. Install Visual Studio 2019/2022 .NET desktop development, including the .NET Framework 4.8 targeting pack, or pass -MSBuildPath.'
}
$output = Join-Path $PSScriptRoot 'artifacts'
New-Item -ItemType Directory -Path $output -Force | Out-Null
Push-Location $PSScriptRoot
try {
    & $MSBuildPath 'Industrial.UI.Wpf.sln' /t:Rebuild /m /nologo "/p:Configuration=$Configuration" '/p:Platform=Any CPU' 2>&1 | Tee-Object -FilePath (Join-Path $output 'build.log')
    if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE. See artifacts\build.log." }
    if (-not $BuildOnly) {
        $smoke = Join-Path $PSScriptRoot "tests\Industrial.UI.Wpf.Smoke\bin\$Configuration\Industrial.UI.Wpf.Smoke.exe"
        & $smoke $output
        if ($LASTEXITCODE -ne 0) { throw 'WPF verification failed. See artifacts\windows-verification.txt.' }
    }
    Write-Host "Build complete. Demo: samples\Industrial.UI.Wpf.Demo\bin\$Configuration\Industrial.UI.Wpf.Demo.exe"
}
finally { Pop-Location }
