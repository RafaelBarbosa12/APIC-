# Variables
$acrName = "recommendationapi" + (Get-Random -Maximum 999999)
$resourceGroup = "recommendation-api-rg"
$location = "eastus"

# Create Azure Container Registry
Write-Host "Creating Azure Container Registry..."
az acr create --resource-group $resourceGroup --name $acrName --sku Basic --admin-enabled true

# Get ACR credentials
$acrLoginServer = az acr show --name $acrName --resource-group $resourceGroup --query loginServer -o tsv
$acrPassword = az acr credential show --name $acrName --resource-group $resourceGroup --query "passwords[0].value" -o tsv

# Login to ACR
Write-Host "Logging in to ACR..."
az acr login --name $acrName

# Tag the image
Write-Host "Tagging image..."
docker tag recommendation-api:optimized "$acrLoginServer/recommendation-api:latest"

# Push the image
Write-Host "Pushing image to ACR..."
docker push "$acrLoginServer/recommendation-api:latest"

# Output the image name for use in deployment
Write-Host "Image pushed successfully!"
Write-Host "Full image name: $acrLoginServer/recommendation-api:latest"
Write-Host "Please update the deployment script with this image name." 