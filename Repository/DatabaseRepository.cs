using Microsoft.Extensions.Configuration;
using RedditTracker.Structures;
using System.Data;
using System.Data.SqlClient;

namespace RedditTracker.Repository
{   
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly string connectionString;
        private const string tableName = "SubredditTracker";
        
        public DatabaseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetValue<string>("dbConnection");
        }

        public async Task<bool> CheckDatabaseConnectionAndTable()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                return false;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                DataTable schema = connection.GetSchema("Tables");
                return schema.Rows
                             .OfType<DataRow>()
                             .Any(row => row["TABLE_NAME"].ToString().Equals(tableName, StringComparison.OrdinalIgnoreCase));
            }
        }

        public async Task Insert(List<SubredditMessage> subredditMessages)
        {
            using (DataTable table = new DataTable())
            {
                table.Columns.Add("Subreddit", typeof(string));
                table.Columns.Add("Message", typeof(string));

                //rows
                foreach (var subredditMessage in subredditMessages)  //skip header
                {
                    table.Rows.Add(subredditMessage.Subreddit, subredditMessage.Message);
                }
                
                using (SqlConnection db = new SqlConnection(connectionString))
                {
                    db.Open();
                    SqlCommand insertCommand = new SqlCommand("dbo.usp_SubredditTracker_Insert", db);
                    insertCommand.CommandType = CommandType.StoredProcedure;
                    SqlParameter tvpParam = insertCommand.Parameters.AddWithValue("@subredditMessages", table);
                    tvpParam.SqlDbType = SqlDbType.Structured;
                    // Execute the command.  
                    insertCommand.ExecuteNonQuery();
                }
            }
            Console.WriteLine($"{subredditMessages.Count} rows inserted.");
        }
    }
}
