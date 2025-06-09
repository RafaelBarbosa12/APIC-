# Login to Azure
Write-Host "Logging in to Azure..."
az login

# Variables
$resourceGroup = "recommendation-api-rg"
$location = "eastus"
$appServicePlan = "recommendation-api-plan"
$webAppName = "recommendation-api-" + (Get-Random -Maximum 999999)

# Create a resource group
Write-Host "Creating resource group..."
az group create --name $resourceGroup --location $location

# Create an App Service plan (Free tier - F1)
Write-Host "Creating App Service plan..."
az appservice plan create --name $appServicePlan --resource-group $resourceGroup --sku F1 --is-linux

# Create a web app
Write-Host "Creating web app..."
az webapp create --resource-group $resourceGroup --plan $appServicePlan --name $webAppName --runtime "DOTNETCORE:8.0" --deployment-container-image-name "recommendation-api:optimized"

# Configure the web app
Write-Host "Configuring web app..."
az webapp config set --resource-group $resourceGroup --name $webAppName --always-on false

# Set environment variables
Write-Host "Setting environment variables..."
az webapp config appsettings set --resource-group $resourceGroup --name $webAppName --settings PythonApi__BaseUrl="YOUR_PYTHON_SERVICE_URL"

# Enable Docker Container logging
Write-Host "Enabling container logging..."
az webapp log config --resource-group $resourceGroup --name $webAppName --docker-container-logging filesystem

Write-Host "Deployment completed!"
Write-Host "Your web app URL is: https://$webAppName.azurewebsites.net"
Write-Host "Note: You'll need to update the PythonApi__BaseUrl setting with your actual Python service URL" 