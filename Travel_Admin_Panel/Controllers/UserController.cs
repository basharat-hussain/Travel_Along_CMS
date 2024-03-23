using System;
using System.Web.Mvc;
using Travel_Admin_Panel.Models;
using Travel_Admin_Panel.App_Code;
using System.Data;
using System.Data.SqlClient;
using System.Net.NetworkInformation;

namespace Travel_Admin_Panel.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(User Model)
        {
            String[] Data = new String[2];

            if (ModelState.IsValid)
            {
                try
                {
                    DBConnection DB = new DBConnection();
                    SqlCommand CMD = new SqlCommand("sp_CheckLogin");
                    CMD.Parameters.AddWithValue("@UserName", Model.UserName);
                    CMD.Parameters.AddWithValue("@Password", Model.Password);

                    SqlParameter Tran = new SqlParameter("@Tran", SqlDbType.Bit);
                    Tran.Direction = ParameterDirection.Output;

                    SqlParameter Msg = new SqlParameter("@Msg", SqlDbType.VarChar, 200);
                    Msg.Direction = ParameterDirection.Output;

                    CMD.Parameters.Add(Tran);
                    CMD.Parameters.Add(Msg);
                    DB.ExecuteSPNonQuery(CMD);

                    if (Tran.Value.ToString() == "True")
                    {
                        Data[0] = "True";
                        Data[1] = "Login Successful. Please wait...";
                        Session["UserSession"] = Model.UserName;
                    }
                    else
                    {
                        Data[0] = "False";
                        Data[1] = "Invalid Username or Password";
                    }
                }
                catch (Exception)
                {
                    Data[0] = "False";
                    Data[1] = "Unknown error occured";
                }
            }
            else
            {
                Data[0] = "False";
                Data[1] = "Please provide username and password";
            }

            return Json(Data);
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePassword Model)
        {
            String[] Data = new String[2];

            if (ModelState.IsValid)
            {
                if (Model.NewPassword != Model.RepeatPassword)
                {
                    Data[0] = "False";
                    Data[1] = "New and repeat password should match";
                }
                else if (Model.CurrentPassword == Model.NewPassword)
                {
                    Data[0] = "False";
                    Data[1] = "OOps! New and current password same";
                }
                else
                {
                    try
                    {
                        DBConnection DB = new DBConnection();
                        SqlCommand CMD = new SqlCommand("sp_ChangePassword");
                        CMD.Parameters.AddWithValue("@UserName", Session["UserSession"]);
                        CMD.Parameters.AddWithValue("@CurrentPassword", Model.CurrentPassword);
                        CMD.Parameters.AddWithValue("@NewPassword", Model.NewPassword);

                        SqlParameter Tran = new SqlParameter("@Tran", SqlDbType.Bit);
                        Tran.Direction = ParameterDirection.Output;

                        SqlParameter Msg = new SqlParameter("@Msg", SqlDbType.VarChar, 200);
                        Msg.Direction = ParameterDirection.Output;

                        CMD.Parameters.Add(Tran);
                        CMD.Parameters.Add(Msg);
                        DB.ExecuteSPNonQuery(CMD);


                        Data[0] = Tran.Value.ToString();
                        Data[1] = Msg.Value.ToString();

                    }
                    catch (Exception)
                    {
                        Data[0] = "False";
                        Data[1] = "Oops! Unknown error occured";
                    }
                }
            }
            else
            {
                Data[0] = "False";
                Data[1] = "Please enter all passwords";
            }

            return Json(Data);
        }

        public ActionResult IsConnectedToInternet()
        {
            string host = "google.com";
            bool result = false;
            byte[] buffer = new byte[32];
            int timeout = 5000;
            PingOptions pingOptions = new PingOptions();
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, timeout, buffer, pingOptions);
                result = reply.Status == IPStatus.Success ? true : false;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return Json(result);
        }
    }
}
