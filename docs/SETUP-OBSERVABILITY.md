=> Read https://github.com/Cloudcostify/platform/blob/main/src/Platform.Shared/Telemetry/README.md

Instance id can be found at => https://grafana.com/auth/sign-in/ => Manage your Grafana Cloud stack on the homepage => Details button => Opentelemetry configure button <img width="603" height="205" alt="image" src="https://github.com/user-attachments/assets/befdbedc-8727-4f4d-b2a1-ac0a7a114aa5" /> For example open telemetry region endpoint: https://otlp-gateway-prod-us-central-0.grafana.net/otlp

=> Instance id and grafana token generation button is on the same page
=> <img width="1254" height="113" alt="image" src="https://github.com/user-attachments/assets/b44e637e-f78a-4549-a231-c8d82a2cf8a0" />

=> Open telemetry data can be seen in the grafana stack instance for example: https://<stack-name>.grafana.net/ > Explore

=> Environment variables that need to be set in production service: 

```powershell
[Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes("<instance-id>:<grafana-opentelemetry-token>"))
```

```bash
OTEL_EXPORTER_OTLP_ENDPOINT="https://otlp-gateway-prod-us-central-0.grafana.net/otlp"
OTEL_EXPORTER_OTLP_HEADERS="Authorization=Basic <instance id and granana-opentelemetry-token base64 encoded>"
OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
OTEL_SERVICE_NAME="<service-name for example: cloudcostify-cost-estimation-api>"
```




