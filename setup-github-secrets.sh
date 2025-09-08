#!/bin/bash

# GitHub Secrets Setup for Azure Container Instances Deployment
# Run this script after executing setup-azure.sh

echo "🔐 GitHub Secrets Setup für Azure Container Instances"
echo "================================================="
echo ""

# Load variables from setup-azure.sh execution
if [ -f ".azure-deployment-info" ]; then
    source .azure-deployment-info
    echo "✅ Azure deployment info loaded"
else
    echo "❌ .azure-deployment-info file not found!"
    echo "Please run setup-azure.sh first."
    exit 1
fi

echo ""
echo "📋 Die folgenden GitHub Secrets müssen eingerichtet werden:"
echo ""

echo "1. AZURE_CREDENTIALS"
echo "   Value: (Service Principal JSON)"
echo ""

echo "2. AZURE_RESOURCE_GROUP"
echo "   Value: $RESOURCE_GROUP_NAME"
echo ""

echo "3. AZURE_DB_SERVER_NAME"
echo "   Value: $DB_SERVER_NAME"
echo ""

echo "4. AZURE_DB_PASSWORD"
echo "   Value: $DB_PASSWORD"
echo ""

echo "================================================="
echo ""

# Service Principal JSON für AZURE_CREDENTIALS
echo "🔑 Service Principal JSON für AZURE_CREDENTIALS Secret:"
echo ""
echo "{"
echo "  \"clientId\": \"$CLIENT_ID\","
echo "  \"clientSecret\": \"$CLIENT_SECRET\","
echo "  \"subscriptionId\": \"$(az account show --query id -o tsv)\","
echo "  \"tenantId\": \"$(az account show --query tenantId -o tsv)\""
echo "}"
echo ""

echo "================================================="
echo ""
echo "📝 Anleitung:"
echo ""
echo "1. Gehe zu deinem GitHub Repository"
echo "2. Settings → Secrets and variables → Actions"
echo "3. Klicke auf 'New repository secret'"
echo "4. Füge die oben genannten Secrets hinzu"
echo ""
echo "💡 Tipp: Kopiere die JSON-Ausgabe für AZURE_CREDENTIALS komplett"
echo "         (inklusive der geschweiften Klammern)"
echo ""

# Optionale interaktive Einrichtung
read -p "🤔 Möchtest du, dass ich versuche die Secrets automatisch einzurichten? (y/N): " -n 1 -r
echo ""

if [[ $REPLY =~ ^[Yy]$ ]]; then
    echo ""
    echo "🔧 Automatische Einrichtung der GitHub Secrets..."
    echo ""
    
    # GitHub CLI prüfen
    if ! command -v gh &> /dev/null; then
        echo "❌ GitHub CLI (gh) ist nicht installiert!"
        echo "   Installiere es mit: winget install GitHub.cli"
        echo "   Oder gehe manuell vor."
        exit 1
    fi
    
    # GitHub CLI Auth prüfen
    if ! gh auth status &> /dev/null; then
        echo "🔐 GitHub CLI Authentifizierung erforderlich..."
        gh auth login
    fi
    
    # Repository erkennen
    REPO=$(gh repo view --json nameWithOwner -q .nameWithOwner 2>/dev/null)
    if [ -z "$REPO" ]; then
        echo "❌ Konnte Repository nicht automatisch erkennen!"
        echo "   Stelle sicher, dass du im Repository-Ordner bist."
        exit 1
    fi
    
    echo "📁 Repository: $REPO"
    echo ""
    
    # Secrets einrichten
    echo "⚙️  Einrichten der Secrets..."
    
    # AZURE_CREDENTIALS
    AZURE_CREDENTIALS=$(cat <<EOF
{
  "clientId": "$CLIENT_ID",
  "clientSecret": "$CLIENT_SECRET",
  "subscriptionId": "$(az account show --query id -o tsv)",
  "tenantId": "$(az account show --query tenantId -o tsv)"
}
EOF
)
    
    echo "$AZURE_CREDENTIALS" | gh secret set AZURE_CREDENTIALS --repo $REPO
    echo "✅ AZURE_CREDENTIALS gesetzt"
    
    echo "$RESOURCE_GROUP_NAME" | gh secret set AZURE_RESOURCE_GROUP --repo $REPO
    echo "✅ AZURE_RESOURCE_GROUP gesetzt"
    
    echo "$DB_SERVER_NAME" | gh secret set AZURE_DB_SERVER_NAME --repo $REPO
    echo "✅ AZURE_DB_SERVER_NAME gesetzt"
    
    echo "$DB_PASSWORD" | gh secret set AZURE_DB_PASSWORD --repo $REPO
    echo "✅ AZURE_DB_PASSWORD gesetzt"
    
    echo ""
    echo "🎉 Alle GitHub Secrets wurden erfolgreich eingerichtet!"
    echo ""
    echo "🚀 Du kannst jetzt einen Push nach main machen oder die GitHub Action"
    echo "   manuell unter Actions → 'Deploy to Azure Container Instances' starten."
else
    echo ""
    echo "👍 OK, richte die Secrets manuell ein."
    echo "   Verwende die oben gezeigten Werte."
fi

echo ""
echo "================================================="
echo "🏁 Setup abgeschlossen!"
echo ""
echo "Nächste Schritte:"
echo "1. ✅ Azure Ressourcen erstellt (setup-azure.sh)"
echo "2. ✅ GitHub Actions Workflow erstellt"
echo "3. ✅ GitHub Secrets Information bereitgestellt"
echo "4. 📝 GitHub Secrets einrichten (manuell oder automatisch)"
echo "5. 🚀 Deployment durch Push nach main oder manuell starten"
