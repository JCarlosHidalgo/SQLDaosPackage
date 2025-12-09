FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /test-env

COPY ./SQLDaosPackage.Code/SQLDaosPackage.Code.csproj ./SQLDaosPackage.Code/
COPY ./SQLDaosPackage.Test/SQLDaosPackage.Test.csproj ./SQLDaosPackage.Test/
COPY ./SQLDaosPackage.Test/.config ./SQLDaosPackage.Test/

WORKDIR /test-env/SQLDaosPackage.Test

RUN dotnet tool restore

RUN dotnet restore

WORKDIR /test-env/

COPY ./ ./

WORKDIR /test-env/SQLDaosPackage.Test

#dotnet test -s ./.runsettings
#dotnet reportgenerator -reports:"**/*.cobertura.xml" -targetdir:./TestResults