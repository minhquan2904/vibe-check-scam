# Delta Spec

### Overview of Changes
- **Feature Scope**: Injected a brand new controller (`GasPriceController`) to support retrieving real-time gas prices via LLM.

### API Surface Area
- **Added Routes**: `GET /api/v1/gas-price/today` (publicly accessible).
- **Modified Routes**: None
- **Removed Routes**: None

### Architectural Impact
- **Database**: No tables, migrations, or contexts were affected.
- **External Dependencies**: Added `HttpClient` connecting to Google's Generative Language API (`GeminiClient`). Introduced new Configuration objects `GeminiOptions` and `GasPriceOptions`.
- **Performance**: Introduced an `IMemoryCache` facade intercepting the Gemini API calls. Ensures internal network stability by caching responses locally for 60 minutes.
