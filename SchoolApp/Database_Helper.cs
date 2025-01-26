using MySql.Data.MySqlClient;

namespace DatabaseHelpers
{
    public static class Database_Helper
    {
        private static string connectionString = "Server=localhost;Database=School;Uid=root;Pwd=123456789;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public static void ExecuteQuery(string query, Action<MySqlCommand>? parameterize = null, Action<MySqlDataReader>? process = null)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new MySqlCommand(query, connection))
                {
                    parameterize?.Invoke(command);
                    if (process != null)
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            process(reader);
                        }
                    }
                    else
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
