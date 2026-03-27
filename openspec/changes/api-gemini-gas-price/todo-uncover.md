# TODO & Uncover Tracking

| # | Type | Location | Line # | Description |
|---|---|---|---|---|
| 1 | UNCOVER | `GasPriceService.cs` | 41 | Cache expiration is currently set to an absolute 60 minutes based on appsettings. If gas prices change intra-day or more dynamically, we may need a mechanism to invalidate cache instead of waiting out the absolute expiration. |
| 2 | UNCOVER | `GeminiClient.cs` | 46 | JSON extraction from Gemini uses hardcoded paths (`document.RootElement.GetProperty("candidates")[0]...`). If Gemini's API response structure changes, this will throw an exception. We should add structured safeguards if we plan to scale Gemini models. |
