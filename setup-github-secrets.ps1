# PowerShell script for GitHub Secrets Setup
# Run this after setting up Azure resources

Write-Host "GitHub Secrets Setup Anleitung" -ForegroundColor Green
Write-Host "===============================" -ForegroundColor Green
Write-Host ""

Write-Host "Gehe zu deinem GitHub Repository:" -ForegroundColor Yellow
Write-Host "https://github.com/elsinan/TrackYourThings" -ForegroundColor Cyan
Write-Host ""

Write-Host "Navigiere zu: Settings -> Secrets and variables -> Actions" -ForegroundColor Yellow
Write-Host ""

Write-Host "Erstelle folgende Repository Secrets:" -ForegroundColor Yellow
Write-Host ""

Write-Host "1. AZURE_CREDENTIALS" -ForegroundColor Cyan
Write-Host "   Wert: Service Principal JSON (siehe unten)" -ForegroundColor Gray
Write-Host ""

Write-Host "2. AZURE_RESOURCE_GROUP" -ForegroundColor Cyan
Write-Host "   Wert: trackyourthings-rg" -ForegroundColor Gray
Write-Host ""

Write-Host "3. AZURE_DB_SERVER_NAME" -ForegroundColor Cyan
Write-Host "   Wert: trackyourthings-db-server" -ForegroundColor Gray
Write-Host ""

Write-Host "4. AZURE_DB_PASSWORD" -ForegroundColor Cyan
Write-Host "   Wert: [Dein PostgreSQL Passwort]" -ForegroundColor Gray
Write-Host ""

Write-Host "==================================================" -ForegroundColor Yellow
Write-Host ""

Write-Host "AZURE_CREDENTIALS JSON:" -ForegroundColor Green
Write-Host ""

# Service Principal Info aus Azure CLI abrufen
$SUBSCRIPTION_ID = az account show --query id -o tsv
$TENANT_ID = az account show --query tenantId -o tsv

$jsonTemplate = @"
{
  "clientId": "[CLIENT_ID aus Service Principal]",
  "clientSecret": "[CLIENT_SECRET aus Service Principal]",
  "subscriptionId": "$SUBSCRIPTION_ID",
  "tenantId": "$TENANT_ID"
}
"@

Write-Host $jsonTemplate

Write-Host ""
Write-Host "HINWEIS:" -ForegroundColor Yellow
Write-Host "Die clientId und clientSecret findest du in der Ausgabe des" -ForegroundColor Gray
Write-Host "Service Principal Erstellungs-Befehls von vorhin." -ForegroundColor Gray
Write-Host ""

Write-Host "Um ein neues PostgreSQL Passwort zu setzen:" -ForegroundColor Yellow
Write-Host "az postgres flexible-server update --resource-group trackyourthings-rg --name trackyourthings-db-server --admin-password [NEUES_PASSWORT]" -ForegroundColor Gray
Write-Host ""

Write-Host "Nach dem Einrichten der Secrets kannst du das Deployment starten:" -ForegroundColor Green
Write-Host "- Push deinen Code nach main branch" -ForegroundColor Gray
Write-Host "- Oder gehe zu Actions -> Deploy to Azure Container Instances -> Run workflow" -ForegroundColor Gray
