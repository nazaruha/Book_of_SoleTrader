using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Worker_DB.Models;
using Bogus;
using System.Data;

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

            //GenerateTables();
            GenerateManufacturers();
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

        static void GenerateTables()
        {
            string[] tables = { "tblCustomers.sql", "tblManufacturers.sql", "tblGroceries.sql", "tblStorage.sql", "tblNotebook.sql" };
            foreach (var table in tables)
            {
                ExecuteCommandFromFile(table);
            }
        }

        static void ExecuteCommandFromFile(string file)
        {
            string sql = ReadSqlFile(file);
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        static string ReadSqlFile(string file)
        {
            string sql = File.ReadAllText($"{dirSql}\\{file}");
            return sql;
        }

        static void GenerateManufacturers()
        {
            string json = File.ReadAllText("JSON_Objects\\Manufacturers.json");
            List<Manufacturer> name = JsonConvert.DeserializeObject<List<Manufacturer>>(json);

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add(new DataColumn(nameof(Manufacturer.Name)));

            for (int i = 0; i < name.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["Id"] = 0;
                row[nameof(Manufacturer.Name)] = name[i].Name;
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblManufacturers";
                bulkCopy.WriteToServer(dt);
            }
            Console.WriteLine("Table is generated");
        }

    }
}
