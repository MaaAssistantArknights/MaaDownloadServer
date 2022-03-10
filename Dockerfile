FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app

COPY ./publish/release /app/

RUN ["apt", "update"]
RUN ["apt", "install", "-y", "python3", "python3-pip"]
RUN ["apt", "autoremove"]
RUN ["/usr/bin/python3", "-m", "pip", "install", "virtualenv"]

RUN ["mkdir", "/app/data"]

ENV DOTNET_RUNNING_IN_CONTAINER=true
ENV ASPNETCORE_URLS=http://+:80
VOLUME /app/data
EXPOSE 80/tcp

ENTRYPOINT ["dotnet", "MaaDownloadServer.dll"]
