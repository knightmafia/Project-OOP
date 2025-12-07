/****************************************
* Name: Eric Martinez
* Date: 12/05/2025
* Assignment: SDC320 Project Class Database Support - Week 4
*
* Class to handle databse interactions with a SQLite database. The
* connect method will either connect to an existing database or
* create the database if the database doesn't exist.
*/

using Microsoft.Data.Sqlite;

public class SQLiteDatabase
{
    public static SqliteConnection Connect(string database)
    {
        string cs = @"Data Source=" + database;
        SqliteConnection conn = new SqliteConnection(cs);

        try
        {
            conn.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return conn;
    }
}