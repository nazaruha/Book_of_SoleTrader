using РРО.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace РРО
{
    /// <summary>
    /// Interaction logic for ViewGroceriesWindow.xaml
    /// </summary>
    public partial class ViewGroceriesWindow : Window
    {
        private SqlCommand cmd { get; set; }
        private string dirScripts { get; set; } = "Scripts";
        bool start = false;

        public ViewGroceriesWindow(SqlCommand cmd)
        {
            this.cmd = cmd;
            InitializeComponent();
            GetGroceries();
            txtSearch.Foreground = Brushes.LightGray;
        }

        private void GetGroceries()
        {
            string script = File.ReadAllText($"{dirScripts}\\viewtblGroceries.sql");
            cmd.CommandText = script;

            dgGroceries.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Product product = new Product() { Id = int.Parse(reader["Id"].ToString()), Name = reader["Name"].ToString() };
                    dgGroceries.Items.Add(product);
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
                GetGroceries();
                return;
            }

            cmd.CommandText = "SELECT * " +
                "FROM tblGroceries " +
                $"WHERE Name LIKE '%{txtSearch.Text}%'";

            dgGroceries.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Manufacturer manufacturer = new Manufacturer() { Id = int.Parse(reader["Id"].ToString()), Name = reader["Name"].ToString() };
                    dgGroceries.Items.Add(manufacturer);
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
