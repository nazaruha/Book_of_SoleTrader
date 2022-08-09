using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for AddCustomerWindow.xaml
    /// </summary>
    public partial class AddCustomerWindow : Window
    {
        private SqlCommand cmd { get; set; }

        public AddCustomerWindow(SqlCommand cmd)
        {
            InitializeComponent();
            this.cmd = cmd;
        }

        private bool IsCustomerExists()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblCustomers " +
                @"WHERE Name = @NameField AND Phone = @PhoneField";
            cmd.Parameters.AddWithValue(@"@NameField", txtName.Text);
            cmd.Parameters.AddWithValue(@"@PhoneField", txtPhone.Text);
            var reader = cmd.ExecuteReader();
            try
            {
                reader.Read();
                if (reader["Id"] != null)
                {
                    lbError.Content = "Customer's already in database";
                    lbError.Visibility = Visibility.Visible;
                    cmd.Parameters.Clear();
                    reader.Close();
                    return true;
                }
                return false;
            }
            catch
            {
                lbError.Visibility = Visibility.Hidden;
                cmd.Parameters.Clear();
                reader.Close();
                return false;
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
                lbError.Visibility = Visibility.Hidden;
                MessageBox.Show("Customer has been added", "Successfully adding", MessageBoxButton.OK);
                txtName.Clear();
                txtPhone.Clear();
            }
            catch
            {
                lbError.Visibility = Visibility.Visible;
                lbError.Content = "The phone number is occupied";
            }
            
            cmd.Parameters.Clear();
        }

        private bool CheckInputting()
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                lbError.Content = "Input customer's name\n";
                if (String.IsNullOrWhiteSpace(txtPhone.Text))
                {
                    lbError.Content += "Input customer's phone";
                }
                lbError.Visibility = Visibility.Visible;
                return false;
            }
            else if (String.IsNullOrWhiteSpace(txtPhone.Text))
            {
                lbError.Content = "Input customer's phone";
                lbError.Visibility = Visibility.Visible;
                return false;
                
            }
            lbError.Visibility = Visibility.Hidden;
            return true;
        }

        private void btnAddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (!IsCustomerExists())
            {
                if (CheckInputting())
                    AddCustomer();
            }
        }
    }
}
