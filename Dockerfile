FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /test-env

COPY ./ ./

WORKDIR /test-env/SQLDaosPackage.Test

RUN dotnet tool restore

RUN dotnet restore

#dotnet test -s ./.runsettings
#dotnet reportgenerator -reports:"**/*.cobertura.xml" -targetdir:./TestResults