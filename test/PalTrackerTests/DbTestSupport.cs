using System.Collections.Generic;
using System.Linq;
using MySql.Data.MySqlClient;

namespace PalTrackerTests
{
    public static class DbTestSupport
    {
        public const string TestDbConnectionString = "server=127.0.0.1;uid=tracker;pwd=password;database=tracker_test";

        public const string TestDbVcapJson = @"
            {
                ""p-mysql"": [
                {
                    ""credentials"": {
                        ""uri"": ""mysql://tracker:password@localhost:3306/tracker_test?reconnect=true""
                    },
                    ""name"": ""pal-tracker-dotnet-mysql""
                }
                ]
            }
        ";

        public static IList<IDictionary<string, object>> ExecuteSql(string sql)
        {
            var result = new List<IDictionary<string, object>>();

            using (var connection = new MySqlConnection(TestDbConnectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                var rowData = Enumerable.Range(0, reader.FieldCount)
                                    .ToDictionary(reader.GetName, reader.GetValue);

                                result.Add(rowData);
                            }

                            reader.NextResult();
                        }
                    }
                }

                connection.Close();
            }

            return result;
        }
    }
}