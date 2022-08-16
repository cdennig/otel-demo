```shell
dotnet new webapi -minimal -n otel-demo -o .

dotnet add package System.Diagnostics.DiagnosticSource
dotnet add package OpenTelemetry.Extensions.Hosting --prerelease
dotnet add package OpenTelemetry.Exporter.Console
dotnet add package OpenTelemetry.Instrumentation.AspNetCore --prerelease
dotnet add package OpenTelemetry.Instrumentation.Http --prerelease
dotnet add package OpenTelemetry.Instrumentation.SqlClient --prerelease
dotnet add package OpenTelemetry.Exporter.OpenTelemetryProtocol

dotnet tool install --global dotnet-counters
dotnet-counters ps
dotnet-counters monitor -p <PORT> "otel.demo.metric"

kind create cluster --name demo-cluster --config ./kind/kind-cluster.yaml

# Prom Operator
helm upgrade --install --wait --timeout 15m \
  --namespace monitoring --create-namespace \
  --repo https://prometheus-community.github.io/helm-charts \
  kube-prometheus-stack kube-prometheus-stack --values ./prometheus/values.yaml

docker build -t otel-demo:1.0 .

kind load --name demo-cluster docker-image otel-demo:1.0

kubectl apply -f ./manifests
´´´