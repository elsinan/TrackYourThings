#!/bin/bash

# Azure Setup für TrackYourThings mit Container Instances
echo "🔷 Setting up Azure resources for TrackYourThings..."

# Überprüfen ob Azure CLI installiert ist
if ! command -v az &> /dev/null; then
    echo "❌ Azure CLI is not installed. Please install it first:"
    echo "https://docs.microsoft.com/en-us/cli/azure/install-azure-cli"
    exit 1
fi

# Login zu Azure
echo "🔐 Logging in to Azure..."
az login

# Variablen
RESOURCE_GROUP="trackyourthings-rg"
LOCATION="westeurope"
DB_SERVER_NAME="trackyourthings-db-$(date +%s)"
DB_PASSWORD=$(openssl rand -base64 32 | tr -d "=+/" | cut -c1-25)

echo "📋 Configuration:"
echo "Resource Group: $RESOURCE_GROUP"
echo "Location: $LOCATION"
echo "DB Server: $DB_SERVER_NAME"
echo "DB Password: $DB_PASSWORD"

# Resource Group erstellen
echo "🏗️  Creating Resource Group..."
az group create \
  --name $RESOURCE_GROUP \
  --location $LOCATION

# PostgreSQL Flexible Server erstellen
echo "🐘 Creating PostgreSQL Flexible Server..."
az postgres flexible-server create \
  --resource-group $RESOURCE_GROUP \
  --name $DB_SERVER_NAME \
  --location $LOCATION \
  --admin-user postgres \
  --admin-password $DB_PASSWORD \
  --sku-name Standard_B1ms \
  --tier Burstable \
  --version 16 \
  --storage-size 32 \
  --public-access 0.0.0.0 \
  --yes

# Datenbank erstellen
echo "💾 Creating database..."
az postgres flexible-server db create \
  --resource-group $RESOURCE_GROUP \
  --server-name $DB_SERVER_NAME \
  --database-name trackyourthings

# Service Principal für GitHub Actions erstellen
echo "🔑 Creating Service Principal for GitHub Actions..."
AZURE_CREDENTIALS=$(az ad sp create-for-rbac \
  --name "TrackYourThings-GitHub-Actions" \
  --role contributor \
  --scopes /subscriptions/$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP \
  --sdk-auth)

echo ""
echo "✅ Setup completed successfully!"
echo ""
echo "📋 Add these secrets to your GitHub repository settings:"
echo "   Go to: https://github.com/elsinan/TrackYourThings/settings/secrets/actions"
echo ""
echo "Secret: AZURE_CREDENTIALS"
echo "Value:"
echo "$AZURE_CREDENTIALS"
echo ""
echo "Secret: AZURE_RESOURCE_GROUP"
echo "Value: $RESOURCE_GROUP"
echo ""
echo "Secret: AZURE_DB_SERVER_NAME"
echo "Value: $DB_SERVER_NAME"
echo ""
echo "Secret: AZURE_DB_PASSWORD"
echo "Value: $DB_PASSWORD"
echo ""
echo "🚀 After adding the secrets, push to main branch to trigger deployment!"
echo ""
echo "📝 Save this information:"
echo "Resource Group: $RESOURCE_GROUP"
echo "DB Server: $DB_SERVER_NAME.postgres.database.azure.com"
echo "DB Password: $DB_PASSWORD"
