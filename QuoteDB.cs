/*
 * Student: Eric Martinez
 * Date: 12/5/25
 * Assignment: SDC320 Project Class Database Support - Week 4
 * Description: Database helper for Quote records.
 */

using Microsoft.Data.Sqlite;
using Project_OOP;

public static class QuoteDB
{
    public static void CreateTable(SqliteConnection conn)
    {
        const string query = @"
            CREATE TABLE IF NOT EXISTS Quote (
                ID TEXT PRIMARY KEY,
                CustomerId TEXT NOT NULL,
                CreatedAt DATETIME NOT NULL,
                Status INTEGER NOT NULL,
                Version INTEGER NOT NULL,
                Notes TEXT,
                ReturnDateTime DATETIME,
                ReturnPickupLocation TEXT
            );";

        using var command = new SqliteCommand(query, conn);
        command.ExecuteNonQuery();
    }

    public static void AddQuote(SqliteConnection conn, Quote q)
    {
        const string query = @"
            INSERT INTO Quote (ID, CustomerId, CreatedAt, Status, Version, Notes, ReturnDateTime, ReturnPickupLocation)
            VALUES (@id, @customerId, @createdAt, @status, @version, @notes, @returnDateTime, @returnPickup);";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@id", q.Id);
        command.Parameters.AddWithValue("@customerId", q.CustomerId);
        command.Parameters.AddWithValue("@createdAt", q.CreatedAt);
        command.Parameters.AddWithValue("@status", (int)q.Status);
        command.Parameters.AddWithValue("@version", q.Version);
        command.Parameters.AddWithValue("@notes", (object?)q.Notes ?? DBNull.Value);
        command.Parameters.AddWithValue("@returnDateTime", (object?)q.ReturnDateTime ?? DBNull.Value);
        command.Parameters.AddWithValue("@returnPickup", (object?)q.ReturnPickupLocation ?? DBNull.Value);
        command.ExecuteNonQuery();
    }

    public static List<Quote> GetAllQuotes(SqliteConnection conn)
    {
        const string query = @"
            SELECT ID, CustomerId, CreatedAt, Status, Version, Notes, ReturnDateTime, ReturnPickupLocation
            FROM Quote
            ORDER BY CreatedAt;";

        using var command = new SqliteCommand(query, conn);
        using var reader = command.ExecuteReader();

        var quotes = new List<Quote>();
        while (reader.Read())
        {
            var id = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            var customerId = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var createdAt = reader.IsDBNull(2) ? DateTime.Now : reader.GetDateTime(2);
            var statusValue = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);
            var version = reader.IsDBNull(4) ? 1 : reader.GetInt32(4);
            var notes = reader.IsDBNull(5) ? null : reader.GetString(5);
            var returnDateTime = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
            var returnPickup = reader.IsDBNull(7) ? null : reader.GetString(7);

            var quote = new Quote(id, customerId, createdAt, notes);
            quote.LoadStatus(Enum.IsDefined(typeof(QuoteStatus), statusValue) ? (QuoteStatus)statusValue : QuoteStatus.Pending, version);
            if (returnDateTime.HasValue || !string.IsNullOrWhiteSpace(returnPickup))
            {
                quote.SetReturnDetails(returnDateTime, returnPickup);
            }

            // Hydrate items from QuoteItem table so totals/states are accurate.
            var items = QuoteItemDB.GetItemsForQuote(conn, id);
            foreach (var item in items.outbound)
            {
                quote.AddItem(item, false);
            }
            foreach (var item in items.returns)
            {
                quote.AddItem(item, true);
            }

            quotes.Add(quote);
        }

        return quotes;
    }

    public static void UpdateQuoteStatus(SqliteConnection conn, string id, QuoteStatus status, int? version = null)
    {
        const string query = @"
            UPDATE Quote
            SET Status = @status,
                Version = COALESCE(@version, Version)
            WHERE ID = @id;";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@status", (int)status);
        command.Parameters.AddWithValue("@version", version.HasValue ? version.Value : DBNull.Value);
        command.Parameters.AddWithValue("@id", id);
        command.ExecuteNonQuery();
    }
}
