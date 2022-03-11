#!/bin/bash

cd MaaDownloadServer.Build || exit

CakeBuildArgs=""
i=1

while [ -n "${!i}" ]; do
  CakeBuildArgs="$CakeBuildArgs ${!i}"
  i=$((i + 1))
done

echo "Running Restore..."
dotnet restore

echo "Running Cake build process with args: $CakeBuildArgs"
dotnet run --no-restore -- "$CakeBuildArgs"

echo "Finished!"
cd .. || exit
