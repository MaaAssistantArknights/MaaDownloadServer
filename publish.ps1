Set-Location -Path ./MaaDownloadServer.Build

$CakeBuildArgs = ""

foreach ($arg in $args) {
  $CakeBuildArgs = $CakeBuildArgs + " " + $arg
}

Write-Output "Running Restore..."
dotnet restore

Write-Output "Running Cake build process with args: $CakeBuildArgs"
dotnet run --no-restore -- $CakeBuildArgs
