# Infrastructure Deployment

This folder contains the Bicep template (`azure-resources.bicep`) and a GitHub Actions workflow (`deploy-bicep.yml`) to automate the deployment of Azure resources required for the Circuit Breaker Pattern on Azure.

## 📋 Prerequisites

- **Azure Subscription**: You must have an active Azure subscription.
- **Azure CLI**: Install the Azure CLI tool on your local machine.
- **GitHub Secrets**: Add the necessary secrets in your GitHub repository settings.

## 🔧 Deployment Steps

1. **Configure GitHub Secrets**

   Add the following secrets to your GitHub repository:

   - `AZURE_CLIENT_ID`: Your Azure service principal client ID.
   - `AZURE_CLIENT_SECRET`: Your Azure service principal client secret.
   - `AZURE_TENANT_ID`: Your Azure tenant ID.
   - `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID.

2. **Deploy Infrastructure**

   The deployment is automated using GitHub Actions. To deploy the infrastructure:

   - **Trigger the GitHub Action**: Push changes to the `main` branch or manually trigger the `deploy-bicep.yml` workflow.

3. **Resources Deployed**

   The following Azure resources will be deployed:

   - Azure Storage Account
   - Azure App Service Plan
   - Azure API App Service
   - Azure Functions (`Retry` and `Fallback`)
   - Azure Static Web App
   - Azure Cache for Redis
   - Azure SQL Server and Database
   - Application Insights
   - Azure Front Door or API Gateway

## ⚙️ Configuration

- Modify the Bicep template (`azure-resources.bicep`) as needed to customize the resource configurations.

## 📘 Usage

- Once deployed, the resources are ready for use. Refer to the respective README files in each component folder for further instructions.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.
