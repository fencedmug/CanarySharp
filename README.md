# CanarySharp
![Docker Version](https://img.shields.io/docker/v/fencedmug/canarysharp)
![Docker Image Size](https://img.shields.io/docker/image-size/fencedmug/canarysharp/latest)

## Configuration (appsettings.json)
ContextPath
- this adds a context path to routes
- e.g. ContextPath = "api", route = "api/echo", "api/version", etc

CustomPath
- name of custom path that represents two api paths
- GET & POST ```${ContextPath}/${CustomPath}```

Version
- value returned by /version
- this is meant for pipelines to replace/insert value

## Local testing
```http://localhost:8443/swagger/index.html```
```
podman compose up -d cs-local
podman compose down cs-local
```

## Configuration via Environment variables
```
Urls=https://*:8443;http://*:8080
HttpsCertP12__Type=filepath
HttpsCertP12__Value=../selfsign.p12
ContextPath=env_api
CustomPath=env_custom
DynamicGets__0="/actuator/health"
DynamicGets__1="/actuator/health/readiness"
DynamicAppendCtxPath=false
```

## APIs
GET ```${ContextPath}/version```
- returns "Version" value in configuration

GET ```${ContextPath}/echo```
- returns caller's http headers
- meant to check if any middleware is adding headers

POST ```${ContextPath}/call```
- calls another endpoint
- meant for connectivity checks

GET ```${ContextPath}/${CustomPath}```
- similar to echo

POST ```${ContextPath}/${CustomPath}```
- similar to echo
- returns caller's request

 
## Todos:
- add endpoints to test commonly used services
  - redis
  - database
- add endpoints to test aws iam roles when calling various services



# References

## Server Endpoint Configuration (Kestrel)
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints

## Generate P12 for local testing
```
choco install openssl
openssl req -x509 -newkey rsa:2048 -nodes -keyout selfsign.key -out selfsign.cert -days 36500 -subj "/CN=localhost/O=selfsign"
openssl pkcs12 -export -out selfsign.p12 -inkey selfsign.key -in selfsign.cert -password pass:""
```

## Copy file content to base64
```
[Convert]::ToBase64String([IO.File]::ReadAllBytes("$PWD\selfsign.p12")) | Set-Clipboard
```
