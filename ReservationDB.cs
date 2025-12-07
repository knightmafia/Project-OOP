/*
 * Student: Eric Martinez
 * Date: 12/5/25
 * Assignment: SDC320 Project Class Database Support - Week 4
 * Description: Database helper for Reservation records.
 */

using Microsoft.Data.Sqlite;
using Project_OOP;

public static class ReservationDB
{
    public static void CreateTable(SqliteConnection conn)
    {
        const string query = @"
            CREATE TABLE IF NOT EXISTS Reservation (
                ID TEXT PRIMARY KEY,
                QuoteId TEXT NOT NULL,
                CustomerId TEXT NOT NULL,
                WhenAt DATETIME NOT NULL,
                Location TEXT NOT NULL,
                Instructions TEXT,
                ServiceSummary TEXT NOT NULL,
                TotalPrice REAL NOT NULL,
                IsReturnTrip INTEGER NOT NULL,
                Notes TEXT,
                CreatedAt DATETIME NOT NULL
            );";

        using var command = new SqliteCommand(query, conn);
        command.ExecuteNonQuery();
    }

    public static void AddReservation(SqliteConnection conn, Reservation r)
    {
        const string query = @"
            INSERT INTO Reservation (ID, QuoteId, CustomerId, WhenAt, Location, Instructions, ServiceSummary, TotalPrice, IsReturnTrip, Notes, CreatedAt)
            VALUES (@id, @quoteId, @customerId, @whenAt, @location, @instructions, @summary, @total, @isReturn, @notes, @createdAt);";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@id", r.Id);
        command.Parameters.AddWithValue("@quoteId", r.QuoteId);
        command.Parameters.AddWithValue("@customerId", r.CustomerId);
        command.Parameters.AddWithValue("@whenAt", r.Schedule.When);
        command.Parameters.AddWithValue("@location", r.Schedule.Location);
        command.Parameters.AddWithValue("@instructions", r.Schedule.Instructions ?? string.Empty);
        command.Parameters.AddWithValue("@summary", r.ServiceSummary);
        command.Parameters.AddWithValue("@total", r.TotalPrice);
        command.Parameters.AddWithValue("@isReturn", r.IsReturnTrip ? 1 : 0);
        command.Parameters.AddWithValue("@notes", (object?)r.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@createdAt", r.CreatedAt);
        command.ExecuteNonQuery();
    }

    public static List<Reservation> GetAllReservations(SqliteConnection conn)
    {
        const string query = @"
            SELECT ID, QuoteId, CustomerId, WhenAt, Location, Instructions, ServiceSummary, TotalPrice, IsReturnTrip, Notes, CreatedAt
            FROM Reservation
            ORDER BY WhenAt;";

        using var command = new SqliteCommand(query, conn);
        using var reader = command.ExecuteReader();

        var reservations = new List<Reservation>();
        while (reader.Read())
        {
            var id = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            var quoteId = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var customerId = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var whenAt = reader.IsDBNull(3) ? DateTime.Now : reader.GetDateTime(3);
            var location = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);
            var instructions = reader.IsDBNull(5) ? string.Empty : reader.GetString(5);
            var summary = reader.IsDBNull(6) ? string.Empty : reader.GetString(6);
            var totalPrice = reader.IsDBNull(7) ? 0m : reader.GetDecimal(7);
            var isReturn = !reader.IsDBNull(8) && reader.GetInt32(8) == 1;
            var notes = reader.IsDBNull(9) ? null : reader.GetString(9);
            var createdAt = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10);

            var schedule = new TravelSchedule(whenAt, location, instructions);
            var reservation = new Reservation(id, quoteId, customerId, schedule, summary, totalPrice, isReturn, notes, createdAt);
            reservations.Add(reservation);
        }

        return reservations;
    }
}
