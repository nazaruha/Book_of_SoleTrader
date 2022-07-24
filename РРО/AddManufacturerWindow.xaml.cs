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
    /// Interaction logic for AddManufacturerWindow.xaml
    /// </summary>
    public partial class AddManufacturerWindow : Window
    {
        private SqlCommand cmd { get; set; }
        public AddManufacturerWindow(SqlCommand cmd)
        {
            InitializeComponent();
            this.cmd = cmd;
        }

        private bool IsManufacturerExists()
        {
            cmd.CommandText = "Select Id " +
                "FROM tblManufacturers " +
                @"WHERE Name = @NameField";
            cmd.Parameters.AddWithValue("@NameField", txtManufacturer.Text);
            try
            {
                var reader = cmd.ExecuteReader();
                lbError.Visibility = Visibility.Visible;
                cmd.Parameters.Clear();
                return true;
            }
            catch
            {
                lbError.Visibility = Visibility.Hidden;
                cmd.Parameters.Clear();
                return false;
            }
            
        }

        private void AddManufacturer()
        {
            cmd.CommandText = "INSERT INTO tblManufacturers " +
                "( " +
                "[Name] " +
                ") " +
                "VALUES ( " +
                "@NameField " +
                ")";
            cmd.Parameters.AddWithValue("@NameField", txtManufacturer.Text);
            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();
        }

        private void btnAddManufacturer_Click(object sender, RoutedEventArgs e)
        {
            if (!IsManufacturerExists())
            {
                AddManufacturer();
            }
        }
    }
}
