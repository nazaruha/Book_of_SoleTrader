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
        public MainWindow()
        {
            InitializeComponent();
            OpenDbWorker();
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
            GetNotebook();
        }

        private void GetNotebook()
        {
            string script = File.ReadAllText($"{dirScripts}\\viewtblNotebook.sql");
            cmd.CommandText = script;
            dgNoteBook.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Sell sell = new Sell() { Id = int.Parse(reader["Id"].ToString()), Date = ((DateTime)reader["Date"]).ToShortDateString(), Manufacturer = reader["Manufacturer"].ToString(), Product = reader["Product"].ToString(), Count = int.Parse(reader["Count"].ToString()), TotalSum = int.Parse(reader["TotalSum"].ToString()), Customer = (reader["Customer"].ToString() + " (" + reader["Phone"].ToString() + ")") };
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
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddSaleWindow addSaleWindow = new AddSaleWindow(cmd);
            addSaleWindow.ShowDialog();
            GetNotebook();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
