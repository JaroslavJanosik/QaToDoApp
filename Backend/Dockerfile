FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /app

COPY QaToDoApp.sln ./QaToDoApp.sln
COPY QaToDoApp/QaToDoApp.csproj ./QaToDoApp/QaToDoApp.csproj
COPY QaToDoAppIntegrationTests/QaToDoAppIntegrationTests.csproj ./QaToDoAppIntegrationTests/QaToDoAppIntegrationTests.csproj
COPY QaToDoAppUnitTests/QaToDoAppUnitTests.csproj ./QaToDoAppUnitTests/QaToDoAppUnitTests.csproj
RUN dotnet restore

COPY . ./
RUN dotnet test -c Release
RUN dotnet publish QaToDoApp -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "QaToDoApp.dll"]