using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;
using Travel_Admin_Panel.Models;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class ActivityController : Controller
    {
        //
        // GET: /Activity/

        public ActionResult Add()
        {
            CombinedActivityModel Mdl = new CombinedActivityModel();
            Mdl.LocationList = GetLocations();
            return View(Mdl);
        }

        public List<SelectListItem> GetLocations()
        {
            List<SelectListItem> LstLocation = new List<SelectListItem>();
            SqlCommand CMD = new SqlCommand("SP_GetLocation");
            CMD.Parameters.AddWithValue("@Action", "ALL_LOCATION");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            foreach (DataRow row in DT.Rows)
            {
                LstLocation.Add(new SelectListItem() { Text = row["LocationName"].ToString(), Value = row["LocationID"].ToString() });
            }
            return LstLocation;
        }


        [HttpPost]
        public ActionResult Add(CombinedActivityModel Model)
        {
            String[] Result = new String[2];
            String ImagePath = null;
            if (ModelState.IsValid)
            {
                try
                {

                    if (Model.Actvty.ActivityImage != null && Model.Actvty.ActivityImage.ContentLength > 0)
                    {

                        if (Model.Actvty.ActivityImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (Model.Actvty.ActivityImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFile(Model.Actvty.ActivityImage, "activity");
                            if (ImagePath == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageActivity");
                    CMD.Parameters.AddWithValue("@Action", "ADD_ACTIVITY");
                    CMD.Parameters.AddWithValue("@LocationID", Model.Actvty.LocationID);
                    CMD.Parameters.AddWithValue("@Title", Model.Actvty.Title);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath);
                    CMD.Parameters.AddWithValue("@Description", Model.Actvty.Description);
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
                    if (Result[0] == "False")
                    {
                        new FileUploadController().DeleteFile(ImagePath);
                    }

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
            DataTable DT = GetAllActivities();
            CombinedActivityModel MDL = new CombinedActivityModel();
            MDL.LocationList = GetLocations();
            MDL.DT = DT;
            return View(MDL);

        }

        private DataTable GetAllActivities()
        {
            SqlCommand CMD = new SqlCommand("SP_GetActivity");
            CMD.Parameters.AddWithValue("@Action", "ALL_ACTIVITIES");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewActivitiesAjax()
        {
            return PartialView("ActivityTable", GetAllActivities());
        }

        [HttpPost]
        public ActionResult Delete(long ID, String ImageUrl)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageActivity");
                CMD.Parameters.AddWithValue("@Action", "DELETE_ACTIVITY");
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
                if (Result[0] == "True")
                {
                    new FileUploadController().DeleteFile(ImageUrl);
                }
            }
            catch (Exception)
            {
                Result[0] = "False";
                Result[1] = "Unknown error occured!";
            }
            ViewBag.Tran = Result[0];
            ViewBag.Msg = Result[1];
            return PartialView("ActivityTable", GetAllActivities());
            // return Json(Result);
        }

        [HttpPost]
        public ActionResult Edit(CombinedActivityModel CModel)
        {
            String[] Result = new String[2];
            String ImagePath = CModel.Actvty.ImageLink;
            bool IsUploaded = false;
            if (ModelState.IsValid)
            {
                try
                {
                    if (CModel.Actvty.ActivityImage != null && CModel.Actvty.ActivityImage.ContentLength > 0)
                    {

                        if (CModel.Actvty.ActivityImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (CModel.Actvty.ActivityImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFile(CModel.Actvty.ActivityImage, "activity");
                            if (ImagePath == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }
                            else
                            {
                                IsUploaded = true;
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageActivity");
                    CMD.Parameters.AddWithValue("@Action", "UPDATE_ACTIVITY");
                    CMD.Parameters.AddWithValue("@LocationID", CModel.Actvty.LocationID);
                    CMD.Parameters.AddWithValue("@ID", CModel.Actvty.ID);
                    CMD.Parameters.AddWithValue("@Title", CModel.Actvty.Title);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath);
                    CMD.Parameters.AddWithValue("@Description", CModel.Actvty.Description);
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

                    if (Tran.Value.ToString() == "True" && IsUploaded)
                    {
                        new FileUploadController().DeleteFile(CModel.Actvty.ImageLink);
                    }

                }
                catch (Exception ex)
                {
                    Result[0] = "False";
                    Result[1] = "Unknown error occured!";

                }
                return Json(Result);
            }
            else
            {
                Result[0] = "False";
                Result[1] = "Please Fill valid data in all required fields";
            }

            return Json(Result);
        }

    }
}
