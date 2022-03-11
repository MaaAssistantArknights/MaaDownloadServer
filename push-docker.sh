#!/bin/bash

# ONLY FOR GITHUB ACTIONS - DO NOT USE IN LOCAL ENVIRONMENT

docker image tag maa-download-server:$1-amd64 $2/maa-download-server:$1-amd64
docker image tag maa-download-server:$1-arm64 $2/maa-download-server:$1-arm64
docker image tag maa-download-server:$1-arm-v7 $2/maa-download-server:$1-arm-v7
docker image push --all-tags $2/maa-download-server

docker manifest create $2/maa-download-server:$1 \
  $2/maa-download-server:$1-amd64 \
  $2/maa-download-server:$1-arm64 \
  $2/maa-download-server:$1-arm-v7

docker manifest push $2/maa-download-server:$1

docker manifest create --amend $2/maa-download-server:latest \
  $2/maa-download-server:$1-amd64 \
  $2/maa-download-server:$1-arm64 \
  $2/maa-download-server:$1-arm-v7

docker manifest push $2/maa-download-server:latest
