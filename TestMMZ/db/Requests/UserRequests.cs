using db.conn;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.Requests
{
    public class UserRequests
    {
        private string Connection { get; }

        public UserRequests (string connection)
        {
            Connection = connection;
        }

        private static void CloseConnection(MySqlConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }

        public int SearchUserId(string login, string password)
        {
            using MySqlConnection? conn = DBUnits.GetSqlConnection(Connection);
            if (conn != null)
            {
                conn.Open();

                string sql = "select id from user " +
                    "where login = @login and password = @password";

                var cmd = conn.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                cmd.Parameters.Add("@login", MySqlDbType.String).Value = login;
                cmd.Parameters.Add("@password", MySqlDbType.String).Value = password;

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(0);
                            CloseConnection(conn);
                            return id;
                        }
                    }
                    else
                    {
                        CloseConnection(conn);
                        throw new ArgumentNullException("Поля с такими данными не существует", nameof(reader));
                    }
                }
            }
            throw new ArgumentNullException("Не удалось подключиться к бд", nameof(conn));
        }
    }
}
