# 💱 Exchange Rate Wallet API

This is a .NET 8 Web API project designed to interact with the **European Central Bank** (ECB) exchange rate feed. It allows for:

- Periodic synchronization of currency exchange rates.

- Creation and management of wallets in various currencies.

- Currency conversion and balance adjustments using **Strategy Pattern**.

- Historical tracking of exchange rate data.

- Clean architectural separation and extensibility.

---
## 🏗 Architecture

This solution follows **Clean Architecture** principles, separating responsibilities clearly into distinct layers:

├── Application           // Core logic, interfaces, and services

├── Infrastructure       // Database, external APIs, repository implementations

├── API                        // HTTP Controllers and middleware

├── Model                   // Dtos, Entities

├── Test                      // Unit Tests

---

## 🧠 Design Patterns Used

### Strategy Pattern

Used for balance adjustment strategies:

- `AddFundsStrategy`

- `SubtractFundsStrategy`

- `ForceSubtractFundsStrategy`

Each strategy implements the `IAdjustWalletStrategy` interface, allowing flexible behavior injection based on user input.

### Repository Pattern

All database interactions go through repository interfaces (`IWalletRepository`, `IExchangeRateRepository`, etc.) to abstract EF Core implementation and follow SOLID principles.

### Dependency Injection

Microsofts Dependency Injection Framework to apply the inversion of control principle  

---
## 🛠 Tech Stack

- **.NET 8**

- **Entity Framework Core** with PostgreSQL

- **Quartz.NET** for scheduled jobs

- **Moq** and **xUnit** for testing

- **Options** for configuration

---
## API Endpoints

### Wallet Management

#### 🔹 Create a Wallet

**POST** `/api/wallets`

```json
{
  "currency": "USD"
}
```

#### **🔹 Get Wallet Balance**

**GET** /api/wallets/{walletId}?currency=EUR

- Returns wallet balance, optionally converted to the requested currency using the latest exchange rate.

#### **🔹 Adjust Wallet Balance**

**POST** /api/wallets/{walletId}/adjustbalance

**Query Parameters:**

- amount: positive decimal (e.g. 100.50)

- currency: 3-letter ISO code (e.g. “EUR”, “USD”)

- strategy: Add, Subtract, or ForceSubtract

---
## **Exchange Rate Management**

- Rates are fetched **every minute** from:

  https://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml

- Stored in the ExchangeRates table.

- Rates are inserted/updated using a **MERGE** SQL operation.

- The ECB base currency is **EUR**.


---

## **🕐 Periodic Job (Quartz.NET)**

The job:
- Fetches current rates from the ECB endpoint.

- Merges them into the DB (adds new or updates existing for the same date).

- Updates in-memory cache (if enabled).

Configured via Quartz with .AddJob<>() and .AddTrigger<>().

---
## **## 🚀How to Run**

1. Configure appsettings.json:
```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=exchange_db;Username=your_user;Password=your_password"
}
```

2. Run migrations:
```
dotnet ef database update
```

3. Start the application:
```
dotnet run
```

4. Quartz job will start and fetch rates every minute.

---
## **Additional Notes**

- All balances are stored in the wallet’s base currency.
    
- Adjustments are automatically converted to/from the wallet currency using the most recent rate.


> Built with ❤️ and best practices in C# and .NET Core.