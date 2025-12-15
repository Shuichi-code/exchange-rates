# Copilot Instructions for Exchange Rates Monorepo

## Project Architecture

**Exchange Rates** is a monorepo containing a .NET 8 backend API and Angular 17 frontend for displaying foreign exchange rates from the Czech National Bank (CNB).

### Backend Structure (ASP.NET Core)

Clean architecture with three layers:

- **ExchangeRates.Api**: Controllers and HTTP entry points (`Controllers/ExchangeRatesController.cs`)
- **ExchangeRates.Application**: Interfaces and domain models (no infrastructure dependencies)
  - `Interfaces/IExchangeRateProvider.cs` - Abstraction for rate providers
  - `Models/ExchangeRatesResponse.cs`, `ExchangeRateRow.cs` - Domain models
  - `Exceptions/` - Custom exceptions for parse/fetch failures
- **ExchangeRates.Infrastructure**: Concrete implementations
  - `Providers/CnbExchangeRateProvider.cs` - Implements `IExchangeRateProvider`
  - `Parsing/CnbDailyRatesParser.cs` - Parses CNB's pipe-delimited text format
  - `Options/CnbOptions.cs` - Configuration from `appsettings.json` section `Cnb`

### Frontend Structure (Angular 17)

Single-page app in `frontend/exchange-rates-ui/`:

- `src/app/exchange-rates.service.ts` - HTTP client for `/api/exchange-rates/latest`
- `src/app/exchange-rates.component.ts` - Display component
- `src/app/models.ts` - TypeScript interfaces matching backend DTOs
- Angular CLI for builds/dev server

### Data Flow

1. Frontend calls `ExchangeRatesService.getLatestRates()` → GET `/api/exchange-rates/latest`
2. Controller calls `IExchangeRateProvider.GetLatestAsync()`
3. `CnbExchangeRateProvider` fetches from CNB URL (configured in `appsettings.json`)
4. Response is parsed by `CnbDailyRatesParser` (date from first line, header on line 2, pipe-delimited rates)
5. Result cached for 15 minutes (in-memory); returns `ExchangeRatesResponse`
6. Frontend displays in a table

## Key Patterns & Conventions

### Dependency Injection

- Services registered in `Program.cs` using extension methods
- `CnbExchangeRateProvider` is singleton `CnbDailyRatesParser`, scoped `HttpClient`
- Memory cache shared across requests

### Exception Handling

- Custom exceptions inherit from base: `ExchangeRateFetchException`, `ExchangeRateParseException`
- Controller catches and maps to HTTP status codes:
  - Fetch failures → 503 (Service Unavailable)
  - Parse failures → 502 (Bad Gateway)

### Configuration

- CNB URL loaded from `Cnb:DailyRatesUrl` in `appsettings.json`
- Development mode enables Swagger UI at `/swagger/ui`
- CORS policy "FrontendDev" allows requests from `http://localhost:4200`

### Testing

- xUnit with Fact attributes
- Parser tests validate date parsing, row parsing, and error cases
- Tests in `ExchangeRates.Tests/` reference Application and Infrastructure layers

## Build & Run Commands

**Backend (from `backend/` folder):**

```bash
dotnet build                 # Build solution
dotnet test                  # Run xUnit tests (ExchangeRates.Tests)
dotnet run --project ExchangeRates.Api/  # Start API on http://localhost:5000
```

**Frontend (from `frontend/exchange-rates-ui/` folder):**

```bash
npm install  # or pnpm install (project uses pnpm-lock.yaml)
npm start    # Dev server on http://localhost:4200
npm test     # Karma tests
npm build    # Production build
```

## Important Implementation Details

- **CNB Parser Format**: First line = date (DD.MM.YYYY #number), second line = headers, rest = currency rows with pipe-delimiters and Czech decimal commas
- **Culture-Specific Parsing**: Parser uses Czech culture (`cs-CZ`) for decimal parsing and English culture for API responses
- **HttpClient Timeout**: Set to 10 seconds in `Program.cs`
- **Cache Key**: Uses constant `"cnb-latest-rates"` with 15-minute duration

## Cross-Component Communication

- Backend API contract: `ExchangeRatesResponse` with `AsOfDate`, `BaseCurrency`, `Rates[]`
- Frontend models in `models.ts` must match backend DTOs
- API runs on port 5000, Angular CLI dev server on 4200 (configured in CORS policy)

## When Adding Features

1. **New Exchange Rate Sources**: Implement `IExchangeRateProvider` in Infrastructure, register in `Program.cs`
2. **New Endpoints**: Add to `ExchangeRatesController`, define response models in Application layer
3. **Frontend Changes**: Update `ExchangeRatesService` and components to match controller endpoints
4. **Configuration**: Add to `appsettings.json` and corresponding `*Options` class
