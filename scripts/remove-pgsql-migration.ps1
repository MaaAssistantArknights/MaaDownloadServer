$PSScriptRoot = Split-Path $MyInvocation.MyCommand.Path -Parent
$EfCoreOperatorScript = [System.IO.Path]::Combine($PSScriptRoot, "ef-core-operator.ps1")

pwsh $EfCoreOperatorScript remove Postgres MaaPgSqlDbContext
