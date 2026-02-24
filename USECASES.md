# AWS ECS #1
- runs in http 8080 & https 8443 mode
  - Urls = https://*:8443;http://*:8080
- makes use of https cert in base64
  - HttpsCertP12__Type = base64
  - HttpsCertP12__Value = from secret mgr, injected by ECS
- provides fake endpoint to simulate spring boot healthcheck
  - DynamicGets__0 = /actuator/health
  - DynamicGets__1 = /actuator/liveliness
  - DynamicAppendCtxPath = false
- disable ssl verification when calling remote endpoint
  - HttpsDisableVerify=true
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
        { "name": "DynamicAppendCtxPath", "value": "false" },
        { "name": "HttpsCertP12__Type", "value": "base64" },
        { "name": "HttpsDisableVerify", "value": "true" },
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

# AWS ECS #2
- similar to #2
- test if certificate provided can call remote https
  - HttpsDisableVerify=false
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
        { "name": "DynamicAppendCtxPath", "value": "false" },
        { "name": "HttpsDisableVerify", "value": "false" },
        { "name": "HttpsCertP12__Type", "value": "base64" },
        { "name": "TruststoreCerts__0__Type", "value": "base64" },
      ],
      "secrets": [
        {
          "name": "HttpsCertP12__Value",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:HttpsP12CertInBase64"
        },
        {
          "name": "TruststoreCerts__0__Value",
          "valueFrom": "arn:aws:secretsmanager:us-east-1:123456789012:secret:RootCertInBase64"
        }
      ]
    }
  ]
}
```

- test if truststore cert works by curling /api/call to itself at /api/version
  - make sure options.host value is same as cert's CN (Common Name)
  - caller will overwrite SNI/Host header and validate server's cert using this value
```
curl -X 'POST' \
  'https://localhost:8444/api/call' \
  -H 'accept: application/json' \
  -H 'Content-Type: application/json' \
  -d '{
  "url": "https://cs-local:8443/api/version",
  "method": "get",
  "options": {
    "host": "localhost"
  },
  "data": {}
}'
```
