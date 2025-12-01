/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Represents a line item within a quote.
 */

using System.Globalization;

namespace Project_OOP;

public class QuoteItem : IIdentifiable
{
    public string Id { get; }
    public string QuoteId { get; }
    public string ServiceId { get; }
    public string ServiceName { get; }
    public decimal UnitRate { get; }
    public int Quantity { get; private set; }
    public decimal LineTotal { get; private set; }

    public QuoteItem(string id, string quoteId, string serviceId, string serviceName, decimal unitRate, int quantity)
    {
        Id = id;
        QuoteId = quoteId;
        ServiceId = serviceId;
        ServiceName = serviceName;
        UnitRate = unitRate;
        Quantity = quantity;
        LineTotal = CalculateLineTotal();
    }

    public decimal CalculateLineTotal()
    {
        LineTotal = UnitRate * Quantity;
        return LineTotal;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
        CalculateLineTotal();
    }

    public override string ToString()
    {
        return $"#{Id} | {ServiceName} | Qty: {Quantity} | Rate: {UnitRate.ToString("C", CultureInfo.CurrentCulture)} | Line: {LineTotal.ToString("C", CultureInfo.CurrentCulture)}";
    }
}
