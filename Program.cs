/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Console entry point with DB-backed CRUD/menu for customers, services, quotes, and reservations.
 */

using Microsoft.Data.Sqlite;
using Project_OOP;

public class OOPProject
{
    public static void Main(string[] args)
    {
        const string dbName = "Booking.db";
        Console.WriteLine("\nEric Martinez, Week 4 Database Interactions GP\n");
        using SqliteConnection conn = SQLiteDatabase.Connect(dbName);
        if (conn == null)
        {
            Console.WriteLine("Unable to open database connection.");
            return;
        }

        CustomerDB.CreateTable(conn);
        ServiceDB.CreateTable(conn);
        QuoteDB.CreateTable(conn);
        QuoteItemDB.CreateTable(conn);
        ReservationDB.CreateTable(conn);

        Console.WriteLine("Welcome to the Transportation Quote console. Choose an option to begin.");
        var running = true;
        while (running)
        {
            Console.WriteLine("\nMenu:");
            Console.WriteLine("1) List customers");
            Console.WriteLine("2) Add customer");
            Console.WriteLine("3) List services");
            Console.WriteLine("4) Add service");
            Console.WriteLine("5) Create quote");
            Console.WriteLine("6) Update quote status / send counter");
            Console.WriteLine("7) Create reservation from accepted quote");
            Console.WriteLine("8) List quotes");
            Console.WriteLine("9) List reservations");
            Console.WriteLine("0) Exit");

            var choice = PromptInt("Select option", 0);
            switch (choice)
            {
                case 1:
                    ListCustomers(conn);
                    break;
                case 2:
                    AddCustomer(conn);
                    break;
                case 3:
                    ListServices(conn);
                    break;
                case 4:
                    AddService(conn);
                    break;
                case 5:
                    CreateQuote(conn);
                    break;
                case 6:
                    UpdateQuoteStatus(conn);
                    break;
                case 7:
                    CreateReservation(conn);
                    break;
                case 8:
                    ListQuotes(conn);
                    break;
                case 9:
                    ListReservations(conn);
                    break;
                case 0:
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void ListCustomers(SqliteConnection conn)
    {
        var customers = CustomerDB.GetAllCustomers(conn);
        if (customers.Count == 0)
        {
            Console.WriteLine("No customers found.");
            return;
        }

        Console.WriteLine("\nCustomers:");
        foreach (var c in customers)
        {
            Console.WriteLine(c);
        }
    }

    static void AddCustomer(SqliteConnection conn)
    {
        var first = Prompt("First Name", "First");
        var last = Prompt("Last Name", "Last");
        var phone = Prompt("Phone", "123-123-1234");
        var email = Prompt("Email", "customer@email.com");

        var customer = new Customer(first, last, phone, email, DateTime.Now);
        try
        {
            CustomerDB.AddCustomer(conn, customer);
            Console.WriteLine("Customer added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding customer: {ex.Message}");
        }
    }

    static void ListServices(SqliteConnection conn)
    {
        var services = ServiceDB.GetAllServices(conn);
        if (services.Count == 0)
        {
            Console.WriteLine("No services found.");
            return;
        }

        Console.WriteLine("\nServices:");
        foreach (var s in services)
        {
            Console.WriteLine(s);
        }
    }

    static void AddService(SqliteConnection conn)
    {
        var name = Prompt("Service name", "New Service");
        var description = Prompt("Description", string.Empty);
        var rate = PromptDecimal("Rate", 50m);
        var unitChoice = Prompt("Unit type (Flat/Hourly/PerMile)", "Flat");
        var unitType = Enum.TryParse<ServiceUnitType>(unitChoice, true, out var parsed) ? parsed : ServiceUnitType.Flat;
        var isActiveInput = Prompt("Is active? (y/n)", "y");
        var isActive = isActiveInput.StartsWith("y", StringComparison.OrdinalIgnoreCase);

        var service = new Service(name, description, rate, unitType, isActive);
        try
        {
            ServiceDB.AddService(conn, service);
            Console.WriteLine("Service added.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding service: {ex.Message}");
        }
    }

    static void CreateQuote(SqliteConnection conn)
    {
        var customer = SelectCustomer(conn);
        if (customer == null)
        {
            return;
        }

        var service = SelectService(conn, "Select outbound service");
        if (service == null)
        {
            return;
        }

        var qty = PromptInt("Outbound quantity/units (e.g., hours, miles, or 1 for flat)", 1);
        var notes = Prompt("Notes (optional)", string.Empty);
        var quoteId = $"Q{DateTime.Now:yyyyMMddHHmmssfff}";
        var quote = new Quote(quoteId, customer.Id.ToString(), DateTime.Now, notes);

        var outboundItem = new QuoteItem($"QI{Guid.NewGuid():N}".Substring(0, 6), quote.Id, service.Id, service.Name, service.Rate, qty);
        quote.AddItem(outboundItem);

        var addReturn = Prompt("Add a return trip? (y/n)", "n");
        if (addReturn.StartsWith("y", StringComparison.OrdinalIgnoreCase))
        {
            var returnService = SelectService(conn, "Select return service (or press Enter to reuse outbound)", allowEmpty: true) ?? service;
            var returnQty = PromptInt("Return quantity/units (e.g., hours, miles, or 1 for flat)", 1);
            var returnItem = new QuoteItem($"QI{Guid.NewGuid():N}".Substring(0, 6), quote.Id, returnService.Id, $"{returnService.Name} (Return)", returnService.Rate, returnQty);

            var returnWhen = PromptDateTime("Return date/time (e.g. 2025-12-01 14:30)", DateTime.Now.AddDays(1));
            var returnPickup = Prompt("Return pickup location", "Return pickup");

            quote.AddItem(returnItem, isReturnTrip: true);
            quote.SetReturnDetails(returnWhen, returnPickup);
        }

        quote.CalculateTotal();

        try
        {
            QuoteDB.AddQuote(conn, quote);
            QuoteItemDB.AddQuoteItem(conn, outboundItem, false);
            foreach (var item in quote.ReturnTripItems)
            {
                QuoteItemDB.AddQuoteItem(conn, item, true);
            }
            Console.WriteLine($"Quote created: {quote}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating quote: {ex.Message}");
        }
    }

    static void UpdateQuoteStatus(SqliteConnection conn)
    {
        var quote = SelectQuote(conn);
        if (quote == null) return;

        Console.WriteLine("Quote actions:");
        Console.WriteLine("1) Accept");
        Console.WriteLine("2) Reject");
        Console.WriteLine("3) Send counter quote");
        Console.WriteLine("0) Cancel");
        var action = PromptInt("Choose action", 0);
        switch (action)
        {
            case 1:
                QuoteDB.UpdateQuoteStatus(conn, quote.Id, QuoteStatus.Accepted);
                Console.WriteLine("Quote accepted.");
                break;
            case 2:
                QuoteDB.UpdateQuoteStatus(conn, quote.Id, QuoteStatus.Rejected);
                Console.WriteLine("Quote rejected.");
                break;
            case 3:
                QuoteDB.UpdateQuoteStatus(conn, quote.Id, QuoteStatus.PendingCustomerReview, null);
                Console.WriteLine("Counter quote sent.");
                break;
            default:
                Console.WriteLine("No action taken.");
                break;
        }
    }

    static void CreateReservation(SqliteConnection conn)
    {
        var quote = SelectQuote(conn, requireAccepted: true);
        if (quote == null) return;

        quote.CalculateTotal();

        var daysAhead = PromptInt("Outbound trip: schedule how many days from now?", 3);
        var location = Prompt("Outbound pickup location", "Home");
        var instructions = Prompt("Outbound instructions (optional)", string.Empty);
        var outboundSchedule = new TravelSchedule(DateTime.Now.AddDays(daysAhead), location, instructions);

        var created = 0;
        var existing = ReservationDB.GetAllReservations(conn);

        if (!existing.Any(r => r.QuoteId == quote.Id && !r.IsReturnTrip))
        {
            var outboundReservation = new Reservation(
                id: $"R{Guid.NewGuid():N}".Substring(0, 6),
                quoteId: quote.Id,
                customerId: quote.CustomerId,
                schedule: outboundSchedule,
                serviceSummary: $"Outbound segment for quote {quote.Id}",
                totalPrice: quote.OutboundTotal > 0 ? quote.OutboundTotal : quote.Total,
                isReturnTrip: false,
                notes: "Created via console menu");

            ReservationDB.AddReservation(conn, outboundReservation);
            created++;
            Console.WriteLine($"Reservation created: {outboundReservation}");
        }
        else
        {
            Console.WriteLine("Outbound reservation already exists for this quote.");
        }

        if (quote.HasReturnTrip)
        {
            var createReturn = Prompt("Create return reservation now? (y/n)", "y");
            if (createReturn.StartsWith("y", StringComparison.OrdinalIgnoreCase))
            {
                var returnWhen = PromptDateTime("Return date/time (e.g. 2025-12-01 14:30)", quote.ReturnDateTime ?? outboundSchedule.When.AddDays(1));
                var returnPickup = Prompt("Return pickup location", quote.ReturnPickupLocation ?? location);
                var returnInstructions = Prompt("Return instructions (optional)", string.Empty);
                var returnSchedule = new TravelSchedule(returnWhen, returnPickup, returnInstructions);

                if (!existing.Any(r => r.QuoteId == quote.Id && r.IsReturnTrip))
                {
                    var returnReservation = new Reservation(
                        id: $"R{Guid.NewGuid():N}".Substring(0, 6),
                        quoteId: quote.Id,
                        customerId: quote.CustomerId,
                        schedule: returnSchedule,
                        serviceSummary: $"Return segment for quote {quote.Id}",
                        totalPrice: quote.ReturnTotal > 0 ? quote.ReturnTotal : quote.Total,
                        isReturnTrip: true,
                        notes: "Return trip reservation");

                    ReservationDB.AddReservation(conn, returnReservation);
                    created++;
                    Console.WriteLine($"Reservation created: {returnReservation}");
                }
                else
                {
                    Console.WriteLine("Return reservation already exists for this quote.");
                }
            }
        }

        if (created == 0)
        {
            Console.WriteLine("No new reservations created.");
        }
    }

    static void ListQuotes(SqliteConnection conn)
    {
        var quotes = QuoteDB.GetAllQuotes(conn);
        ListQuotes(conn, quotes);
    }

    static void ListQuotes(SqliteConnection conn, List<Quote> quotes)
    {
        if (quotes.Count == 0)
        {
            Console.WriteLine("No quotes found.");
            return;
        }

        Console.WriteLine("\nQuotes:");
        foreach (var q in quotes)
        {
            Console.WriteLine(q);
            var items = QuoteItemDB.GetItemsForQuote(conn, q.Id);
            foreach (var item in items.outbound)
            {
                Console.WriteLine($"  - {item}");
            }
            foreach (var item in items.returns)
            {
                Console.WriteLine($"  - {item}");
            }
        }
    }

    static void ListReservations(SqliteConnection conn)
    {
        var reservations = ReservationDB.GetAllReservations(conn);
        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations found.");
            return;
        }

        Console.WriteLine("\nReservations:");
        foreach (var r in reservations)
        {
            Console.WriteLine(r);
        }
    }

    // Selection helpers to reduce manual ID typing.
    static Customer? SelectCustomer(SqliteConnection conn)
    {
        var customers = CustomerDB.GetAllCustomers(conn);
        if (customers.Count == 0)
        {
            Console.WriteLine("No customers available. Add a customer first.");
            return null;
        }

        Console.WriteLine("\nCustomers:");
        for (var i = 0; i < customers.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {customers[i]}");
        }

        var index = PromptInt("Select customer by number", 1);
        index = Math.Clamp(index, 1, customers.Count);
        return customers[index - 1];
    }

    static Service? SelectService(SqliteConnection conn, string prompt, bool allowEmpty = false)
    {
        var services = ServiceDB.GetAllServices(conn);
        if (services.Count == 0)
        {
            Console.WriteLine("No services available. Add a service first.");
            return null;
        }

        Console.WriteLine("\nServices:");
        for (var i = 0; i < services.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {services[i]}");
        }

        Console.Write($"{prompt} (1-{services.Count}): ");
        var input = Console.ReadLine();
        if (allowEmpty && string.IsNullOrWhiteSpace(input))
        {
            return null;
        }
        if (!int.TryParse(input, out var index))
        {
            index = 1;
        }
        index = Math.Clamp(index, 1, services.Count);
        return services[index - 1];
    }

    static Quote? SelectQuote(SqliteConnection conn, bool requireAccepted = false)
    {
        var quotes = QuoteDB.GetAllQuotes(conn);
        if (requireAccepted)
        {
            quotes = quotes.Where(q => q.Status == QuoteStatus.Accepted).ToList();
        }

        if (quotes.Count == 0)
        {
            Console.WriteLine(requireAccepted ? "No accepted quotes found." : "No quotes found.");
            return null;
        }

        Console.WriteLine("\nQuotes:");
        for (var i = 0; i < quotes.Count; i++)
        {
            Console.WriteLine($"{i + 1}) {quotes[i]}");
        }

        var index = PromptInt("Select quote by number", 1);
        index = Math.Clamp(index, 1, quotes.Count);
        return quotes[index - 1];
    }

    // Local helper methods for simple terminal input.
    static string Prompt(string message, string fallback)
    {
        Console.Write($"{message}: ");
        var input = Console.ReadLine();
        return string.IsNullOrWhiteSpace(input) ? fallback : input.Trim();
    }

    static int PromptInt(string message, int fallback)
    {
        Console.Write($"{message}: ");
        return int.TryParse(Console.ReadLine(), out var value) && value > 0 ? value : fallback;
    }

    static decimal PromptDecimal(string message, decimal fallback)
    {
        Console.Write($"{message}: ");
        return decimal.TryParse(Console.ReadLine(), out var value) && value >= 0 ? value : fallback;
    }

    static DateTime PromptDateTime(string message, DateTime fallback)
    {
        Console.Write($"{message}: ");
        return DateTime.TryParse(Console.ReadLine(), out var value) ? value : fallback;
    }
}
