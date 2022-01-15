using System;
using System.Data.SQLite;

namespace TestSQLite
{
    class Program
    {
        static void Main(string[] args)
        {
            SQLiteConnection sqlite_conn;
            sqlite_conn = CreateConnection();
            CreateTable(sqlite_conn);
            InsertData(sqlite_conn);
            ReadData(sqlite_conn);
            Console.ReadKey();
        }
        static SQLiteConnection CreateConnection()
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=database.db; Version = 3; New = True; Compress = True; ");
         // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        static void CreateTable(SQLiteConnection conn)
        {

            SQLiteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE Person(Name VARCHAR(20), Age INT)";
            string Createsql1 = "CREATE TABLE Emp(Name VARCHAR(20), Age INT)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = Createsql1;
            sqlite_cmd.ExecuteNonQuery();

        }

        static void InsertData(SQLiteConnection conn)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO Person(Name, Age) VALUES('fgfjh ', 1); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO Person(Name, Age) VALUES('fhgjh', 2); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO Person(Name, Age) VALUES('fghfhj', 3); ";
            sqlite_cmd.ExecuteNonQuery();


            sqlite_cmd.CommandText = "INSERT INTO Emp(Name, Age) VALUES('hfjhjk', 3); ";
            sqlite_cmd.ExecuteNonQuery();

        }

        static void ReadData(SQLiteConnection conn)
        {
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM Person";

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }

    }
}
