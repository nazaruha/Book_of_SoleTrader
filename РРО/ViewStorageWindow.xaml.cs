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
    /// Interaction logic for ViewStorageWindow.xaml
    /// </summary>
    public partial class ViewStorageWindow : Window
    {
        private SqlCommand cmd { get; set; }
        private string dirScripts { get; set; } = "Scripts";
        private bool start { get; set; } = false;

        public ViewStorageWindow(SqlCommand cmd)
        {
            this.cmd = cmd;
            InitializeComponent();
            GetStorage();
            txtSearch.Foreground = Brushes.LightGray;
        }

        private void GetStorage()
        {
            string script = File.ReadAllText($"{dirScripts}\\viewtblStorage.sql");
            cmd.CommandText = script;

            ReadStorage();
        }

        private void ReadStorage()
        {
            dgStorage.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    StorageObject storageOBJ = new StorageObject() { Manufacturer = reader["Manufacturer"].ToString(), Product = reader["Product"].ToString(), Price = int.Parse(reader["Price"].ToString()), Count = int.Parse(reader["Count"].ToString()) };
                    dgStorage.Items.Add(storageOBJ);
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
                GetStorage();
                return;
            }

            string script = File.ReadAllText($"{dirScripts}\\viewtblStorage.sql");
            if (rb_Manufacturer.IsChecked == true)
                cmd.CommandText = script + $" WHERE m.[Name] LIKE '%{txtSearch.Text}%'";
            else if (rb_Product.IsChecked == true)
                cmd.CommandText = script + $" WHERE g.[Name] LIKE '%{txtSearch.Text}%'";
            else
            {
                GetStorage();
                return;
            }
            ReadStorage();
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

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!start)
            {
                start = true;
                return;
            }
            else if (String.IsNullOrWhiteSpace(txtSearch.Text) || (txtSearch.Text == "Search" && txtSearch.Foreground == Brushes.LightGray))
            {
                GetStorage();
                return;
            }

            string script = File.ReadAllText($"{dirScripts}\\viewtblStorage.sql");
            if (rb_Manufacturer.IsChecked == true)
                cmd.CommandText = script + $" WHERE m.[Name] LIKE '%{txtSearch.Text}%'";
            else if (rb_Product.IsChecked == true)
                cmd.CommandText = script + $" WHERE g.[Name] LIKE '%{txtSearch.Text}%'";
            ReadStorage();
        }
    }
}
