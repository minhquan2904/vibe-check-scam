## 1. Configuration & Setup

- [x] 1.1 Add Gemini configuration section (`Gemini:ApiKey`, `Gemini:Endpoint`, `Gemini:Model`) to `appsettings.json`.
- [x] 1.2 Create `GeminiOptions.cs` in `Options/` to bind configuration.

## 2. Infrastructure & External Services

- [x] 2.1 Create `IGeminiClient.cs` interface.
- [x] 2.2 Create `GeminiClient.cs` implementing `IGeminiClient`, using `HttpClient` to call Gemini API and parse standard response.
- [x] 2.3 Register `GeminiClient` in DI container (`Program.cs`) using `AddHttpClient`.

## 3. Core Service Layer

- [x] 3.1 Create `IGasPriceService.cs` interface.
- [x] 3.2 Create `GasPriceService.cs` orchestrating caching (`IMemoryCache`) and calling `IGeminiClient`.
- [x] 3.3 Register `GasPriceService` in DI container as `Scoped` or `Transient`.

## 4. API Controller

- [x] 4.1 Create DTOs (e.g., `GasPriceResponse.cs`) for the endpoint response.
- [x] 4.2 Create `GasPriceController.cs` exposing `GET /api/v1/gas-price/today`.
- [x] 4.3 Add `[AllowAnonymous]` to exactly match the design specification.
