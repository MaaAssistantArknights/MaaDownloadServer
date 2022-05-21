[CmdletBinding()]
Param(
  [Parameter(Position = 0, Mandatory = $false)]
  [string]
  $OperationType,
  
  [Parameter(Position = 1, Mandatory = $false)]
  [string]
  $DatabaseType,

  [Parameter(Position = 2, Mandatory = $false)]
  [string]
  $ContextName,

  [Parameter(Position = 3, Mandatory = $false)]
  [string]
  $MigrationName
)

if ([System.String]::IsNullOrEmpty($DatabaseType) -eq $true -or [System.String]::IsNullOrEmpty($ContextName) -eq $true) {
  Write-Error "Database type and context name could not be empty"
  exit -1
}

if ($OperationType -eq "add") {
  if ([System.String]::IsNullOrEmpty($MigrationName))
  {
    Write-Error "Migration name could not be null"
    exit -1
  }
  Write-Output "Create new MaaDbContext migration for $($DatabaseType) database: $($MigrationName)"
}
elseif ($OperationType -eq "remove") {
  Write-Output "Remove the last migration"
}
else {
  exit -1
}

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$ContextProject = [System.IO.Path]::Combine($PSScriptRoot, "../src/Data/MaaDownloadServer.Data.Db.$($DatabaseType)")
$StartupProject = [System.IO.Path]::Combine($PSScriptRoot, "../src/Data/MaaDownloadServer.Data.Migrations")

if ([System.IO.Directory]::Exists($ContextProject) -eq $false -or [System.IO.Directory]::Exists($StartupProject) -eq $false) {
  Write-Error "Context or Startup project not found"
  exit -1
}

function ExecSafe([scriptblock] $cmd) {
  & $cmd
  if ($LASTEXITCODE) { exit $LASTEXITCODE }
}

ExecSafe { & dotnet --version }
if ($null -eq (Get-Command "dotnet" -ErrorAction SilentlyContinue) -or $LASTEXITCODE -ne 0) {
  Write-Error "Command dotnet is not exist"
  exit $LASTEXITCODE
}

ExecSafe { & dotnet ef --version }
if ($LASTEXITCODE -ne 0) {
  Write-Error "Command dotnet is not exist"
  exit $LASTEXITCODE
}

if ($OperationType -eq "add")
{
  ExecSafe { &  dotnet ef migrations add $MigrationName -p $ContextProject -s $StartupProject -c $ContextName }
}
else
{
  ExecSafe { &  dotnet ef migrations remove -p $ContextProject -s $StartupProject -c $ContextName -f }
}
