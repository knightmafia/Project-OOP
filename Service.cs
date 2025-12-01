/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Represents a service that can be quoted/reserved.
 */

namespace Project_OOP;

public enum ServiceUnitType
{
    Flat,
    Hourly,
    PerMile
}

public class Service : IIdentifiable
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public decimal Rate { get; }
    public ServiceUnitType UnitType { get; }
    public bool IsActive { get; private set; }

    public Service(string id, string name, string description, decimal rate, ServiceUnitType unitType, bool isActive = true)
    {
        Id = id;
        Name = name;
        Description = description;
        Rate = rate;
        UnitType = unitType;
        IsActive = isActive;
    }

    public decimal PriceFor(int quantity)
    {
        return Rate * quantity;
    }

    public void SetActive(bool isActive)
    {
        IsActive = isActive;
    }

    public override string ToString()
    {
        var unitText = FormatUnit(UnitType);
        var status = IsActive ? string.Empty : " | Inactive";
        var description = string.IsNullOrWhiteSpace(Description) ? string.Empty : $" | {Description}";
        return $"#{Id} | {Name} | {Rate:C} {unitText}{description}{status}";
    }

    private static string FormatUnit(ServiceUnitType unitType)
    {
        return unitType switch
        {
            ServiceUnitType.Flat => "flat",
            ServiceUnitType.Hourly => "hourly",
            ServiceUnitType.PerMile => "per-mile",
            _ => unitType.ToString().ToLowerInvariant()
        };
    }
}
