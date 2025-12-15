# Exchange Rates Monorepo

This repository contains a simple full-stack application that retrieves **real daily exchange-rate fixing data from the Czech National Bank (CNB)** and displays it in a web UI.

The solution is intentionally minimal and focuses on correctness, clarity, and maintainability.

---

## Project Structure

- backend/ ASP.NET Core (.NET 8) Web API
- frontend/exchange-rates-ui Angular web application

---

## Backend (ASP.NET Core API)

- Framework: .NET 8
- Endpoint:

  ```
  GET /api/exchange-rates/latest
  ```

- Fetches and parses the official CNB daily exchange-rate fixing feed
- Returns **raw CNB data** (including the original `Amount` field)
- Uses in-memory caching to avoid unnecessary upstream requests
- Swagger UI is enabled in Development

### Run backend

```bash
dotnet run --project backend/ExchangeRates.Api
```

Swagger UI:

```
http://localhost:5000/swagger
```

---

## Frontend (Angular)

- Angular (latest)
- Fetches exchange rates from the backend API
- Displays rates in a searchable table
- Clearly indicates that rates are shown in raw CNB format

### Run frontend

```bash
cd frontend/exchange-rates-ui
npm install
npm start
```

The frontend expects the backend to be running at:

```
http://localhost:5000
```

---

## Notes & Assumptions

- Exchange rates are displayed exactly as published by CNB (no normalization).
- Some currencies are quoted per 100 or 1000 units; this is reflected via the Amount field.
- The dataset updates once per working day, so short-term in-memory caching is sufficient.

---

## Data Source

- Czech National Bank (CNB)
- https://www.cnb.cz/
