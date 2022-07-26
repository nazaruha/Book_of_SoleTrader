using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
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
    /// Interaction logic for AddSaleWindow.xaml
    /// </summary>
    public partial class AddSaleWindow : Window
    {
        private SqlCommand cmd { get; set; }
        private bool isDiscounted = false;

        public AddSaleWindow(SqlCommand cmd)
        {
            InitializeComponent();
            this.cmd = cmd;
            GetManufacturers();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void GetManufacturers()
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

        private void GetManufacturerGroceries(int ManufacturerId)
        {
            cmd.CommandText = "SELECT g.Name " +
                "FROM tblStorage AS s " +
                "INNER JOIN tblGroceries AS g " +
                "ON s.ProductId = g.Id " +
                @"WHERE s.ManufacturerId = @IdField";
            cmd.Parameters.AddWithValue(@"@IdField", ManufacturerId);

            cbGroceries.Items.Clear();
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    cbGroceries.Items.Add(reader["Name"].ToString());
                }
            }
            cmd.Parameters.Clear();
        }

        private int GetManufacturerProductPrice()
        {
            int productId = GetProductId();
            int manufacturerId = GetManufacturerId();

            cmd.CommandText = "SELECT Price " +
                "FROM tblStorage " +
                @"WHERE ManufacturerId = @ManIdField AND ProductId = @ProdIdField";
            cmd.Parameters.AddWithValue(@"@ManIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProdIdField", productId);

            int price = 0;
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                price = int.Parse(reader["Price"].ToString());
            }
            cmd.Parameters.Clear();
            return price;
        }

        private int GetManufacturerProductCount()
        {
            int productId = GetProductId();
            int manufacturerId = GetManufacturerId();

            cmd.CommandText = "SELECT Count " +
                "FROM tblStorage " +
                @"WHERE ManufacturerId = @ManIdField AND ProductId = @ProdIdField";
            cmd.Parameters.AddWithValue(@"@ManIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProdIdField", productId);

            int count = 0;
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                count = int.Parse(reader["Count"].ToString());
            }

            cmd.Parameters.Clear();
            return count;
        }

        private bool IsCustomerExists()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblCustomers " +
                @"WHERE Name = @NameField AND Phone = @PhoneField";
            cmd.Parameters.AddWithValue(@"@NameField", txtName.Text);
            cmd.Parameters.AddWithValue(@"@PhoneField", txtPhone.Text);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                try
                {
                    if (reader["Id"] != null)
                    {
                        cmd.Parameters.Clear();
                        return true;
                    }
                    cmd.Parameters.Clear();
                    return false;
                }
                catch
                {
                    cmd.Parameters.Clear();
                    return false;
                }
            }
           
        }

        private void AddCustomer()
        {
            cmd.CommandText = "INSERT INTO tblCustomers " +
                "( " +
                "[Name], Phone, PurchaseSum " +
                ") " +
                "VALUES ( " +
                "@NameField, @PhoneField, @SumField " +
                ")";
            cmd.Parameters.AddWithValue(@"@NameField", txtName.Text);
            cmd.Parameters.AddWithValue(@"@PhoneField", txtPhone.Text);
            cmd.Parameters.AddWithValue(@"@SumField", 0);
            try
            {
                cmd.ExecuteNonQuery();
                lbPhoneError.Visibility = Visibility.Hidden;
            }
            catch
            {
                lbPhoneError.Visibility = Visibility.Visible;
            }
            cmd.Parameters.Clear();
        }

        private int GetCustomerId()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblCustomers " +
                @"WHERE Name = @NameField AND Phone = @PhoneField";
            cmd.Parameters.AddWithValue(@"@NameField", txtName.Text);
            cmd.Parameters.AddWithValue(@"@PhoneField", txtPhone.Text);

            int id = 0;
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                id = Convert.ToInt32(reader["Id"]);
            }
            cmd.Parameters.Clear();
            return id;
        }

        private void UpdateCustomer()
        {
            cmd.CommandText = "SELECT PurchaseSum " +
                "FROM tblCustomers " +
                "WHERE Name = @NameField AND Phone = @PhoneField";
            cmd.Parameters.AddWithValue(@"@NameField", txtName.Text);
            cmd.Parameters.AddWithValue(@"@PhoneField", txtPhone.Text);

            int purchaseSum = int.Parse(txtTotalSum.Text);
            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                purchaseSum += int.Parse(reader["PurchaseSum"].ToString());
            }
            cmd.Parameters.Clear();

            int customerId = GetCustomerId();
            cmd.CommandText = "UPDATE tblCustomers" +
                @" SET PurchaseSum = @SumField " +
                @"WHERE Id = @IdField";
            cmd.Parameters.AddWithValue(@"@SumField", purchaseSum);
            cmd.Parameters.AddWithValue(@"@IdField", customerId);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        private void UpdateManufacturerProduct()
        {
            int count = GetManufacturerProductCount();
            count -= int.Parse(txtCount.Text);
            int manufacturerId = GetManufacturerId();
            int productId = GetProductId();

            if (count == 0)
            {
                cmd.CommandText = "DELETE tblStorage " +
                    @"WHERE ManufacturerId = @ManIdField AND ProductId = @ProdIdField";
            }
            else
            {
                cmd.CommandText = "UPDATE tblStorage " +
                @"SET Count = @CountField " +
                @"WHERE ManufacturerId = @ManIdField AND ProductId = @ProdIdField";
                cmd.Parameters.AddWithValue(@"@CountField", count);
            }
            cmd.Parameters.AddWithValue(@"@ManIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@ProdIdField", productId);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        private void cbManufacturers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbManufacturers.SelectedIndex == -1)
            {
                cbGroceries.Items.Clear();
                txtTotalSum.Clear();
                return;
            }
            int id = GetManufacturerId();
            GetManufacturerGroceries(id);
        }

        private void cbGroceries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbGroceries.SelectedIndex == -1)
                return;
            int productId = GetProductId();
            if (productId == -1)
            {
                txtTotalSum.Text = "0";
                return;
            }
            int price = GetManufacturerProductPrice();
            txtTotalSum.Text = price.ToString();

        }

        private void txtCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cbManufacturers.SelectedIndex == -1) return;
            if (String.IsNullOrWhiteSpace(txtCount.Text))
            {
                txtTotalSum.Text = "0";
                return;
            }
            int count = int.Parse(txtCount.Text);
            int storageCount = GetManufacturerProductCount();
            if (count > storageCount)
            {
                txtTotalSum.Text = "0";
                lbCountError.Visibility = Visibility.Visible;
                txtCount.BorderBrush = Brushes.Red;
            }
            else
            {
                lbCountError.Visibility = Visibility.Hidden;
                txtCount.BorderBrush = Brushes.Gray;
                int price = GetManufacturerProductPrice();
                int totalSum = count * price;
                txtTotalSum.Text = totalSum.ToString();
            }
        }

        private void CustomerTxts_TextChanged(object sender, TextChangedEventArgs e)
        {
            cmd.CommandText = "SELECT * " +
                "FROM tblCustomers " +
                @"WHERE Name = @NameField AND Phone = @PhoneField";
            cmd.Parameters.AddWithValue(@"@NameField", txtName.Text);
            cmd.Parameters.AddWithValue(@"@PhoneField", txtPhone.Text);

            using (var reader = cmd.ExecuteReader())
            {
                reader.Read();
                try
                {
                    if (reader["PurchaseSum"] != null)
                    {
                        int purchaseSum = int.Parse(reader["PurchaseSum"].ToString());
                        if (purchaseSum > 10000)
                        {
                            txtDiscount.Text = "5";
                            cmd.Parameters.Clear();
                            reader.Close();
                            if (!String.IsNullOrEmpty(txtTotalSum.Text) && lbCountError.Visibility == Visibility.Hidden)
                            {
                                int count = int.Parse(txtCount.Text);
                                int price = GetManufacturerProductPrice();
                                int totalSum = count * price;
                                int discount = (totalSum * int.Parse(txtDiscount.Text)) / 100;
                                totalSum -= discount;
                                txtTotalSum.Text = totalSum.ToString();
                            }
                        }
                        else
                        {
                            txtDiscount.Clear();
                            cmd.Parameters.Clear();
                        }
                    }
                }
                catch
                {
                    txtDiscount.Clear();
                    cmd.Parameters.Clear();
                    reader.Close();
                    if (cbGroceries.SelectedIndex != -1)
                    {
                        int count = int.Parse(txtCount.Text);
                        int price = GetManufacturerProductPrice();
                        int totalSum = count * price;
                        txtTotalSum.Text = totalSum.ToString();
                    }
                    
                }
            }
        }

        private void txtTotalSum_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtTotalSum.Text == "0") return;
            if (!String.IsNullOrEmpty(txtDiscount.Text) && lbCountError.Visibility == Visibility.Hidden)
            {
                int count = int.Parse(txtCount.Text);
                int price = GetManufacturerProductPrice();
                int totalSum = count * price;
                int discount = (totalSum * int.Parse(txtDiscount.Text)) / 100;
                totalSum -= discount;
                txtTotalSum.Text = totalSum.ToString();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (txtTotalSum.Text == "0" || String.IsNullOrEmpty(txtTotalSum.Text))
            {
                MessageBox.Show("Choose at least 1 product");
                return;
            }
            if (String.IsNullOrWhiteSpace(txtName.Text) || String.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Input customer data");
                return;
            }
            if (DatePick.SelectedDate == null)
            {
                MessageBox.Show("Choose date");
                return;
            }

            if (!IsCustomerExists())
            {
                AddCustomer();
                if (lbPhoneError.Visibility == Visibility.Visible) return;
            }
            UpdateCustomer();
            UpdateManufacturerProduct();
            FillNotebook();
            MessageBox.Show("Sale has been added");
        }

        private void FillNotebook()
        {
            DateTime date = (DateTime)DatePick.SelectedDate;
            int productId = GetProductId();
            int manufacturerId = GetManufacturerId();
            int customerId = GetCustomerId();

            if (!String.IsNullOrEmpty(txtDiscount.Text))
            {
                cmd.CommandText = "INSERT INTO tblNotebook " +
                "( " +
                "[Date], ProductId, ManufacturerId, [Count], TotalSum, CustomerId, Discount " +
                ") " +
                "VALUES ( " +
                @"@DateField, @ProdIdField, @ManIdField, @CountField, @TotalSumField, @CustIdField, @DiscountField " +
                ")";
                cmd.Parameters.AddWithValue(@"@DiscountField", int.Parse(txtDiscount.Text));
            }
            else
            {
                cmd.CommandText = "INSERT INTO tblNotebook " +
                "( " +
                "[Date], ProductId, ManufacturerId, [Count], TotalSum, CustomerId " +
                ") " +
                "VALUES ( " +
                @"@DateField, @ProdIdField, @ManIdField, @CountField, @TotalSumField, @CustIdField " +
                ")";
            }
            cmd.Parameters.AddWithValue(@"@DateField", date);
            cmd.Parameters.AddWithValue(@"@ProdIdField", productId);
            cmd.Parameters.AddWithValue(@"@ManIdField", manufacturerId);
            cmd.Parameters.AddWithValue(@"@CountField", int.Parse(txtCount.Text));
            cmd.Parameters.AddWithValue(@"@TotalSumField", int.Parse(txtTotalSum.Text));
            cmd.Parameters.AddWithValue(@"@CustIdField", customerId);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

    }
}
