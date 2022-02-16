using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WebexBot.Models;

namespace WebexBot.Infrastructure
{

    public class AdoDbAccessor
    {
        private string connectionString = "server=azdamicoapp.damicospa.int;Trusted_Connection=True;MultipleActiveResultSets=true;database=WebexBot;";

        public void SaveRequest(string clientId, string message)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlString;
                sqlString = "INSERT INTO RequestLog (clientid, message, datetimerequest) values (@clientId, @message, GETUTCDATE())";
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand(sqlString, connection);
                    cmd.CommandTimeout = 1000;
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    cmd.Parameters.AddWithValue("@message", message);
                    cmd.ExecuteNonQuery();
                    connection.Close();
                }
                catch 
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }
        }

        public bool ClientExists(string clientId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlString;
                sqlString = "Select * from users where clientID = @clientID and enabled = '1'";
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand(sqlString, connection);
                    cmd.CommandTimeout = 1000;
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    //
                    if (rdr.HasRows )
                    {
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        connection.Close();
                        return false;
                    }               
                }
                catch
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return false;
                }
            }
        }

        public bool ClientExists(string clientId, int actionID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlString;
                sqlString = "Select * from ActionsPermissions where clientID = @clientID and actionID = @actionID and enabled = '1'";
                try
                {
                    connection.Open();
                    var cmd = new SqlCommand(sqlString, connection);
                    cmd.CommandTimeout = 1000;
                    cmd.Parameters.AddWithValue("@clientId", clientId);
                    cmd.Parameters.AddWithValue("@actionID", actionID);
                    SqlDataReader rdr = cmd.ExecuteReader();
                    //
                    if (rdr.HasRows)
                    {
                        connection.Close();
                        return true;
                    }
                    else
                    {
                        connection.Close();
                        return false;
                    }
                }
                catch
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    return false;
                }
            }
        }

        public List<Actions> GetActionsByClientID(string clientID)
        {
            List<Actions> actionsList = new List<Actions>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sqlString = "Select * from Actions where ID in (select actionID from ActionsPermissions where ClientID = @clientIDparam and enabled = '1') order by Description";
                try
                {
                    connection.Open();
                    actionsList = connection.Query<Actions>(sqlString, new { clientIDparam = clientID }).ToList();
                    connection.Close();
                }
                catch
                {
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
                return actionsList;        
            }
        }
    }
}
