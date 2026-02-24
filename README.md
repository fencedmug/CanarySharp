# CanarySharp

## Configuration (appsettings.json)
ContextPath
- this adds a context path to routes
- e.g. ContextPath = "api", route = "api/echo", "api/version", etc

CustomPath
- name of custom path that represents two api paths
- GET & POST ${ContextPath}/${CustomPath}

Version
- value returned by /version
- this is meant for pipelines to replace/insert value


## Configuration via Environment variables
ASPNETCORE_HTTP_PORT=8080
ASPNETCORE_HTTP_PORTS=8443
ContextPath=env_api
CustomPath=env_custom


## APIs
GET ${ContextPath}/version
- returns "Version" value in configuration

GET ${ContextPath}/echo
- returns caller's http headers
- meant to check if any middleware is adding headers

POST ${ContextPath}/call
- calls another endpoint
- meant for connectivity checks

GET ${ContextPath}/${CustomPath}
- similar to echo

POST ${ContextPath}/${CustomPath}
- similar to echo
- returns caller's request

 
## Todos:
- https support - to help check if certs generated works
- host both http & https
- add endpoints to test commonly used services
  - redis
  - database
- add endpoints to test aws iam roles when calling various services



# References

## Server Endpoint Configuration (Kestrel)
https://learn.microsoft.com/en-us/aspnet/core/fundamentals/servers/kestrel/endpoints

## Generate P12 for local testing
choco install openssl
openssl req -x509 -newkey rsa:2048 -nodes -keyout selfsign.key -out selfsign.cert -days 36500 -subj "/CN=localhost/O=selfsign"
openssl pkcs12 -export -out selfsign.p12 -inkey selfsign.key -in selfsign.cert -password pass:""

## Copy file content to base64
[Convert]::ToBase64String([IO.File]::ReadAllBytes("$PWD\selfsign.p12")) | Set-Clipboard
