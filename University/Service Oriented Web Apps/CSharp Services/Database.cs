using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace TwitchServices
{
    public class Database
    {
        private SqlConnection sqlConn;

        public Database()
        {
            // Open a connection the the SQL Server
            sqlConn = NewDbConnection();
            sqlConn.Open();
        }

        public SqlDataReader ExecuteQuery(string sqlQuery)
        {
            // Execute SQL query
            SqlCommand sqlCmd = new SqlCommand(sqlQuery, sqlConn);
            SqlDataReader data = sqlCmd.ExecuteReader();
            return data;
        }

        public void CloseConn()
        {
            sqlConn.Close();
        }

        private SqlConnection NewDbConnection()
        {
            // Open a connection the the SQL Server
            SqlConnection sqlConn = new SqlConnection();
            sqlConn.ConnectionString =
                      @"Data Source=SQL-SERVER;Initial Catalog=mp115;Integrated Security=True";
            return sqlConn;
        }
    }
}