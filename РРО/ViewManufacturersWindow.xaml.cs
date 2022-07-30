using РРО.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
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
    /// Interaction logic for ViewManufacturersWindow.xaml
    /// </summary>
    public partial class ViewManufacturersWindow : Window
    {
        private SqlCommand cmd { get; set; }
        private string dirScripts { get; set; } = "Scripts";
        bool start = false;
        public ViewManufacturersWindow(SqlCommand cmd)
        {
            this.cmd = cmd;
            InitializeComponent();
            GetManufacturers();
            txtSearch.Foreground = Brushes.LightGray;
        }

        private void GetManufacturers()
        {
            string script = File.ReadAllText($"{dirScripts}\\viewtblManufacturers.sql");
            cmd.CommandText = script;

            dgManufacturers.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Manufacturer manufacturer = new Manufacturer() { Id = int.Parse(reader["Id"].ToString()), Name = reader["Name"].ToString() };
                    dgManufacturers.Items.Add(manufacturer);
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
                GetManufacturers();
                return;
            }

            cmd.CommandText = "SELECT * " +
                "FROM tblManufacturers " +
                $"WHERE Name LIKE '%{txtSearch.Text}%'";

            dgManufacturers.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Manufacturer manufacturer = new Manufacturer() { Id = int.Parse(reader["Id"].ToString()), Name = reader["Name"].ToString() };
                    dgManufacturers.Items.Add(manufacturer);
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
