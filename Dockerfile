FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet build --no-restore

RUN pwsh bin/Debug/net8.0/playwright.ps1 install --with-deps chromium

ENTRYPOINT ["dotnet", "test", "--no-build", "--settings", "PlaywrightTests.runsettings"]

# Usage:
#   docker build -t pw-dotnet .
#   docker run --rm pw-dotnet
#   docker run --rm pw-dotnet --filter "Category=smoke"
