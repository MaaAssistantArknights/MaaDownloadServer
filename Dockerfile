FROM alisaqaq/maa-download-server-runtime-environment
WORKDIR /app

COPY ./publish/release /app/

RUN ["mkdir", "/app/data"]

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:80
VOLUME /app/data
EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "MaaDownloadServer.dll"]
