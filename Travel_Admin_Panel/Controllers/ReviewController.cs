using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class ReviewController : Controller
    {
        //
        // GET: /Review/

        public ActionResult Manage()
        {
            return View(GetAllReviews());
        }

        private DataTable GetAllReviews()
        {
            SqlCommand CMD = new SqlCommand("SP_GetReviews");
            CMD.Parameters.AddWithValue("@Action", "ALL_REVIEWS");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewReviewsAjax()
        {
            return PartialView("ReviewTable", GetAllReviews());
        }

        [HttpPost]
        public ActionResult Approval(long ID, Boolean Status)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageReviews");
                CMD.Parameters.AddWithValue("@Action", "UPDATE_REVIEW");
                CMD.Parameters.AddWithValue("@ID", ID);
                CMD.Parameters.AddWithValue("@Status", Status ? false : true);

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
            return PartialView("ReviewTable", GetAllReviews());

        }
    }
}
