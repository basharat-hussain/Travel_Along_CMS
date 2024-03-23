using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using Travel_Admin_Panel.App_Code;
using Travel_Admin_Panel.Models;
using System.Collections.Generic;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class DestinationController : Controller
    {
        //
        // GET: /Destination/

        public ActionResult Add()
        {
            CombinedDestinationModel Mdl = new CombinedDestinationModel();
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
        public ActionResult Add(CombinedDestinationModel Model)
        {
            String[] Result = new String[2];
            String[] ImagePath = new String[2];
            if (ModelState.IsValid)
            {
                try
                {

                    if (Model.Dest.DestinationImage != null && Model.Dest.DestinationImage.ContentLength > 0)
                    {

                        if (Model.Dest.DestinationImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (Model.Dest.DestinationImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFileWithThumb(Model.Dest.DestinationImage, "destination");

                            if (ImagePath == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageDestination");
                    CMD.Parameters.AddWithValue("@Action", "ADD_DESTINATION");
                    CMD.Parameters.AddWithValue("@LocationID", Model.Dest.LocationID);
                    CMD.Parameters.AddWithValue("@Name", Model.Dest.Name);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath[0]);
                    CMD.Parameters.AddWithValue("@ThumbnailUrl", ImagePath[1]);
                    CMD.Parameters.AddWithValue("@Description", Model.Dest.Description);
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
                        new FileUploadController().DeleteMultipleFiles(ImagePath);
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
            DataTable DT = GetAllDestinations();
            CombinedDestinationModel MDL = new CombinedDestinationModel();
            MDL.LocationList = GetLocations();
            MDL.DT = DT;
            return View(MDL);

        }
        private DataTable GetAllDestinations()
        {
            SqlCommand CMD = new SqlCommand("SP_GetDestinations");
            CMD.Parameters.AddWithValue("@Action", "ALL_DESTINATION");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewDestinationsAjax()
        {
            return PartialView("DestinationTable", GetAllDestinations());
        }

        [HttpPost]
        public ActionResult Delete(long ID, String ImageUrl, String ThumbUrl)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageDestination");
                CMD.Parameters.AddWithValue("@Action", "DELETE_DESTINATION");
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
                    String[] Urls = { ImageUrl, ThumbUrl };
                    new FileUploadController().DeleteMultipleFiles(Urls);
                }
            }
            catch (Exception)
            {
                Result[0] = "False";
                Result[1] = "Unknown error occured!";
            }
            ViewBag.Tran = Result[0];
            ViewBag.Msg = Result[1];
            return PartialView("DestinationTable", GetAllDestinations());
            // return Json(Result);
        }

        

        [HttpPost]
        public ActionResult Edit(CombinedDestinationModel CModel)
        {
            String[] Result = new String[2];
            String[] ImagePath = { CModel.Dest.ImageLink, CModel.Dest.ThumbLink };
            bool IsUploaded = false;
            if (ModelState.IsValid)
            {
                try
                {
                    if (CModel.Dest.DestinationImage != null && CModel.Dest.DestinationImage.ContentLength > 0)
                    {

                        if (CModel.Dest.DestinationImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (CModel.Dest.DestinationImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            new FileUploadController().DeleteFile(CModel.Dest.ThumbLink);
                            ImagePath = new FileUploadController().UploadFileWithThumb(CModel.Dest.DestinationImage, "destination");
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

                    SqlCommand CMD = new SqlCommand("SP_ManageDestination");
                    CMD.Parameters.AddWithValue("@Action", "UPDATE_DESTINATION");
                    CMD.Parameters.AddWithValue("@ID", CModel.Dest.ID);
                    CMD.Parameters.AddWithValue("@LocationId", CModel.Dest.LocationID);
                    CMD.Parameters.AddWithValue("@Name", CModel.Dest.Name);
                    CMD.Parameters.AddWithValue("@Description", CModel.Dest.Description);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath[0]);
                    CMD.Parameters.AddWithValue("@ThumbnailUrl", ImagePath[1]);
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
                        new FileUploadController().DeleteFile(CModel.Dest.ImageLink);
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
