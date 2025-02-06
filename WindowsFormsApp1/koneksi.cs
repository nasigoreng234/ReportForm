using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace WindowsFormsApp1
{
    internal class koneksi
    {
        private static string connectionString = "Data source=DESKTOP-H0DAI5O;Initial catalog=EsemkaVote;Integrated Security=True;";

        public static SqlConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected to database.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error: " + e.Message);
                }

            return connection;

            }
        }
    }
}
