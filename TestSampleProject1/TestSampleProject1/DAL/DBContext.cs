using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using TestSampleProject1.Entities;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

namespace TestSampleProject1.DAL
{
    public class DBContext : DbContext
    {
        private string connectionString { get; set; }
        public DBContext(DbContextOptions<DBContext> options) : base(options)
        {
            Database.SetCommandTimeout(150000);
            this.connectionString = this.Database.GetDbConnection().ConnectionString;
        }
        public DbSet<UserDetails> Test1 { get; set; }
        //public DbSet<UserDetails1> Test2 { get; set; }

        public object SNO { get; internal set; }

        public IEnumerable<T> ExecuteSP<T>(string spname, object data)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var timeout = this.Database.GetCommandTimeout();

                connection.Open();

                IEnumerable<T> returnobj = connection.Query<T>(spname, data, commandType: System.Data.CommandType.StoredProcedure);
                return returnobj;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserDetails>(entity =>
            {
                entity.ToTable("Test1", "DBO");
                entity.HasKey(c => new { c.SNO });
            });

            //modelBuilder.Entity<UserDetails1>(entity =>
            //{
            //    entity.ToTable("Test2", "DBO");
            //    entity.HasKey(c => new { c.SNO });
            //});
        }

        public int ExecuteNonQuery(string spname, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var timeout = this.Database.GetCommandTimeout();

                connection.Open();
                return ExecuteNonQuery(connection, CommandType.StoredProcedure, spname, commandParameters);

                //   IEnumerable<T> returnobj = connection.Query<T>(spname, data, commandType: System.Data.CommandType.StoredProcedure);
                //   return returnobj;
            }
        }


        public IEnumerable<T> ExecuteTextQuery<T>(string spname, object data)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var timeout = this.Database.GetCommandTimeout();

                connection.Open();

                IEnumerable<T> returnobj = connection.Query<T>(spname, data, commandType: System.Data.CommandType.Text);
                return returnobj;
            }
        }
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            try
            {
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

                // Finally, execute the command
                int retval = cmd.ExecuteNonQuery();

                // Detach the SqlParameters from the command object, so they can be used again
                cmd.Parameters.Clear();
                if (mustCloseConnection)
                    connection.Close();
                return retval;
            }
            catch (Exception ex)
            {
                // Default handler for unexpected exceptions
                object exMsg = ex.Message;
                throw;
            }
        }
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, out bool mustCloseConnection)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandText == null || commandText.Length == 0) throw new ArgumentNullException("commandText");

            // If the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                mustCloseConnection = true;
                connection.Open();
            }
            else
            {
                mustCloseConnection = false;
            }

            // Associate the connection with the command
            command.Connection = connection;

            // Set the command text (stored procedure name or SQL statement)
            command.CommandText = commandText;

            // If we were provided a transaction, assign it
            if (transaction != null)
            {
                if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                command.Transaction = transaction;
            }

            // Set the command type
            command.CommandType = commandType;
            command.CommandTimeout = 120;

            // Attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }
            return;
        }

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (commandParameters != null)
            {
                foreach (SqlParameter p in commandParameters)
                {
                    if (p != null)
                    {
                        // Check for derived output value with no value assigned
                        if ((p.Direction == ParameterDirection.InputOutput ||
                            p.Direction == ParameterDirection.Input) &&
                            (p.Value == null))
                        {
                            p.Value = DBNull.Value;
                        }
                        command.Parameters.Add(p);
                    }
                }
            }
        }

        public DataSet ExecuteDataset(string spname, params SqlParameter[] commandParameters)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                var timeout = this.Database.GetCommandTimeout();

                connection.Open();
                // Call the overload that takes a connection in place of the connection string
                return ExecuteDataset(connection, CommandType.StoredProcedure, spname, commandParameters);
            }
        }
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if (connection == null) throw new ArgumentNullException("connection");

            // Create a command and prepare it for execution
            SqlCommand cmd = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, out mustCloseConnection);

            // Create the DataAdapter & DataSet
            try
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataSet ds = new DataSet();

                    // Fill the DataSet using default values for DataTable names, etc
                    da.Fill(ds);

                    // Detach the SqlParameters from the command object, so they can be used again
                    cmd.Parameters.Clear();

                    if (mustCloseConnection)
                        connection.Close();

                    // Return the dataset
                    return ds;
                }
            }
            catch (Exception ex)
            {
                if (mustCloseConnection)
                    connection.Close();
                var msg = ex.Message;
                throw;
            }
        }


    }
}
