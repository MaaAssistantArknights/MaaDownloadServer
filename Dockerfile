FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY ./publish/release /app/
RUN ["apt", "update"]
RUN ["apt", "install", "-y", "python3", "python3-pip", "python3-venv"]
RUN ["./MaaDownloadServer"]
