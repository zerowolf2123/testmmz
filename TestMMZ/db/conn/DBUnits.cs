using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.conn
{
    public class DBUnits
    {
        public static MySqlConnection GetSqlConnection(string connString)
        {
            return DBMySQLUnits.GetDBConnection(connString);
        }
    }
}
