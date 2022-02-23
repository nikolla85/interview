## Nacin inicijalizacije biblioteke u projekat

Potrebno je kreirati NuGet paket te isti instalirati u zeljeni servis. Zatim je potrebno registrovati middleware u pipeline-u.
Primer registrovanja middleware-a:

```csharp
 var rateLimiterConfiguration = Configuration.GetSection("RateLimiter").Get<RateLimiterConfiguration>();
 app.UseRateLimiter(rateLimiterConfiguration);
 ```

## Primer konfiguracije iz TestApi servisa

Konfiguracija se nalazi unutar appsettings.json file-a.

```json
"RateLimiter": {
  "RequestLimiterEnabled": true,
  "DefaultRequestLimitMs": 1000,
  "DefaultRequestLimitCount": 5,
  "EndpointLimits": [
    {
      "Endpoint": "/api/products/books",
      "RequestLimitMs": 1000,
      "RequestLimitCount": 1
    },
    {
      "Endpoint": "/api/products/pencils",
      "RequestLimitMs": 500,
      "RequestLimitCount": 2
    }
  ]
}
```

## Napomene

Pored RateLimiter biblioteke napravljen je testni WebApi servis kao i IntegrationTests projekat sa testovima koji proveravaju zeljenu funkcionalnost.
U ovom primeru RateLimiter biblioteka je dodata kao project reference u TestApi servis, u realnom primeru bi to bio NuGet paket.


