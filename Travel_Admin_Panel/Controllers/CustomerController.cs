using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;
using Travel_Admin_Panel.Models;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class CustomerController : Controller

    {
        //
        // GET: /Customer/

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Customer Model)
        {
            String[] Result = new String[2];
            if (ModelState.IsValid)
            {
                try
                {
                    SqlCommand CMD = new SqlCommand("SP_ManageCustomer");
                    CMD.Parameters.AddWithValue("@Action", "ADD_CUSTOMER");
                    CMD.Parameters.AddWithValue("@Name", Model.Name);
                    CMD.Parameters.AddWithValue("@Email", Model.Email);
                    CMD.Parameters.AddWithValue("@Address", Model.Address);
                    CMD.Parameters.AddWithValue("@Phone", Model.Phone);
                    CMD.Parameters.AddWithValue("@CreatedBy", Session["UserSession"].ToString());

                    SqlParameter Tran = new SqlParameter("@Tran", SqlDbType.Bit);
                    Tran.Direction = ParameterDirection.Output;

                    SqlParameter Msg = new SqlParameter("@Msg", SqlDbType.VarChar, 200);
                    Msg.Direction = ParameterDirection.Output;

                    CMD.Parameters.Add(Tran);
                    CMD.Parameters.Add(Msg);

                    DBConnection DB = new DBConnection();
                    DB.ExecuteSPNonQuery(CMD);
                    Result[0] = Tran.Value.ToString();
                    Result[1] = Msg.Value.ToString();

                }
                catch (Exception ex)
                {
                    Result[0] = "False";
                    Result[1] = "Unknown error occured!";

                }
                return Json(Result);
            }
            Result[0] = "False";
            Result[1] = "Please Fill valid data in all required fields";
            return Json(Result);
        }
        public ActionResult Manage()
        {
            DataTable DT = GetAllCustomers();
            CombinedCustomerModel MDL = new CombinedCustomerModel();
            MDL.DT = DT;
            return View(MDL);

        }
        private DataTable GetAllCustomers()
        {
            SqlCommand CMD = new SqlCommand("SP_GetCustomers");
            CMD.Parameters.AddWithValue("@Action", "ALL_CUSTOMERS");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewCustomersAjax()
        {
            return PartialView("CustomerTable", GetAllCustomers());
        }

        [HttpPost]
        public ActionResult Delete(long ID)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageCustomer");
                CMD.Parameters.AddWithValue("@Action", "DELETE_CUSTOMER");
                CMD.Parameters.AddWithValue("@ID", ID);

                SqlParameter Tran = new SqlParameter("@Tran", SqlDbType.Bit);
                Tran.Direction = ParameterDirection.Output;

                SqlParameter Msg = new SqlParameter("@Msg", SqlDbType.VarChar, 200);
                Msg.Direction = ParameterDirection.Output;

                CMD.Parameters.Add(Tran);
                CMD.Parameters.Add(Msg);

                DBConnection DB = new DBConnection();
                DB.ExecuteSPNonQuery(CMD);
                Result[0] = Tran.Value.ToString();
                Result[1] = Msg.Value.ToString();


            }
            catch (Exception)
            {
                Result[0] = "False";
                Result[1] = "Unknown error occured!";
            }
            ViewBag.Tran = Result[0];
            ViewBag.Msg = Result[1];
            return PartialView("CustomerTable", GetAllCustomers());
        }


        [HttpPost]
        public ActionResult Edit(CombinedCustomerModel CModel)
        {
            String[] Result = new String[2];
            if (ModelState.IsValid)
            {
                try
                {
                    SqlCommand CMD = new SqlCommand("SP_ManageCustomer");
                    CMD.Parameters.AddWithValue("@Action", "UPDATE_CUSTOMER");
                    CMD.Parameters.AddWithValue("@ID", CModel.Cust.ID);
                    CMD.Parameters.AddWithValue("@Name", CModel.Cust.Name);
                    CMD.Parameters.AddWithValue("@Email", CModel.Cust.Email);
                    CMD.Parameters.AddWithValue("@Address", CModel.Cust.Address);
                    CMD.Parameters.AddWithValue("@Phone", CModel.Cust.Phone);
                    CMD.Parameters.AddWithValue("@CreatedBy", Session["UserSession"].ToString());

                    SqlParameter Tran = new SqlParameter("@Tran", SqlDbType.Bit);
                    Tran.Direction = ParameterDirection.Output;

                    SqlParameter Msg = new SqlParameter("@Msg", SqlDbType.VarChar, 200);
                    Msg.Direction = ParameterDirection.Output;

                    CMD.Parameters.Add(Tran);
                    CMD.Parameters.Add(Msg);

                    DBConnection DB = new DBConnection();
                    DB.ExecuteSPNonQuery(CMD);
                    Result[0] = Tran.Value.ToString();
                    Result[1] = Msg.Value.ToString();

                }
                catch (Exception ex)
                {
                    Result[0] = "False";
                    Result[1] = "Unknown error occured!";

                }
                return Json(Result);
            }
            Result[0] = "False";
            Result[1] = "Please Fill valid data in all required fields";
            return Json(Result);
        }

    }
}