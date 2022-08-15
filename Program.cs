using System.Diagnostics.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetryMetrics(metricsProvider =>
{
    metricsProvider
        .AddConsoleExporter()
        .AddOtlpExporter(options =>
        {
            options.Endpoint = new Uri("http://otel-collector.default.svc.cluster.local:4317");
            options.Protocol = OtlpExportProtocol.Grpc;
        })
        .AddMeter("otel.demo.metric")
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(serviceName: "otel.demo", serviceVersion: "0.0.1")
        );
});

var app = builder.Build();
var otel_metric = new Meter("otel.demo.metric", "0.0.1");
var randomNum = new Random();
// Create two metrics
var obs_gauge1 = otel_metric.CreateObservableGauge<int>("otel.demo.metric.gauge1", () =>
{
    return randomNum.Next(10, 80);
});
var obs_gauge2 = otel_metric.CreateObservableGauge<double>("otel.demo.metric.gauge2", () =>
{
    return randomNum.NextDouble();
});

app.MapGet("/otelmetric", () =>
{
    return "Hello, Otel-Metric!";
});

app.Run();
