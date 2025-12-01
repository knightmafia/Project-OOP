/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Console entry point with in-memory CRUD/menu for customers, services, quotes, and reservations.
 */

using Project_OOP;

Console.WriteLine("Welcome to the Transportation Quote console. Choose an option to begin.");
var random = new Random();

var customers = SeedCustomers();
var services = SeedServices();
var quotes = new List<Quote>();
var reservations = new List<Reservation>();

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
            ListCustomers(customers);
            break;
        case 2:
            AddCustomer(customers, random);
            break;
        case 3:
            ListServices(services);
            break;
        case 4:
            AddService(services, random);
            break;
        case 5:
            CreateQuote(customers, services, quotes, random);
            break;
        case 6:
            UpdateQuoteStatus(quotes);
            break;
        case 7:
            CreateReservation(quotes, reservations, random);
            break;
        case 8:
            ListQuotes(quotes);
            break;
        case 9:
            ListReservations(reservations);
            break;
        case 0:
            running = false;
            break;
        default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
    }
}

static void ListCustomers(List<Customer> customers)
{
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

static void AddCustomer(List<Customer> customers, Random random)
{
    var first = Prompt("First name", "First");
    var last = Prompt("Last name", "Last");
    var phone = Prompt("Phone", "555-0000");
    var email = Prompt("Email", "customer@example.com");
    var id = $"C{random.Next(1000, 9999)}";
    var customer = new Customer(id, first, last, phone, email);
    customers.Add(customer);
    Console.WriteLine($"Added customer {customer}");
}

static void ListServices(List<Service> services)
{
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

static void AddService(List<Service> services, Random random)
{
    var name = Prompt("Service name", "New Service");
    var description = Prompt("Description", string.Empty);
    var rate = PromptDecimal("Rate", 50m);
    var unitChoice = Prompt("Unit type (Flat/Hourly/PerMile)", "Flat");
    var unitType = Enum.TryParse<ServiceUnitType>(unitChoice, true, out var parsed) ? parsed : ServiceUnitType.Flat;

    var id = $"S{random.Next(1000, 9999)}";
    var service = new Service(id, name, description, rate, unitType);
    services.Add(service);
    Console.WriteLine($"Added service {service}");
}

static Customer? ChooseCustomer(List<Customer> customers)
{
    if (customers.Count == 0)
    {
        Console.WriteLine("No customers available. Please add a customer first.");
        return null;
    }

    ListCustomers(customers);
    var input = Prompt("Enter customer ID", customers[0].Id);
    Customer? customer = null;
    foreach (var c in customers)
    {
        if (c.Id.Equals(input, StringComparison.OrdinalIgnoreCase))
        {
            customer = c;
            break;
        }
    }

    if (customer == null)
    {
        Console.WriteLine("Customer not found.");
    }
    return customer;
}

static Service? ChooseService(List<Service> services)
{
    if (services.Count == 0)
    {
        Console.WriteLine("No services available. Please add a service first.");
        return null;
    }

    ListServices(services);
    var input = Prompt("Enter service ID", services[0].Id);
    Service? service = null;
    foreach (var s in services)
    {
        if (s.Id.Equals(input, StringComparison.OrdinalIgnoreCase))
        {
            service = s;
            break;
        }
    }

    if (service == null)
    {
        Console.WriteLine("Service not found.");
    }
    return service;
}

static void CreateQuote(List<Customer> customers, List<Service> services, List<Quote> quotes, Random random)
{
    var customer = ChooseCustomer(customers);
    if (customer == null)
    {
        return;
    }

    var quote = new Quote($"Q{random.Next(1000, 9999)}", customer.Id, DateTime.Now, "Console-generated quote");

    // Outbound
    var service = ChooseService(services);
    if (service == null)
    {
        return;
    }
    var qty = PromptInt("Outbound quantity/units", 1);
    quote.AddItem(new QuoteItem("OI1", quote.Id, service.Id, service.Name, service.Rate, qty));

    // Return (dual booking)
    var addReturn = Prompt("Add a return trip? (y/n)", "n");
    if (addReturn.StartsWith("y", StringComparison.OrdinalIgnoreCase))
    {
        var returnQty = PromptInt("Return quantity/units", 1);
        var returnService = ChooseService(services);
        if (returnService == null)
        {
            returnService = service; // fallback to outbound service
        }
        quote.AddItem(new QuoteItem("RI1", quote.Id, returnService.Id, $"{returnService.Name} (Return)", returnService.Rate, returnQty), isReturnTrip: true);
        Console.WriteLine("Return trip added.");
    }

    quotes.Add(quote);
    Console.WriteLine($"Created quote: {quote}");
}

static void UpdateQuoteStatus(List<Quote> quotes)
{
    if (quotes.Count == 0)
    {
        Console.WriteLine("No quotes available.");
        return;
    }

    ListQuotes(quotes);
    var input = Prompt("Enter quote ID", quotes[0].Id);
    Quote? quote = null;
    foreach (var q in quotes)
    {
        if (q.Id.Equals(input, StringComparison.OrdinalIgnoreCase))
        {
            quote = q;
            break;
        }
    }
    if (quote == null)
    {
        Console.WriteLine("Quote not found.");
        return;
    }

    Console.WriteLine("Quote actions:");
    Console.WriteLine("1) Accept outbound only");
    Console.WriteLine("2) Accept return trip");
    Console.WriteLine("3) Accept all");
    Console.WriteLine("4) Send counter quote (return trip needs changes)");
    Console.WriteLine("5) Reject");
    var action = PromptInt("Choose action", 1);
    switch (action)
    {
        case 1:
            quote.AcceptOutbound();
            Console.WriteLine("Outbound accepted.");
            break;
        case 2:
            quote.AcceptReturnTrip();
            Console.WriteLine("Return trip accepted.");
            break;
        case 3:
            quote.Accept();
            Console.WriteLine("Quote fully accepted.");
            break;
        case 4:
            quote.SendCounterQuote(reviseOutbound: false, reviseReturnTrip: true);
            Console.WriteLine("Counter quote sent for return trip.");
            break;
        case 5:
            quote.Reject();
            Console.WriteLine("Quote rejected.");
            break;
        default:
            Console.WriteLine("No action taken.");
            break;
    }

    Console.WriteLine($"Updated quote: {quote}");
}

static void CreateReservation(List<Quote> quotes, List<Reservation> reservations, Random random)
{
    if (quotes.Count == 0)
    {
        Console.WriteLine("No quotes available.");
        return;
    }

    ListQuotes(quotes);
    var input = Prompt("Enter accepted quote ID", quotes[0].Id);
    Quote? quote = null;
    foreach (var q in quotes)
    {
        if (q.Id.Equals(input, StringComparison.OrdinalIgnoreCase))
        {
            quote = q;
            break;
        }
    }
    if (quote == null)
    {
        Console.WriteLine("Quote not found.");
        return;
    }

    if (quote.Status != QuoteStatus.Accepted)
    {
        Console.WriteLine("Quote must be fully accepted before creating a reservation.");
        return;
    }

    var daysAhead = PromptInt("Schedule how many days from now?", 3);
    var location = Prompt("Pickup location", "Home");
    var instructions = Prompt("Special instructions (optional)", string.Empty);
    var schedule = new TravelSchedule(DateTime.Now.AddDays(daysAhead), location, instructions);

    var reservation = new Reservation(
        id: $"R{random.Next(1000, 9999)}",
        quoteId: quote.Id,
        customerId: quote.CustomerId,
        schedule: schedule,
        serviceSummary: $"Reservation for quote {quote.Id}",
        totalPrice: quote.Total,
        notes: "Created via console menu");

    reservations.Add(reservation);
    Console.WriteLine($"Reservation created: {reservation}");
}

static void ListQuotes(List<Quote> quotes)
{
    if (quotes.Count == 0)
    {
        Console.WriteLine("No quotes available.");
        return;
    }

    Console.WriteLine("\nQuotes:");
    foreach (var q in quotes)
    {
        Console.WriteLine(q);
        foreach (var item in q.AllItems)
        {
            Console.WriteLine($"  - {item}");
        }
    }
}

static void ListReservations(List<Reservation> reservations)
{
    if (reservations.Count == 0)
    {
        Console.WriteLine("No reservations available.");
        return;
    }

    Console.WriteLine("\nReservations:");
    foreach (var r in reservations)
    {
        Console.WriteLine(r);
    }
}

// Seed helpers give quick data for testing without a database.
static List<Customer> SeedCustomers()
{
    var list = new List<Customer>();
    list.Add(new Customer("C1001", "Kim", "Nguyen", "555-0102", "kim@example.com"));
    list.Add(new Customer("C1002", "Alex", "Martinez", "555-7711", "alex@example.com"));
    return list;
}

static List<Service> SeedServices()
{
    var list = new List<Service>();
    list.Add(new Service("S1", "Airport Transfer", "One-way trip to the airport", 95m, ServiceUnitType.Flat));
    list.Add(new Service("S2", "Hourly Charter", "Local charter with driver", 80m, ServiceUnitType.Hourly));
    list.Add(new Service("S3", "Point-to-Point", "Direct ride between two addresses", 55m, ServiceUnitType.Flat));
    return list;
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
