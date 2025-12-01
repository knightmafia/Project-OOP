/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Abstract base for booking documents and shared interfaces.
 */

namespace Project_OOP;

// Shared base for documents (quotes, reservations) to enable polymorphic behavior.
public abstract class BookingDocument : IIdentifiable
{
    public string Id { get; }
    public string CustomerId { get; }
    public DateTime CreatedAt { get; }

    protected BookingDocument(string id, string customerId, DateTime? createdAt = null)
    {
        Id = id;
        CustomerId = customerId;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }

    public abstract string Describe();
}

public interface IIdentifiable
{
    string Id { get; }
}

public interface IPriceable
{
    decimal Total { get; }
}
