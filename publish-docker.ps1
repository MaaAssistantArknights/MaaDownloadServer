$DockerArches = "amd64,arm64,arm/v7"

if (!$args[0] || $args[0] -eq "") {
  Write-Output ">>>>> ERROR: Image tag could not be empty!"
  Break Script
}

$VersionTag = $args[0]

if ($args[1]) {
  if ($args[1] -ne "") {
    $DockerArches = $args[1]
  }
}

Write-Output ">>>>> Publish..."
pwsh ./publish.ps1 --docker=true --docker-arches=$DockerArches --maads-version=$VersionTag

$Arches = $DockerArches.Split(",")

foreach ($Arch in $Arches) {
  Write-Output ""
  Write-Output "Build Docker image with arch $arch"
  Write-Output ""

  Write-Output ">>>>> Copy $arch artifacts..."
  Copy-Item "./publish/net6.0-docker-$arch-Release" ./publish/release

  Write-Output ">>>>> Build $arch Docker image..."
  if ($Arch -eq "arm/v7") {
    docker buildx build --load --platform linux/"$arch" -t maa-download-server:"$VersionTag"-arm-v7 .
  }
  else {
    docker buildx build --load --platform linux/"$arch" -t maa-download-server:"$VersionTag"-"$arch" .
  }

  Write-Output ">>>>> Remove $arch artifacts"
  Remove-Item ./publish/release -Force -Recurse
  Remove-Item ./publish/net6.0-docker-"$arch"-Release -Force -Recurse
}
