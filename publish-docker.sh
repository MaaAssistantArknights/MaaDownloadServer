#!/bin/bash

rid="linux-x64"

if [ -n $1 ]; then
  rid=$1
fi

echo "Publish..."
/bin/bash ./publish.sh --rid=$rid

echo "Copy artifacts..."
cp -r ./publish/net6.0-$rid-Release ./publish/release

echo "Build Docker image..."
docker build -t maa-download-server .
