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
    /// Interaction logic for AddToStorageWindow.xaml
    /// </summary>
    public partial class AddToStorageWindow : Window
    {
        private SqlCommand cmd { get; set; }

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

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
