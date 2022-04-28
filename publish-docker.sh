#!/bin/bash

docker_arches="amd64,arm64,arm/v7"

if [ -z "$1" ]; then
  echo ">>>>> ERROR: Version could not be empty!"
  exit
fi

if [ -n "$2" ]; then
  docker_arches=$2
fi

echo ">>>>> Publish..."
/bin/bash ./publish.sh --docker=true --docker-arches="$docker_arches" --maads-version="$1"

IFS=','
read -ra arches <<<"$docker_arches"
for arch in "${arches[@]}"; do

  echo ""
  echo "Build Docker image with arch $arch"
  echo ""

  echo ">>>>> Copy $arch artifacts..."
  cp -r ./publish/net6.0-docker-"$arch"-Release ./publish/release

  echo ">>>>> Build $arch Docker image..."
  if [ "$arch" == "arm/v7" ]; then
    docker buildx build --load --platform linux/"$arch" -t maa-download-server:"$1"-arm-v7 .
  else
    docker buildx build --load --platform linux/"$arch" -t maa-download-server:"$1"-"$arch" .
  fi

  echo ">>>>> Remove $arch artifacts"
  rm -r ./publish/release
  rm -r ./publish/net6.0-docker-"$arch"-Release
done
