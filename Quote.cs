/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Represents a quote with dual booking and counter-quote capabilities.
 */

using System.Linq;
using System.Text;

namespace Project_OOP;

public enum QuoteStatus
{
    Pending,
    Accepted,
    Rejected,
    PendingCustomerReview
}

public class Quote : BookingDocument, IPriceable
{
    public QuoteStatus Status { get; private set; }
    public List<QuoteItem> OutboundItems { get; } = new();
    public List<QuoteItem> ReturnTripItems { get; } = new();
    public IEnumerable<QuoteItem> AllItems => OutboundItems.Concat(ReturnTripItems);
    public bool HasReturnTrip => ReturnTripItems.Count > 0;
    public bool OutboundApproved { get; private set; }
    public bool ReturnApproved { get; private set; }
    public decimal OutboundTotal { get; private set; }
    public decimal ReturnTotal { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Total { get; private set; }
    public string? Notes { get; private set; }

    public Quote(string id, string customerId, DateTime? createdAt = null, string? notes = null)
        : base(id, customerId, createdAt)
    {
        Notes = notes;
        Status = QuoteStatus.Pending;
    }

    public void AddItem(QuoteItem item)
    {
        AddItem(item, false);
    }

    public void AddItem(QuoteItem item, bool isReturnTrip)
    {
        if (isReturnTrip)
        {
            ReturnTripItems.Add(item);
        }
        else
        {
            OutboundItems.Add(item);
        }

        CalculateTotal();
    }

    public bool RemoveItem(string quoteItemId)
    {
        var removed = OutboundItems.RemoveAll(i => i.Id == quoteItemId) > 0;
        removed = ReturnTripItems.RemoveAll(i => i.Id == quoteItemId) > 0 || removed;
        if (removed)
        {
            CalculateTotal();
        }

        return removed;
    }

    public decimal CalculateTotal()
    {
        OutboundTotal = OutboundItems.Sum(i => i.CalculateLineTotal());
        ReturnTotal = ReturnTripItems.Sum(i => i.CalculateLineTotal());
        Subtotal = OutboundTotal + ReturnTotal;
        Total = Subtotal;
        return Total;
    }

    public void Accept()
    {
        OutboundApproved = true;
        ReturnApproved = true;
        Status = QuoteStatus.Accepted;
    }

    public void AcceptOutbound()
    {
        OutboundApproved = true;
        Status = HasReturnTrip && !ReturnApproved ? QuoteStatus.PendingCustomerReview : QuoteStatus.Accepted;
    }

    public void AcceptReturnTrip()
    {
        ReturnApproved = true;
        Status = OutboundApproved ? QuoteStatus.Accepted : QuoteStatus.PendingCustomerReview;
    }

    public void Reject()
    {
        Status = QuoteStatus.Rejected;
    }

    public void SendCounterQuote(bool reviseOutbound = false, bool reviseReturnTrip = false)
    {
        if (reviseOutbound)
        {
            OutboundApproved = false;
        }

        if (reviseReturnTrip)
        {
            ReturnApproved = false;
        }

        Status = QuoteStatus.PendingCustomerReview;
    }

    public void UpdateNotes(string? notes)
    {
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
        var builder = new StringBuilder();
        var totalItems = OutboundItems.Count + ReturnTripItems.Count;
        builder.Append($"#{Id} | {FormatStatus(Status)} | Items: {totalItems} | Total: {Total:C}");
        if (HasReturnTrip)
        {
            builder.Append(" | Return Trip: Yes");
        }
        if (!string.IsNullOrWhiteSpace(Notes))
        {
            builder.Append($" | Notes: {Notes}");
        }
        builder.Append($" | Created: {CreatedAt:g}");

        return builder.ToString();
    }

    private static string FormatStatus(QuoteStatus status)
    {
        return status switch
        {
            QuoteStatus.PendingCustomerReview => "Pending Customer Review",
            _ => status.ToString()
        };
    }
}
