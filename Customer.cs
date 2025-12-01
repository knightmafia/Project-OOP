/*
 * Student: Eric Martinez
 * Date: 11/25/25
 * Assignment: SDC320 Project Class Implementation - Week 3
 * Description: Stores customer contact information.
 */

namespace Project_OOP;

public class Customer
{
    public string Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string PhoneNumber { get; }
    public string Email { get; }

    public Customer(string id, string firstName, string lastName, string phoneNumber, string email)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
    }

    public override string ToString()
    {
        return $"#{Id} | {FirstName} {LastName} | {PhoneNumber} | {Email}";
    }
}
