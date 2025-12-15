/*
 * Student: Eric Martinez
 * Date: 12/5/25
 * Assignment: SDC320 Project Class Database Support - Week 4
 * Description: Database helper for Service records.
 */

using Microsoft.Data.Sqlite;
using Project_OOP;

public static class ServiceDB
{
    // Ensures the Service table exists before use.
    public static void CreateTable(SqliteConnection conn)
    {
        const string query = @"
            CREATE TABLE IF NOT EXISTS Service (
                ID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Description TEXT,
                Rate REAL NOT NULL,
                UnitType INTEGER NOT NULL,
                IsActive INTEGER NOT NULL,
                CreatedAt DATETIME NOT NULL
            );";

        using var command = new SqliteCommand(query, conn);
        command.ExecuteNonQuery();
    }

    public static void AddService(SqliteConnection conn, Service s)
    {
        const string query = @"
            INSERT INTO Service (Name, Description, Rate, UnitType, IsActive, CreatedAt)
            VALUES (@name, @desc, @rate, @unit, @active, @createdAt);";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@name", s.Name);
        command.Parameters.AddWithValue("@desc", s.Description ?? string.Empty);
        command.Parameters.AddWithValue("@rate", s.Rate);
        command.Parameters.AddWithValue("@unit", (int)s.UnitType);
        command.Parameters.AddWithValue("@active", s.IsActive ? 1 : 0);
        command.Parameters.AddWithValue("@createdAt", DateTime.Now);
        command.ExecuteNonQuery();
    }

    // Updates a service row by primary key; returns true when a row was changed.
    public static bool UpdateService(SqliteConnection conn, int id, string name, string description, decimal rate, ServiceUnitType unitType, bool isActive)
    {
        const string query = @"
            UPDATE Service
            SET Name = @name,
                Description = @desc,
                Rate = @rate,
                UnitType = @unit,
                IsActive = @active
            WHERE ID = @id;";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@name", name);
        command.Parameters.AddWithValue("@desc", description ?? string.Empty);
        command.Parameters.AddWithValue("@rate", rate);
        command.Parameters.AddWithValue("@unit", (int)unitType);
        command.Parameters.AddWithValue("@active", isActive ? 1 : 0);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }

    // Retrieves a service by primary key; returns null when absent.
    public static Service? GetServiceById(SqliteConnection conn, int id)
    {
        const string query = @"
            SELECT ID, Name, Description, Rate, UnitType, IsActive
            FROM Service
            WHERE ID = @id;";

        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@id", id);
        using var reader = command.ExecuteReader();
        if (!reader.Read())
        {
            return null;
        }

        var name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
        var description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
        var rate = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3);
        var unitTypeValue = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
        var unitType = Enum.IsDefined(typeof(ServiceUnitType), unitTypeValue)
            ? (ServiceUnitType)unitTypeValue
            : ServiceUnitType.Flat;
        var isActive = !reader.IsDBNull(5) && reader.GetInt32(5) == 1;

        return new Service(id.ToString(), name, description, rate, unitType, isActive);
    }

    // Deletes a service by primary key; returns true when a row was removed.
    public static bool DeleteService(SqliteConnection conn, int id)
    {
        const string query = @"DELETE FROM Service WHERE ID = @id;";
        using var command = new SqliteCommand(query, conn);
        command.Parameters.AddWithValue("@id", id);
        return command.ExecuteNonQuery() > 0;
    }

    // Fetches all services ordered by their primary key.
    public static List<Service> GetAllServices(SqliteConnection conn)
    {
        const string query = @"
            SELECT ID, Name, Description, Rate, UnitType, IsActive
            FROM Service
            ORDER BY ID;";

        using var command = new SqliteCommand(query, conn);
        using var reader = command.ExecuteReader();

        var services = new List<Service>();
        while (reader.Read())
        {
            var id = reader.GetInt32(0).ToString();
            var name = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
            var description = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
            var rate = reader.IsDBNull(3) ? 0m : reader.GetDecimal(3);
            var unitTypeValue = reader.IsDBNull(4) ? 0 : reader.GetInt32(4);
            var unitType = Enum.IsDefined(typeof(ServiceUnitType), unitTypeValue)
                ? (ServiceUnitType)unitTypeValue
                : ServiceUnitType.Flat;
            var isActive = !reader.IsDBNull(5) && reader.GetInt32(5) == 1;

            services.Add(new Service(id, name, description, rate, unitType, isActive));
        }

        return services;
    }
}
