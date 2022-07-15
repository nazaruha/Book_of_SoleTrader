using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Worker_DB
{
    public class Program
    {
        static string DbName = "РРО";
        static string dirSql = "SqlTables";
        static SqlConnection con;
        static SqlCommand cmd;

        static void Main(string[] args)
        {
            
        }

        static bool IsDBExists()
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT name FROM master.sys.databases";
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader["name"].ToString() == DbName)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
