# Official Playwright .NET image, pinned to the same version as the Microsoft.Playwright
# package so the bundled browsers match the driver. It already contains the browsers and
# their OS dependencies.
FROM mcr.microsoft.com/playwright/dotnet:v1.64.1-noble

# Make sure the .NET 10 runtime the project targets is present (the install script
# skips it if the image already has it).
RUN curl -sSL https://dot.net/v1/dotnet-install.sh \
      | bash -s -- --runtime dotnet --channel 10.0 --install-dir /usr/share/dotnet

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

WORKDIR /src
COPY PlaywrightTests.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet build --no-restore -c Release

ENTRYPOINT ["dotnet", "test", "--no-build", "-c", "Release"]

# Usage:
#   docker build -t pw-dotnet .
#   docker run --rm pw-dotnet                                  # full suite
#   docker run --rm pw-dotnet --filter "Category=Smoke"        # extra args go to dotnet test
#   docker run --rm -e TEST_Ui__Browser=firefox -v "$PWD/TestResults:/src/TestResults" pw-dotnet
