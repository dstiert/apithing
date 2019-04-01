FROM microsoft/dotnet:sdk AS build-env
WORKDIR /app

COPY . ./
RUN dotnet restore

LABEL test=true
RUN dotnet test **/*.unittests.csproj /p:CollectCoverage=true /p:CoverletOutput=/app/coverage.xml /p:CoverletOutputFormat=opencover /p:Exclude="[*.unittests]*"

RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:aspnetcore-runtime
WORKDIR /app

ENV CORECLR_ENABLE_PROFILING=1 \
CORECLR_PROFILER={36032161-FFC0-4B61-B559-F6C5D41BAE5A} \
CORECLR_NEWRELIC_HOME=/usr/local/newrelic-netcore20-agent \
CORECLR_PROFILER_PATH=/usr/local/newrelic-netcore20-agent/libNewRelicProfiler.so

COPY ./newrelic ./newrelic
RUN dpkg -i ./newrelic/newrelic-netcore20-agent*.deb

COPY --from=build-env /app/apithing/out .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet apithing.dll