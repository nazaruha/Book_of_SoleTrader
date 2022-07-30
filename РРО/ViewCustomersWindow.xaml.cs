using РРО.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace РРО
{
    /// <summary>
    /// Interaction logic for ViewCustomersWindow.xaml
    /// </summary>
    public partial class ViewCustomersWindow : Window
    {
        private SqlCommand cmd { get; set; }
        private string dirScripts { get; set; } = "Scripts";
        bool start = false;

        public ViewCustomersWindow(SqlCommand cmd)
        {
            this.cmd = cmd;
            InitializeComponent();
            GetCustomers();
            txtSearch.Foreground = Brushes.LightGray;
        }

        private void GetCustomers()
        {
            string script = File.ReadAllText($"{dirScripts}\\viewtblCustomers.sql");
            cmd.CommandText = script;

            dgCustomers.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Customer customer = new Customer() { Id = int.Parse(reader["Id"].ToString()), Name = reader["Name"].ToString(), Phone = reader["Phone"].ToString(), PurchaseSum = int.Parse(reader["PurchaseSum"].ToString()) };
                    dgCustomers.Items.Add(customer);
                }
            }
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!start)
            {
                start = true;
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtSearch.Text) || (txtSearch.Text == "Search" && txtSearch.Foreground == Brushes.LightGray))
            {
                GetCustomers();
                return;
            }

            cmd.CommandText = "SELECT * " +
                "FROM tblCustomers " +
                $"WHERE Name LIKE '%{txtSearch.Text}%'";

            dgCustomers.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Customer customer = new Customer() { Id = int.Parse(reader["Id"].ToString()), Name = reader["Name"].ToString(), Phone = reader["Phone"].ToString(), PurchaseSum = int.Parse(reader["PurchaseSum"].ToString()) };
                    dgCustomers.Items.Add(customer);
                }
            }
        }

        private void txtSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            if (txtSearch.Text == "Search" && txtSearch.Foreground == Brushes.LightGray)
            {
                txtSearch.Clear();
                txtSearch.Foreground = Brushes.Black;
            }
        }

        private void txtSearch_LostFocus(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Foreground = Brushes.LightGray;
                txtSearch.Text = "Search";
            }
        }
    }
}
