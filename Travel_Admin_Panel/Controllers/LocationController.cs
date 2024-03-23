using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;
using Travel_Admin_Panel.Models;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class LocationController : Controller
    {
        //
        // GET: /Location/

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Location Model)
        {
            String[] Result = new String[2];
            String ImagePath = null;
            if (ModelState.IsValid)
            {
                try
                {

                    if (Model.LocationImage != null && Model.LocationImage.ContentLength > 0)
                    {

                        if (Model.LocationImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (Model.LocationImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFile(Model.LocationImage, "location");
                            if (ImagePath == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageLocation");
                    CMD.Parameters.AddWithValue("@Action", "ADD_LOCATION");
                    CMD.Parameters.AddWithValue("@Name", Model.Name);
                    CMD.Parameters.AddWithValue("@ImageURL", ImagePath);
                    CMD.Parameters.AddWithValue("@Description", Model.Description);
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
            DataTable DT = GetAllLocations();
            CombinedLocationModel MDL = new CombinedLocationModel();
            MDL.DT = DT;
            return View(MDL);

        }

        private DataTable GetAllLocations()
        {
            SqlCommand CMD = new SqlCommand("SP_GetLocation");
            CMD.Parameters.AddWithValue("@Action", "ALL_LOCATION");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewLocationsAjax()
        {
            return PartialView("LocationTable", GetAllLocations());
        }

        [HttpPost]
        public ActionResult Delete(long ID, String ImageUrl)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageLocation");
                CMD.Parameters.AddWithValue("@Action", "DELETE_LOCATION");
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
            return PartialView("LocationTable", GetAllLocations());
            // return Json(Result);
        }

        [HttpPost]
        public ActionResult Edit(CombinedLocationModel CModel)
        {
            String[] Result = new String[2];
            String ImagePath = CModel.Loc.ImageLink;
            bool IsUploaded = false;
            if (ModelState.IsValid)
            {
                try
                {
                    if (CModel.Loc.LocationImage != null && CModel.Loc.LocationImage.ContentLength > 0)
                    {

                        if (CModel.Loc.LocationImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (CModel.Loc.LocationImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFile(CModel.Loc.LocationImage, "location");
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

                    SqlCommand CMD = new SqlCommand("SP_ManageLocation");
                    CMD.Parameters.AddWithValue("@Action", "UPDATE_LOCATION");
                    CMD.Parameters.AddWithValue("@ID", CModel.Loc.ID);
                    CMD.Parameters.AddWithValue("@Name", CModel.Loc.Name);
                    CMD.Parameters.AddWithValue("@Description", CModel.Loc.Description);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath);
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
                        new FileUploadController().DeleteFile(CModel.Loc.ImageLink);
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
