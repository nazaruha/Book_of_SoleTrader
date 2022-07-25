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
    /// Interaction logic for AddProductWindow.xaml
    /// </summary>
    public partial class AddProductWindow : Window
    {
        private SqlCommand cmd { get; set; }

        public AddProductWindow(SqlCommand cmd)
        {
            InitializeComponent();
            this.cmd = cmd;
        }

        private bool IsProductExists()
        {
            cmd.CommandText = "SELECT Id " +
                "FROM tblGroceries " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue(@"@NameField", txtProduct.Text);
            var reader = cmd.ExecuteReader();
            try
            {
                reader.Read();
                if (reader["Id"] != null)
                {
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

        private void AddManufacturer()
        {
            cmd.CommandText = "INSERT INTO tblGroceries " +
                "( " +
                "[Name] " +
                ") " +
                "VALUES ( " +
                "@NameField " +
                ")";
            cmd.Parameters.AddWithValue(@"@NameField", txtProduct.Text);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            if (!IsProductExists())
            {
                AddManufacturer();
                MessageBox.Show("Product has been added", "Successfully adding", MessageBoxButton.OK);
                txtProduct.Clear();
            }
        }
    }
}
