# Retry Azure Function

This folder contains the code for the Retry Azure Function, which handles normal operations such as read/write actions to the SQL Database and cache updates.

## 📋 Prerequisites

- **Azure Subscription**: You must have an active Azure subscription.
- **GitHub Secrets**: Ensure that the necessary secrets are added to your GitHub repository.

## 🔧 Deployment Instructions

1. **Configure GitHub Secrets**

   Add the following secrets to your GitHub repository:

   - `AZURE_FUNCTIONAPP_RETRY_NAME`: The name of your Azure Function App for Retry.
   - `AZURE_FUNCTIONAPP_RETRY_PUBLISH_PROFILE`: The publish profile for your Retry Function App.

2. **Deploy Retry Function App**

   The deployment is automated using GitHub Actions. To deploy the Retry Function App:

   - **Trigger the GitHub Action**: Push changes to the `main` branch or manually trigger the `deploy-retry-function.yml` workflow.

## ⚙️ Configuration

- **Environment Variables**:

  Ensure that the following environment variables are set in the Azure portal for your Function App:

  - `AzureWebJobsStorage`: Connection string for Azure Storage.
  - `REDIS_CONNECTION_STRING`: Connection string for Azure Cache for Redis.
  - `SQL_DB_CONNECTION_STRING`: Connection string for Azure SQL Database.

## 📘 Usage

- The Retry Function is triggered by an HTTP POST request to the `/api/retry-operation` endpoint.
- It performs the necessary operations and updates the cache if successful.

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.
