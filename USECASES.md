# AWS ECS
- runs in http 8080 & https 8443 mode
  - Urls = https://*:8443;http://*:8080
- makes use of https cert in base64
  - HttpsCertP12__Type = base64
  - HttpsCertP12__Value = from secret mgr, injected by ECS
- provides fake endpoint to simulate spring boot healthcheck
  - DynamicGets__0 = /actuator/health
  - DynamicGets__1 = /actuator/liveliness
```json
{
  "containerDefinitions": [
    {
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        },
        {
          "containerPort": 8443,
          "protocol": "tcp"
        }
      ],
      "environment": [
        { "name": "ASPNETCORE_ENVIRONMENT", "value": "Production" },
        { "name": "Urls", "value": "https://*:8443;http://*:8080" },
        { "name": "ContextPath", "value": "api" },
        { "name": "CustomPath", "value": "custom-api-name" },
        { "name": "DynamicGets__0", "value": "/actuator/health" },
        { "name": "DynamicGets__1", "value": "/actuator/liveliness" },
        { "name": "HttpsCertP12__Type", "value": "base64" },
      ],
      "secrets": [
        {
          "name": "HttpsCertP12__Value",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:HttpsP12CertInBase64"
        }
      ]
    }
  ]
}
```
