using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.conn
{
    public class DBMySQLUnits
    {
        public static MySqlConnection GetDBConnection(string connString)
        {
            return new MySqlConnection(connString);
        }
    }
}