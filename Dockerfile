FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore

LABEL test=true
RUN dotnet test **/*.unittests.csproj /p:CollectCoverage=true /p:CoverletOutput=/app/coverage.xml /p:CoverletOutputFormat=opencover /p:Exclude="[*.unittests]*"

RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/apithing/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet apithing.dll