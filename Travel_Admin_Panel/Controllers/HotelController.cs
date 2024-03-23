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
    public class HotelController : Controller
    {
        //
        // GET: /Hotel/

        public ActionResult Add()
        {
            CombinedHotelModel Mdl = new CombinedHotelModel();
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
        public ActionResult Add(CombinedHotelModel Model)
        {
            String[] Result = new String[2];
            String[] ImagePath = new String[2];
            if (ModelState.IsValid)
            {
                try
                {

                    if (Model.Htl.HotelImage != null && Model.Htl.HotelImage.ContentLength > 0)
                    {

                        if (Model.Htl.HotelImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (Model.Htl.HotelImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFileWithThumb(Model.Htl.HotelImage, "hotel");

                            if (ImagePath[0] == null || ImagePath[1] == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageHotel");
                    CMD.Parameters.AddWithValue("@Action", "ADD_HOTEL");
                    CMD.Parameters.AddWithValue("@LocationID", Model.Htl.LocationID);
                    CMD.Parameters.AddWithValue("@Name", Model.Htl.Name);
                    CMD.Parameters.AddWithValue("@Address", Model.Htl.Address);
                    CMD.Parameters.AddWithValue("@Hotelcategory", Model.Htl.HotelCategory);
                    CMD.Parameters.AddWithValue("@Features", Model.Htl.Features);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath[0]);
                    CMD.Parameters.AddWithValue("@ThumbnailUrl", ImagePath[1]);
                    CMD.Parameters.AddWithValue("@Description", Model.Htl.Description);
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
            DataTable DT = GetAllHotels();
            CombinedHotelModel MDL = new CombinedHotelModel();
            MDL.LocationList = GetLocations();
            MDL.DT = DT;
            return View(MDL);

        }
        private DataTable GetAllHotels()
        {
            SqlCommand CMD = new SqlCommand("SP_GetHotel");
            CMD.Parameters.AddWithValue("@Action", "ALL_HOTEL");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewHotelAjax()
        {
            return PartialView("HotelTable", GetAllHotels());
        }

        [HttpPost]
        public ActionResult Delete(long ID, String ImageUrl, String ThumbUrl)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageHotel");
                CMD.Parameters.AddWithValue("@Action", "DELETE_HOTEL");
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
            return PartialView("HotelTable", GetAllHotels());
            // return Json(Result);
        }


        [HttpPost]
        public ActionResult Edit(CombinedHotelModel CModel)
            {
            String[] Result = new String[2];
            String[] ImagePath = { CModel.Htl.ImageLink, CModel.Htl.ThumbLink };
            
            bool IsUploaded = false;
            if (ModelState.IsValid)
            {
                try
                {
                    if (CModel.Htl.HotelImage != null && CModel.Htl.HotelImage.ContentLength > 0)
                    {

                        if (CModel.Htl.HotelImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (CModel.Htl.HotelImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            new FileUploadController().DeleteFile(CModel.Htl.ThumbLink);
                            ImagePath = new FileUploadController().UploadFileWithThumb(CModel.Htl.HotelImage, "hotel");

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

                    SqlCommand CMD = new SqlCommand("SP_ManageHotel");
                    CMD.Parameters.AddWithValue("@Action", "UPDATE_HOTEL");
                    CMD.Parameters.AddWithValue("@LocationID", CModel.Htl.LocationID);
                    CMD.Parameters.AddWithValue("@ID", CModel.Htl.ID);
                    CMD.Parameters.AddWithValue("@Name", CModel.Htl.Name);
                    CMD.Parameters.AddWithValue("@Address", CModel.Htl.Address);
                    CMD.Parameters.AddWithValue("@Hotelcategory", CModel.Htl.HotelCategory);
                    CMD.Parameters.AddWithValue("@Features", CModel.Htl.Features);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath[0]);
                    CMD.Parameters.AddWithValue("@ThumbnailUrl", ImagePath[1]);
                    CMD.Parameters.AddWithValue("@Description", CModel.Htl.Description);
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
                        new FileUploadController().DeleteFile(CModel.Htl.ImageLink);
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
