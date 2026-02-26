# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy project files and restore as distinct layers
COPY QaToDoApp.sln ./
COPY QaToDoApp/QaToDoApp.csproj ./QaToDoApp/
COPY QaToDoAppIntegrationTests/QaToDoAppIntegrationTests.csproj ./QaToDoAppIntegrationTests/
COPY QaToDoAppUnitTests/QaToDoAppUnitTests.csproj ./QaToDoAppUnitTests/
RUN dotnet restore

# Copy the rest of the code
COPY . ./

# Run tests
RUN dotnet test -c Release

# Publish the app
RUN dotnet publish QaToDoApp -c Release -o /app/out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Tell the app to listen on port 80
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80

ENTRYPOINT ["dotnet", "QaToDoApp.dll"]