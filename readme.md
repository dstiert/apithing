[![Build status](https://img.shields.io/appveyor/ci/dstiert/apithing.svg)](https://ci.appveyor.com/project/dstiert/apithing/branch/master)
[![Coveralls github](https://img.shields.io/coveralls/github/dstiert/apithing.svg)](https://coveralls.io/github/dstiert/apithing)

## Requirements

* .NET Core SDK 2.2 [Download](https://dotnet.microsoft.com/download/dotnet-core/2.2)
* Docker - [Download](https://download.docker.com/win/stable/Docker%20for%20Windows%20Installer.exe)

## Running

### With Docker Compose

* `docker-compose up`
  * Service available at http://localhost:5000/
* `Ctrl-C` to shutdown containers

### Locally

* `docker run -p 6379:6379 --name apithing-redis -d redis`
* `dotnet run --project apithing/apithing.csproj`
  * Service available at http://localhost:5000/
