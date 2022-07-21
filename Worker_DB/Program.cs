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
        static string dirJson = "JSON_Objects";
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

            GenerateTables();
            GenerateManufacturers();
            GenerateGroceries();
            GenerateStorage();
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
            string json = File.ReadAllText($"{dirJson}\\Manufacturers.json");
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

        static void GenerateGroceries()
        {
            string json = File.ReadAllText($"{dirJson}\\Groceries.json");
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(json);

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add(new DataColumn(nameof(Product.Name)));

            for (int i = 0; i < products.Count; i++)
            {
                DataRow row = dt.NewRow();
                row["Id"] = 0;
                row[nameof(Product.Name)] = products[i].Name;
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblGroceries";
                bulkCopy.WriteToServer(dt);
            }
            Console.WriteLine("Table is generated");
        }

        static private void GenerateStorage()
        {
            List<int> groceries = GetGroceriesId();
            List<int> manufacturers = GetManufacturersId();

            List<Storage> storages = new List<Storage>();
            Random rand = new Random();
            Faker faker = new Faker();
            foreach (var manufacturer in manufacturers)
            {
                int productCount = rand.Next(1, groceries.Count + 1);
                List<int> checkIds = new List<int>();
                checkIds.Clear();
                for (int i = 0; i < productCount; i++)
                {
                    int productId = rand.Next(0, groceries.Count);
                    if (checkIds.Contains(productId)) continue;
                    checkIds.Add(productId);
                    storages.Add(new Storage() { ManufacturerId = manufacturer, ProductId = groceries[productId], Price = (int)faker.Finance.Amount(1, 500, 1), Count = (int)faker.Finance.Amount() });
                }
            }

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(nameof(Storage.ManufacturerId)));
            dt.Columns.Add(new DataColumn(nameof(Storage.ProductId)));
            dt.Columns.Add(new DataColumn(nameof(Storage.Price)));
            dt.Columns.Add(new DataColumn(nameof(Storage.Count)));

            for (int i = 0; i < storages.Count; i++)
            {
                DataRow row = dt.NewRow();
                row[nameof(Storage.ManufacturerId)] = storages[i].ManufacturerId;
                row[nameof(Storage.ProductId)] = storages[i].ProductId;
                row[nameof(Storage.Price)] = storages[i].Price;
                row[nameof(Storage.Count)] = storages[i].Count;
                dt.Rows.Add(row);
            }
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(con))
            {
                bulkCopy.DestinationTableName = "tblStorage";
                bulkCopy.WriteToServer(dt);
            }
        }

        static private List<int> GetGroceriesId()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblGroceries";
            List<int> groceriesId = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    groceriesId.Add(Int32.Parse(reader["Id"].ToString()));
                }
            }
            return groceriesId;
        }

        static private List<int> GetManufacturersId()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblManufacturers";
            List<int> manufacturersId = new List<int>();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    manufacturersId.Add(Int32.Parse(reader["Id"].ToString()));
                }
            }
            return manufacturersId;
        }

    }
}
