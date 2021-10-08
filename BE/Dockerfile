FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

COPY QaToDoApp.sln ./QaToDoApp.sln
COPY QaToDoApp/QaToDoApp.csproj ./QaToDoApp/QaToDoApp.csproj
COPY QaToDoAppIntegrationTests/QaToDoAppIntegrationTests.csproj ./QaToDoAppIntegrationTests/QaToDoAppIntegrationTests.csproj
COPY QaToDoAppUnitTests/QaToDoAppUnitTests.csproj ./QaToDoAppUnitTests/QaToDoAppUnitTests.csproj
RUN dotnet restore

COPY ./ ./
RUN dotnet test QaToDoAppUnitTests -c Release
RUN dotnet test QaToDoAppIntegrationTests -c Release
RUN dotnet publish QaToDoApp -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "QaToDoApp.dll"]