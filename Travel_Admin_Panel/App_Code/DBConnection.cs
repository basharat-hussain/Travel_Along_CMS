using System;
using System.Data;
using System.Data.SqlClient;
namespace Travel_Admin_Panel.App_Code
{

    public class DBConnection
    {
        public string ConnectionString = "";
        public DBConnection()
        {
            //ConnectionString = @"server=192.168.1.3; Database=TM_DB_Travel; User Id=sa; Password=Hussain@313#";
            ConnectionString = @"server=wow.grabweb.in,2419; Database=TravelAlong_DB; User Id=travelalong_dbuser; Password=TravelAlong_DB#Usr1";
        }
        #region Execute SP Query DataTable
        //Executes a stored Procedure Query
        //Returns a Data Table

        public DataTable ExecuteSPQueryDT(SqlCommand cmd)
        {
            DataTable dt = new DataTable();
            SqlConnection con = new SqlConnection(ConnectionString);
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);

            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }

            return dt;

        }
        #endregion

        #region Execute SP Query DataSet
        //Executes a stored Procedure Query
        //Returns a Data Set
        public DataSet ExecuteSPQueryDS(SqlCommand cmd)
        {
            DataSet ds = new DataSet();
            SqlConnection con = new SqlConnection(ConnectionString);
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;


            try
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return ds;
        }

        #endregion

        #region Execute SP Non Query
        //Executes a Stored Procedure Non Query
        //Returns String

        public string ExecuteSPNonQuery(SqlCommand cmd)
        {
            string retString = "OK";

            SqlConnection con = new SqlConnection(ConnectionString);
            cmd.Connection = con;
            cmd.CommandType = CommandType.StoredProcedure;

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                retString = "Error";
            }
            finally
            {
                con.Close();
            }

            return retString;
        }

        #endregion
    }

}