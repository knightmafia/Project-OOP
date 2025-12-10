# Transportation Quote & Reservation Console

## Overview
Console-based booking assistant that manages customers, services, quotes (with optional return trips), and reservations against a SQLite database. Built to demonstrate CRUD, menus, and basic data relationships.

## Features
- Customer and service management (create, list, delete by primary key)
- Quote creation with outbound/return trip items, status updates, and counter-quotes
- Reservation creation from accepted quotes, plus deletion by primary key
- Cascading cleanup: deleting a quote removes its quote items and related reservations
- Inline comments on helpers to clarify function intent

## Technologies Used
- C# / .NET console app (target `net10.0`)
- SQLite via `Microsoft.Data.Sqlite`
- Basic ADO.NET commands for CRUD

## Project Structure
```
/Project OOP
├── BookingDocument.cs
├── Customer.cs
├── CustomerDB.cs
├── Program.cs
├── Quote.cs
├── QuoteDB.cs
├── QuoteItem.cs
├── QuoteItemDB.cs
├── Reservation.cs
├── ReservationDB.cs
├── Service.cs
├── ServiceDB.cs
├── TravelSchedule.cs
├── SQLiteDatabase.cs
├── Booking.db          (created at runtime)
├── Project OOP.csproj
└── README.md
```

## How to Run

### Prerequisites
- .NET SDK 8.0 or later (project targets `net10.0`; 8+ SDKs can build/run with preview enabled)
- SQLite available on the host (database file is created automatically)

### Steps
1. Restore packages  
   `dotnet restore`
2. Run the console  
   `dotnet run`
3. Use the menu to perform CRUD operations. Primary keys displayed in lists are used for delete flows.

## Menu Guide (CRUD)
- Customers: list, add, delete by ID
- Services: list, add, delete by ID
- Quotes: create, update status/send counter, delete by ID (removes related items/reservations), list
- Reservations: create from accepted quotes, delete by ID, list

## OOP Concepts Demonstrated
| Concept        | Implementation                                   |
|----------------|---------------------------------------------------|
| Abstraction    | Shared base `BookingDocument`; interfaces in models|
| Encapsulation  | Entity classes manage their own calculations/state|
| Polymorphism   | Menu flows reuse common selection/output helpers  |
| Composition    | Quotes own outbound/return `QuoteItem` collections|

## Extending the Project
- Add an update/edit flow for customers and services
- Support taxes/fees in quote totals
- Persist and view basic reports (e.g., revenue by service)
- Add input validation and nicer formatting for currency/dates

## Author
Developed by Eric Martinez for SDC320 coursework; freely modifiable for learning. 
