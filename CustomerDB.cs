/*
 * Student: Eric Martinez
 * Date: 12/5/25
 * Assignment: SDC320 Project Class Database Support - Week 4
 * Description: Database helper for Customer records.
 */

using Microsoft.Data.Sqlite;
using Project_OOP;

public static class CustomerDB
{
    public static void CreateTable(SqliteConnection conn)
    {
        const string query = @"
            CREATE TABLE IF NOT EXISTS Customer (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                FirstName TEXT NOT NULL,
                LastName TEXT NOT NULL,
                PhoneNumber TEXT NOT NULL,
                Email TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

        using var command = new SqliteCommand(query, conn);
        command.ExecuteNonQuery();
    }

    public static void AddCustomer(SqliteConnection conn, Customer c)
    {
        const string query = @"
            INSERT INTO Customer (FirstName, LastName, PhoneNumber, Email, CreatedAt)
            VALUES (@fname, @lname, @pnum, @eml, @createdat);";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@fname", c.FirstName);
        command.Parameters.AddWithValue("@lname", c.LastName);
        command.Parameters.AddWithValue("@pnum", c.PhoneNumber);
        command.Parameters.AddWithValue("@eml", c.Email);
        command.Parameters.AddWithValue("@createdat", DateTime.Now);
        command.ExecuteNonQuery();
    }

    public static List<Customer> GetAllCustomers(SqliteConnection conn)
    {
        const string query = @"
            SELECT ID, FirstName, LastName, PhoneNumber, Email, CreatedAt
            FROM Customer
            ORDER BY ID;";

        using var command = new SqliteCommand(query, conn);
        using var reader = command.ExecuteReader();

        var customers = new List<Customer>();
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var first = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var last = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var phone = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            var email = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
            var createdAt = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5);

            customers.Add(new Customer(id, first, last, phone, email, createdAt));
        }

        return customers;
    }
}
