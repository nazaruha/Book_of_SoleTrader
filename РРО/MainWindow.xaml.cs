using Microsoft.Extensions.Configuration;
using System;
using РРО.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace РРО
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string database { get; set; } = "РРО";
        private string dirScripts { get; set; } = "Scripts";
        public SqlConnection con { get; set; }
        public SqlCommand cmd { get; set; }
        private int MaxGroceriesInPage { get; set; } = 20;
        private int pages { get; set; }
        private int start { get; set; }
        private int currentPage { get; set; } = 1;

        public MainWindow()
        {
            InitializeComponent();
            OpenDbWorker();
            GetPages();
            GetRangeNotebook();
        }

        private void OpenDbWorker()
        {
            Console.InputEncoding = Encoding.Unicode;
            Console.OutputEncoding = Encoding.Unicode;

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();
            string connectionDB = configuration.GetConnectionString("DefaultConnection");
            con = new SqlConnection(connectionDB + $"Initial Catalog={database}");
            con.Open();
            cmd = con.CreateCommand();
        }

        private void GetNotebook()
        {
            string script = File.ReadAllText($"{dirScripts}\\viewtblNotebook.sql");
            cmd.CommandText = script;
            InitializeDataGrid();
            
        }

        private void GetRangeNotebook(int start = 0)
        {
            this.start = start;
            string script = File.ReadAllText($"{dirScripts}\\viewtblNotebook_OFFSET.sql");
            cmd.CommandText = script;
            cmd.Parameters.AddWithValue(@"@start", start);
            cmd.Parameters.AddWithValue(@"@end", MaxGroceriesInPage);
            InitializeDataGrid();
        }

        private void InitializeDataGrid()
        {
            dgNoteBook.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Sell sell = new Sell() { Id = int.Parse(reader["Id"].ToString()), Date = ((DateTime)reader["Date"]).ToShortDateString(), Manufacturer = reader["Manufacturer"].ToString(), Product = reader["Product"].ToString(), Count = int.Parse(reader["Count"].ToString()), TotalSum = int.Parse(reader["TotalSum"].ToString()), Customer = reader["Customer"].ToString(), Phone = reader["Phone"].ToString() };
                    try
                    {
                        sell.Discount = int.Parse(reader["Discount"].ToString());
                    }
                    catch
                    {
                        sell.Discount = 0;
                    }
                    dgNoteBook.Items.Add(sell);
                }
            }
            cmd.Parameters.Clear();
            PageOfPages_txt.Text = $"{currentPage} of {pages}";
        }

        private void GetPages()
        {
            cmd.CommandText = "SELECT COUNT(*) FROM tblNotebook";
            int SalesCount = Int32.Parse(cmd.ExecuteScalar().ToString());
            if (SalesCount < MaxGroceriesInPage)
            {
                pages = 1;
                return;
            }
            pages = SalesCount / MaxGroceriesInPage;
            int extra = SalesCount % MaxGroceriesInPage;
            if (extra != 0)
            {
                pages++;
            }
        }

        private int GetManufacturerId()
        {
            if (dgNoteBook.SelectedIndex == -1) return -1;
            cmd.CommandText = "SELECT Id " +
                "FROM tblManufacturers " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"NameField", (dgNoteBook.SelectedItem as Sell).Manufacturer);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                int id = int.Parse(reader["Id"].ToString());
                cmd.Parameters.Clear();
                return id;
            }
        }

        private int GetProductId()
        {
            if (dgNoteBook.SelectedIndex == -1) return -1;
            cmd.CommandText = "SELECT Id " +
                "FROM tblGroceries " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"@NameField", (dgNoteBook.SelectedItem as Sell).Product);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                int id = int.Parse(reader["Id"].ToString());
                cmd.Parameters.Clear();
                return id;
            }
        }

        private int GetCustomerId()
        {
            if (dgNoteBook.SelectedIndex == -1) return -1;
            cmd.CommandText = "SELECT Id " +
                "FROM tblCustomers " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"@NameField", (dgNoteBook.SelectedItem as Sell).Customer);

            int id = 0;
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                id = int.Parse(reader["Id"].ToString());
                cmd.Parameters.Clear();
            }
            return id;
        }

        private bool CheckStorageProduct()
        {
            int manufacturerId = GetManufacturerId();
            int productId = GetProductId();

            cmd.CommandText = "SELECT * " +
                "FROM tblStorage " +
                @"WHERE ManufacturerId = @ManuIdField AND ProductId = @ProdIdField";
            cmd.Parameters.AddWithValue(@"@ManuIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProdIdField", productId);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                try
                {
                    int price = int.Parse(reader["Price"].ToString());
                    cmd.Parameters.Clear();
                    return true;
                }
                catch
                {
                    cmd.Parameters.Clear();
                    return false;
                }
            }
        }

        private int GetSellCount()
        {
            int manufacturerId = GetManufacturerId();
            int productId = GetProductId();
            cmd.CommandText = "SELECT [Count] " +
                "FROM tblStorage " +
                @"WHERE ManufacturerId = @ManuIdField AND ProductId = @ProdIdField";
            cmd.Parameters.AddWithValue(@"@ManuIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProdIdField", productId);
            var reader = cmd.ExecuteReader();
            reader.Read();
            int count = int.Parse(reader[0].ToString());
            cmd.Parameters.Clear();
            reader.Close();
            return count;
        }

        private int GetCustomerPurchaseSum()
        {
            string name = (dgNoteBook.SelectedItem as Sell).Customer;
            cmd.CommandText = "SELECT PurchaseSum " +
                "FROM tblCustomers " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"@NameField", name);

            int sum = 0;
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                sum = int.Parse(reader[0].ToString());
                cmd.Parameters.Clear();
            }
            return sum;
        }

        private void UpdateCustomerPurchaseSum(int SumToRemove)
        {
            int oldSum = GetCustomerPurchaseSum();
            int id = GetCustomerId();
            int newSum = oldSum - SumToRemove;

            cmd.CommandText = "UPDATE tblCustomers " +
                @"SET PurchaseSum = @NewSum " +
                @"WHERE Id = @IdField";
            cmd.Parameters.AddWithValue(@"NewSum", newSum);
            cmd.Parameters.AddWithValue(@"@IdField", id);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddSaleWindow addSaleWindow = new AddSaleWindow(cmd);
            addSaleWindow.ShowDialog();
            GetRangeNotebook(start);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (dgNoteBook.SelectedItem == null) return;
            int manufacturerId = GetManufacturerId();
            int productId = GetProductId();
            int totalSum = (dgNoteBook.SelectedItem as Sell).TotalSum;
            int countBringBack = (dgNoteBook.SelectedItem as Sell).Count;
            int id = (dgNoteBook.SelectedItem as Sell).Id;

            if (CheckStorageProduct())
            {
                int oldCount = GetSellCount();
                int newCount = oldCount + countBringBack;
                cmd.CommandText = "UPDATE tblStorage " +
                    @"SET Count = @CountField " +
                    @"WHERE ManufacturerId = @ManuIdField AND ProductId = @ProdIdField";
                cmd.Parameters.AddWithValue(@"CountField", newCount);
                cmd.Parameters.AddWithValue(@"@ManuIdField", manufacturerId);
                cmd.Parameters.AddWithValue(@"@ProdIdField", productId);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            else
            {
                cmd.CommandText = "INSERT INTO tblStorage " +
                    "( " +
                    "ManufacturerId, ProductId, Price, Count " +
                    ") " +
                    "VALUES (" +
                    @"@ManuIdField, @ProdIdField, @PriceField, @CountField " +
                    ")";
                cmd.Parameters.AddWithValue(@"@ManuIdField", manufacturerId);
                cmd.Parameters.AddWithValue(@"@ProdIdField", productId);
                cmd.Parameters.AddWithValue(@"@PriceField", totalSum / countBringBack);
                cmd.Parameters.AddWithValue(@"@CountField", countBringBack);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }

            UpdateCustomerPurchaseSum(totalSum);
            cmd.CommandText = "DELETE tblNotebook " +
               @"WHERE Id = @IdField";
            cmd.Parameters.AddWithValue(@"@IdField", id);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            GetRangeNotebook(start);
        }

        private void btnAddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            AddManufacturerWindow addManufacturerWindow = new AddManufacturerWindow(cmd);
            addManufacturerWindow.ShowDialog();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductWindow addProductWindow = new AddProductWindow(cmd);
            addProductWindow.ShowDialog();
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomerWindow addCustomerWindow = new AddCustomerWindow(cmd);
            addCustomerWindow.ShowDialog();
        }

        private void btnAddStorage_Click(object sender, RoutedEventArgs e)
        {
            AddToStorageWindow addToStorageWindow = new AddToStorageWindow(cmd);
            addToStorageWindow.ShowDialog();
        }

        private void btnViewManufacturers_Click(object sender, RoutedEventArgs e)
        {
            ViewManufacturersWindow manufacturers = new ViewManufacturersWindow(cmd);
            manufacturers.ShowDialog();
        }

        private void btnViewGroceries_Click(object sender, RoutedEventArgs e)
        {
            ViewGroceriesWindow groceries = new ViewGroceriesWindow(cmd);
            groceries.ShowDialog();
        }

        private void btnViewCustomers_Click(object sender, RoutedEventArgs e)
        {
            ViewCustomersWindow customers = new ViewCustomersWindow(cmd);
            customers.ShowDialog();
        }

        private void btnViewStorage_Click(object sender, RoutedEventArgs e)
        {
            ViewStorageWindow storage = new ViewStorageWindow(cmd);
            storage.ShowDialog();
        }

        private void PrevPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPage == 1) return;
            currentPage--;
            start -= MaxGroceriesInPage;
            GetRangeNotebook(start);
        }

        private void NextPage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentPage == pages) return;
            currentPage++;
            start += MaxGroceriesInPage;
            GetRangeNotebook(start);
        }
    }
}
