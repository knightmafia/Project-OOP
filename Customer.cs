/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Stores customer contact information.
 */

namespace Project_OOP;

public class Customer
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }

    public Customer(int id, string firstName, string lastName, string phoneNumber, string email, DateTime createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        CreatedAt = createdAt;
    }

    public Customer(string firstName, string lastName, string phoneNumber, string email, DateTime createdAt)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        CreatedAt = createdAt;
    }
    
    public override string ToString()
    {
        return $"#{Id} | {FirstName} {LastName} | {PhoneNumber} | {Email} | {CreatedAt}";
    }
}
