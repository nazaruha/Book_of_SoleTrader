using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
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
    /// Interaction logic for AddToStorageWindow.xaml
    /// </summary>
    public partial class AddToStorageWindow : Window
    {
        private SqlCommand cmd { get; set; }
        private bool flag = false;

        public AddToStorageWindow(SqlCommand cmd)
        {
            InitializeComponent();
            this.cmd = cmd;
            GetManufecturers();
            GetGroceries();
        }

        private void GetManufecturers()
        {
            cmd.CommandText = "SELECT Name " +
                "FROM tblManufacturers";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    cbManufacturers.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private int GetManufacturerId()
        {
            if (cbManufacturers.SelectedIndex == -1) return -1;
            cmd.CommandText = "SELECT Id " +
                "FROM tblManufacturers " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"NameField", cbManufacturers.SelectedItem.ToString());

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                int id = int.Parse(reader["Id"].ToString());
                cmd.Parameters.Clear();
                return id;
            }
           
        }

        private void GetGroceries()
        {
            cmd.CommandText = "SELECT Name " +
                "FROM tblGroceries";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    cbGroceries.Items.Add(reader["Name"].ToString());
                }
            }
        }

        private int GetProductId()
        {
            if (cbGroceries.SelectedIndex == -1) return -1;
            cmd.CommandText = "SELECT Id " +
                "FROM tblGroceries " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"@NameField", cbGroceries.SelectedItem.ToString());

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                int id = int.Parse(reader["Id"].ToString());
                cmd.Parameters.Clear();
                return id;
            }

        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void cbGroceries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbManufacturers.SelectedIndex == -1) return;
            GetStorageInfo();
            
        }

        private void cbManufacturers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbGroceries.SelectedIndex == -1) return;
            GetStorageInfo();
        }

        private void GetStorageInfo()
        {
            int manufacturerId = GetManufacturerId();
            int productId = GetProductId();
            cmd.CommandText = "SELECT Price, Count " +
                "FROM tblStorage " +
                @"WHERE ManufacturerId = @ManufacturerIdField AND ProductId = @ProductIdField";
            cmd.Parameters.AddWithValue(@"@ManufacturerIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProductIdField", productId);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                try
                {
                    if (reader["Price"] != null && reader["Count"] != null)
                    {
                        txtPrice.Text = reader["Price"].ToString();
                        txtCount.Text = reader["Count"].ToString();
                        flag = true;
                    }
                }
                catch
                {
                    txtPrice.Clear();
                    txtCount.Clear();
                    flag = false;
                }
                cmd.Parameters.Clear();
            }
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInputting()) return;
            int manufacturerId = GetManufacturerId();
            int productId = GetProductId();
            if (flag)
            {
                cmd.CommandText = "UPDATE tblStorage " +
                    @"SET Price = @PriceField, [Count] = @CountField " +
                    @"WHERE ManufacturerId = @ManufacturerIdField AND ProductId = @ProductIdField";
            }
            else
            {
                cmd.CommandText = "INSERT INTO tblStorage " +
                    "( " +
                    "ManufacturerId, ProductId, Price, [Count] " +
                    ") " +
                    "VALUES ( " +
                    @"@ManufacturerIdField, @ProductIdField, @PriceField, @CountField " +
                    ")";
            }
            cmd.Parameters.AddWithValue(@"PriceField", int.Parse(txtPrice.Text));
            cmd.Parameters.AddWithValue(@"@CountField", int.Parse(txtCount.Text));
            cmd.Parameters.AddWithValue(@"@ManufacturerIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProductIdField", productId);
            cmd.ExecuteNonQuery();
            MessageBox.Show("Data has been updated");
        }

        private bool CheckInputting()
        {
            string errors = "";
            if (cbManufacturers.SelectedIndex == -1)
                errors += "Choose Manufacturer\n";
            if (cbGroceries.SelectedIndex == -1)
                errors += "Choose Product\n";
            if (String.IsNullOrEmpty(txtPrice.Text))
                errors += "Input Price\n";
            if (String.IsNullOrEmpty(txtCount.Text))
                errors += "Input Count\n";

            if (errors != "")
            {
                MessageBox.Show(errors, "Error Inputting", MessageBoxButton.OK);
                return false;
            }

                
            return true;
        }
    }
}
