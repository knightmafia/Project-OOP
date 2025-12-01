/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Encapsulates scheduling details for a reservation.
 */

namespace Project_OOP;

// Encapsulates scheduling details for a reservation (composition).
public class TravelSchedule
{
    public DateTime When { get; }
    public string Location { get; }
    public string Instructions { get; }

    public TravelSchedule(DateTime when, string location, string instructions = "")
    {
        When = when;
        Location = location;
        Instructions = instructions;
    }

    public override string ToString()
    {
        var extra = string.IsNullOrWhiteSpace(Instructions) ? string.Empty : $" | Note: {Instructions}";
        return $"{When:g} @ {Location}{extra}";
    }
}
