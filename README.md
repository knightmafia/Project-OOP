# Transportation Quote & Reservation Console

## Overview
Console-based booking assistant that manages customers, services, quotes (with optional return trips), and reservations against a SQLite database. Built to demonstrate CRUD, menus, and basic data relationships.

## Screen Recording
- Demo.mov

## Features
- Customer and service management (create, list, delete by primary key)
- Quote creation with outbound/return trip items, status updates, and counter-quotes
- Reservation creation from accepted quotes, plus deletion by primary key
- Cascading cleanup: deleting a quote removes its quote items and related reservations
- Inline comments on helpers to clarify function intent

## Technologies Used
- C# / .NET console app (target `net8.0`)
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
- .NET SDK 8.0 or later
- SQLite available on the host (database file is created automatically)

### Steps
1. Restore packages  
   `dotnet restore`
2. Run the console  
   `dotnet run`
3. Use the menu to perform CRUD operations. Primary keys displayed in lists are used for delete flows.

## Menu Guide (CRUD)
- Customers: list, add, delete by ID
- Customers: update by ID
- Services: list, add, delete by ID
- Services: update by ID
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
Developed by Eric Martinez.

---

## Project Summary
### Project Description
This project is a console-based transportation booking assistant that helps staff create customers and services, generate quotes (including optional return trips), and convert accepted quotes into reservations. Data is persisted in a SQLite database and managed through a menu-driven UI that demonstrates object-oriented design and full CRUD operations.

### Project Tasks
- **Task 1: Set up the development environment**
- Install the .NET SDK and configure the project
- Add SQLite package dependencies
- **Task 2: Design the domain model**
- Create classes for customers, services, quotes, quote items, and reservations
- Apply OOP concepts (abstraction, composition, polymorphism, constructors)
- **Task 3: Implement database persistence**
- Create tables at startup
- Implement create, read, update, and delete flows for each entity
- **Task 4: Implement console UI**
- Build menu navigation, input prompts, and formatted output
- Add validation/confirmation for destructive actions
- **Task 5: Document and deliver**
- Write README usage and feature documentation
- Record a short demonstration video and link it in the repository

### Project Skills Learned
- Object-oriented programming in C# (inheritance, interfaces, encapsulation)
- ADO.NET-style SQLite CRUD with parameterized queries
- Console UI design (menus, prompts, validation)
- Structuring a small application for maintainability
- Git/GitHub workflow and documentation for portfolio readiness

### Language Used
- **C# / .NET**

### Development Process Used
- **Iterative Development**: implementing features in small increments, testing each menu flow against the database as it was added.

### Link to Project
https://github.com/erimar2678ecpi/Project-OOP.git
