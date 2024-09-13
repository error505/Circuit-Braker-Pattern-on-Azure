// Parameters
param location string = resourceGroup().location
param storageAccountName string = 'cbpatternstorage'
param appServicePlanName string = 'cbAppServicePlan'
param apiAppServiceName string = 'cbApiAppService'
param functionAppRetryName string = 'cbFunctionAppRetry'
param functionAppFallbackName string = 'cbFunctionAppFallback'
param staticWebAppName string = 'cbStaticWebApp'
param redisCacheName string = 'cbRedisCache'
param sqlServerName string = 'cbSqlServer'
param sqlDatabaseName string = 'cbDatabase'
param sqlDatabaseUserName string = 'adminuser'
@secure()
#disable-next-line secure-parameter-default
param sqlDatabasePassword string = 'Password@123'
param appInsightsName string = 'cbAppInsights'
param frontDoorName string = 'cbFrontDoor'

// Azure Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
  }
}

// Azure App Service Plan
resource appServicePlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: appServicePlanName
  location: location
  sku: {
    name: 'S1'
    tier: 'Standard'
  }
}

// API App Service
resource apiAppService 'Microsoft.Web/sites@2022-03-01' = {
  name: apiAppServiceName
  location: location
  kind: 'app'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccount.properties.primaryEndpoints.blob
        }
        {
          name: 'FUNCTION_APP_RETRY_URL'
          value: 'https://${functionAppRetryName}.azurewebsites.net'
        }
        {
          name: 'FUNCTION_APP_FALLBACK_URL'
          value: 'https://${functionAppFallbackName}.azurewebsites.net'
        }
      ]
    }
  }
}

// Azure Function App for Retries and Circuit Breaker
resource functionAppRetry 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppRetryName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccount.properties.primaryEndpoints.blob
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'SQL_DB_CONNECTION_STRING'
          value: 'Server=tcp:${sqlServerName}.database.windows.net,1433;Database=${sqlDatabaseName};User ID=${sqlDatabaseUserName};Password=${sqlDatabasePassword};'
        }
        {
          name: 'REDIS_CONNECTION_STRING'
          value: 'Primary connection string from Azure Redis Cache'
        }
      ]
    }
  }
}

// Azure Function App for Fallback and Cache Updates
resource functionAppFallback 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppFallbackName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: appServicePlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccount.properties.primaryEndpoints.blob
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'REDIS_CONNECTION_STRING'
          value: 'Primary connection string from Azure Redis Cache'
        }
      ]
    }
  }
}

// Azure Static Web App
resource staticWebApp 'Microsoft.Web/staticSites@2023-12-01' = {
  name: staticWebAppName
  location: location
  sku: {
    name: 'Standard'
  }
}

// Azure Cache for Redis
resource redisCache 'Microsoft.Cache/redis@2024-04-01-preview' = {
  name: redisCacheName
  location: location
  properties: {
    sku: {
      name: 'Standard'
      family: 'C'
      capacity: 1
    }
  }
}

// Azure SQL Server
resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: 'adminuser'
    administratorLoginPassword: 'Password@123'
  }
}

// Azure SQL Database
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDatabaseName
  location: location
}

// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    RetentionInDays: 30
  }
}

// Azure Front Door
resource frontDoor 'Microsoft.Network/frontDoors@2021-06-01' = {
  name: frontDoorName
  location: location
  properties: {
    frontendEndpoints: [
      {
        name: 'default'
      }
    ]
  }
}
