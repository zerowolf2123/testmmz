using db.conn;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace db.Requests
{
    public class MainRequests
    {
        private string Connection { get; }

        public MainRequests(string connection)
        {
            Connection = connection;
        }

        private static void CloseConnection(MySqlConnection conn)
        {
            conn.Close();
            conn.Dispose();
        }

        public Dictionary<string, string> GetAllProducts()
        {
            using MySqlConnection? conn = DBUnits.GetSqlConnection(Connection);
            if (conn != null)
            {
                conn.Open();

                string sql = "select oboz, naim from izd";

                var cmd = conn.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        Dictionary<string, string> products = new();
                        while (reader.Read())
                        {
                            products.Add(reader.GetString(0), reader.GetString(1));
                        }
                        CloseConnection(conn);
                        return products;
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

        public int GetProductId(string oboz)
        {
            using MySqlConnection? conn = DBUnits.GetSqlConnection(Connection);
            if (conn != null)
            {
                conn.Open();

                string sql = "select id from izd " +
                    "where oboz = @oboz";

                var cmd = conn.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                cmd.Parameters.Add("@oboz", MySqlDbType.String).Value = oboz;

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

        public List<int> GetDetailsId(int productId)
        {
            using MySqlConnection? conn = DBUnits.GetSqlConnection(Connection);
            if (conn != null)
            {
                conn.Open();

                string sql = "select nom_id from enumeration_details " +
                    "where izd_id = @productId";

                var cmd = conn.CreateCommand();
                cmd.Connection = conn;
                cmd.CommandText = sql;

                cmd.Parameters.Add("@productId", MySqlDbType.Int32).Value = productId;

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        List<int> detailsId = new();
                        while (reader.Read())
                        {
                            detailsId.Add(reader.GetInt32(0));
                        }
                        CloseConnection(conn);
                        return detailsId;
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

        public Dictionary<string, string> GetDetails(List<int> detailsId)
        {
            using MySqlConnection? conn = DBUnits.GetSqlConnection(Connection);
            if (conn != null)
            {
                conn.Open();

                Dictionary<string, string> details = new();
                foreach (int detailId in detailsId)
                {
                    string sql = "select oboz, naim from nom " +
                        "where id = @detailId";

                    var cmd = conn.CreateCommand();
                    cmd.Connection = conn;
                    cmd.CommandText = sql;

                    cmd.Parameters.Add("@detailId", MySqlDbType.Int32).Value = detailId;

                    using (DbDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                details.Add(reader.GetString(0), reader.GetString(1));
                                continue;
                            }
                        }
                    }
                }
                if (details != null)
                {
                    CloseConnection(conn);
                    return details;
                }
                throw new ArgumentNullException("Нет данных с таким(и) id", nameof(detailsId));
            }
            throw new ArgumentNullException("Не удалось подключиться к бд", nameof(conn));
        }
    }
}
