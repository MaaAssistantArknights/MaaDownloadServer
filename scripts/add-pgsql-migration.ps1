[CmdletBinding()]
Param(
  [Parameter(Position = 0, Mandatory = $false)]
  [string]
  $MigrationName
)

$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$EfCoreOperatorScript = [System.IO.Path]::Combine($PSScriptRoot, "ef-core-operator.ps1")

pwsh $EfCoreOperatorScript add Postgres MaaPgSqlDbContext $MigrationName
