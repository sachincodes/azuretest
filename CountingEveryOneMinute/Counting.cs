using System;
using System.Data.SqlClient;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace CountingEveryOneMinute
{
    public class Counting
    {
        public static string _connection = "Server=tcp:azuretestingdbsachin.database.windows.net,1433;Initial Catalog=countingdb;Persist Security Info=False;User ID=sachin;Password=s@chin@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        public int count = 0;
        [FunctionName("Counting")]
        public void Run([TimerTrigger("0 */1 * * * *")]TimerInfo myTimer, ILogger log)
        {
            CreateTable();
            count++;
            AddEmailEntryToTheDatabase();
        }

        private void CreateTable()
        {
            using SqlConnection connection = new(_connection);
            string query = @"IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'Counting')
                            BEGIN
                                CREATE TABLE Counting
                                (
                                    Id bigint primary key identity(1,1),
                                    Count int not null,
                                )
                            END";
            SqlCommand command = new(query, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void AddEmailEntryToTheDatabase()
        {
            using SqlConnection connection = new(_connection);
            string query = @"INSERT INTO Counting(Count)
                             VALUES(@Count)";
            SqlCommand command = new(query, connection);
            command.Parameters.AddWithValue("@Count", count);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
