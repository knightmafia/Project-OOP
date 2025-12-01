/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Represents a reservation created from an accepted quote.
 */

namespace Project_OOP;

public class Reservation : BookingDocument, IPriceable
{
    public string QuoteId { get; }
    public TravelSchedule Schedule { get; }
    public string ServiceSummary { get; }
    public decimal TotalPrice { get; }
    public string? Notes { get; }
    public DateTime ScheduledFor => Schedule.When;
    public decimal Total => TotalPrice;

    public Reservation(string id, string quoteId, string customerId, TravelSchedule schedule, string serviceSummary, decimal totalPrice, string? notes = null, DateTime? createdAt = null)
        : base(id, customerId, createdAt)
    {
        QuoteId = quoteId;
        Schedule = schedule;
        ServiceSummary = serviceSummary;
        TotalPrice = totalPrice;
        Notes = notes;
    }

    public override string Describe()
    {
        return BuildSummary();
    }

    public override string ToString()
    {
        return BuildSummary();
    }

    private string BuildSummary()
    {
        var noteText = string.IsNullOrWhiteSpace(Notes) ? string.Empty : $" | Notes: {Notes}";
        return $"#{Id} | Quote: {QuoteId} | Customer: {CustomerId} | When: {Schedule} | Total: {TotalPrice:C} | {ServiceSummary} | Created: {CreatedAt:g}{noteText}";
    }
}
