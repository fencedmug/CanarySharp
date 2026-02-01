# CanarySharp

## Configuration
ContextPath
- this adds a context path to routes
- e.g. ContextPath = "api", route = "api/echo", "api/version", etc

Version
- value returned by /version
- this is meant for pipelines to replace/insert value


## Endpoints
Version
- returns "Version" value in configuration

Echo
- returns caller's http headers
- meant to check if any middleware is adding headers

Call
- calls another endpoint
- meant for connectivity checks
