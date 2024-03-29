using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
namespace Travel_Admin_Panel.App_Code
{

    public class DBConnection
    {
        public string ConnectionString = "";
        public DBConnection()
        {
            ConnectionString = ConfigurationManager.ConnectionStrings["Connstring"].ConnectionString;
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