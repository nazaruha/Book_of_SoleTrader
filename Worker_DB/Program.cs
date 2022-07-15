using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker_DB
{
    public class Program
    {
        static string DbName = "РРО2";
        static string dirSql = "SqlTables";
        static SqlConnection con;
        static SqlCommand cmd;

        static void Main(string[] args)
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionDB = configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(connectionDB);
            con.Open();

            if (!IsDBExists())
            {
                cmd.CommandText = $"CREATE DATABASE {DbName}";
                cmd.ExecuteNonQuery();
                Console.WriteLine(con.ToString());
            }
            con = new SqlConnection(connectionDB + $"Initial Catalog={DbName}");
            con.Open();
            cmd = con.CreateCommand();
        }

        static bool IsDBExists()
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT name FROM master.sys.databases";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString() == DbName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
