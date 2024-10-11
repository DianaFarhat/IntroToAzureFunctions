# Use the .NET 6 SDK to build the function app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS installer-env

# Set the working directory
WORKDIR /src/dotnet-function-app

# Copy the project files into the container
COPY . .

# Publish the Azure Function app to the output folder
RUN dotnet publish *.csproj --output /home/site/wwwroot

# Use the Azure Functions base image for .NET 6 (v4 runtime)
FROM mcr.microsoft.com/azure-functions/dotnet:4
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
AzureFunctionsJobHost__Logging__Console__IsEnabled=true

# Copy the published function app from the build stage
COPY --from=installer-env /home/site/wwwroot /home/site/wwwroot

