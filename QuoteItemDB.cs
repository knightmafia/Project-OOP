/*
 * Student: Eric Martinez
 * Date: 12/5/25
 * Assignment: SDC320 Project Class Database Support - Week 4
 * Description: Database helper for QuoteItem records.
 */

using Microsoft.Data.Sqlite;
using Project_OOP;

public static class QuoteItemDB
{
    public static void CreateTable(SqliteConnection conn)
    {
        const string query = @"
            CREATE TABLE IF NOT EXISTS QuoteItem (
                ID TEXT PRIMARY KEY,
                QuoteId TEXT NOT NULL,
                ServiceId TEXT NOT NULL,
                ServiceName TEXT NOT NULL,
                UnitRate REAL NOT NULL,
                Quantity INTEGER NOT NULL,
                LineTotal REAL NOT NULL,
                IsReturnTrip INTEGER NOT NULL
            );";

        using var command = new SqliteCommand(query, conn);
        command.ExecuteNonQuery();
    }

    public static void AddQuoteItem(SqliteConnection conn, QuoteItem item, bool isReturnTrip = false)
    {
        const string query = @"
            INSERT INTO QuoteItem (ID, QuoteId, ServiceId, ServiceName, UnitRate, Quantity, LineTotal, IsReturnTrip)
            VALUES (@id, @quoteId, @serviceId, @serviceName, @unitRate, @qty, @lineTotal, @isReturn);";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@id", item.Id);
        command.Parameters.AddWithValue("@quoteId", item.QuoteId);
        command.Parameters.AddWithValue("@serviceId", item.ServiceId);
        command.Parameters.AddWithValue("@serviceName", item.ServiceName);
        command.Parameters.AddWithValue("@unitRate", item.UnitRate);
        command.Parameters.AddWithValue("@qty", item.Quantity);
        command.Parameters.AddWithValue("@lineTotal", item.LineTotal);
        command.Parameters.AddWithValue("@isReturn", isReturnTrip ? 1 : 0);
        command.ExecuteNonQuery();
    }

    public static (List<QuoteItem> outbound, List<QuoteItem> returns) GetItemsForQuote(SqliteConnection conn, string quoteId)
    {
        const string query = @"
            SELECT ID, QuoteId, ServiceId, ServiceName, UnitRate, Quantity, LineTotal, IsReturnTrip
            FROM QuoteItem
            WHERE QuoteId = @quoteId
            ORDER BY ID;";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@quoteId", quoteId);

        var outbound = new List<QuoteItem>();
        var returns = new List<QuoteItem>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var id = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            var qid = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var serviceId = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var serviceName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);
            var unitRate = reader.IsDBNull(4) ? 0m : reader.GetDecimal(4);
            var qty = reader.IsDBNull(5) ? 0 : reader.GetInt32(5);
            var isReturn = !reader.IsDBNull(7) && reader.GetInt32(7) == 1;
            var item = new QuoteItem(id, qid, serviceId, serviceName, unitRate, qty);
            if (isReturn)
            {
                returns.Add(item);
            }
            else
            {
                outbound.Add(item);
            }
        }

        return (outbound, returns);
    }
}
