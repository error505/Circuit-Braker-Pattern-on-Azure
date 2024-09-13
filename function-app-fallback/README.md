# Fallback Azure Function

This folder contains the code for the Fallback Azure Function, which provides cached or alternative data when the main services are unavailable.

## 📋 Prerequisites

- **Azure Subscription**: You must have an active Azure subscription.
- **GitHub Secrets**: Ensure that the necessary secrets are added to your GitHub repository.

## 🔧 Deployment Instructions

1. **Configure GitHub Secrets**

   Add the following secrets to your GitHub repository:

   - `AZURE_FUNCTIONAPP_FALLBACK_NAME`: The name of your Azure Function App for Fallback.
   - `AZURE_FUNCTIONAPP_FALLBACK_PUBLISH_PROFILE`: The publish profile for your Fallback Function App.

2. **Deploy Fallback Function App**

   The deployment is automated using GitHub Actions. To deploy the Fallback Function App:

   - **Trigger the GitHub Action**: Push changes to the `main` branch or manually trigger the `deploy-fallback-function.yml` workflow.

## ⚙️ Configuration

- **Environment Variables**:

  Ensure that the following environment variables are set in the Azure portal for your Function App:

  - `AzureWebJobsStorage`: Connection string for Azure Storage.
  - `REDIS_CONNECTION_STRING`: Connection string for Azure Cache for Redis.

## 📘 Usage

- The Fallback Function is triggered by an HTTP POST request to the `/api/fallback-operation` endpoint.
- It returns cached data or a default response if no cached data is available.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.
