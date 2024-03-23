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
    public class TransportController : Controller
 
    {
        //
        // GET: /Transport/

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(Transport Model)
        {
            String[] Result = new String[2];
            String ImagePath = null;
            if (ModelState.IsValid)
            {
                try
                {

                    if (Model.TransportImage != null && Model.TransportImage.ContentLength > 0)
                    {

                        if (Model.TransportImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (Model.TransportImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFile(Model.TransportImage, "transport");
                            if (ImagePath == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageTransport");
                    CMD.Parameters.AddWithValue("@Action", "ADD_TRANSPORT");
                    CMD.Parameters.AddWithValue("@Name", Model.Name);
                    CMD.Parameters.AddWithValue("@ImageURL", ImagePath);
                    CMD.Parameters.AddWithValue("@Features", Model.Features);
                    CMD.Parameters.AddWithValue("@Rate", Model.Rate);
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
            DataTable DT = GetAllTransports();
            CombinedTransportModel MDL = new CombinedTransportModel();
            MDL.DT = DT;
            return View(MDL);

        }
        private DataTable GetAllTransports()
        {
            SqlCommand CMD = new SqlCommand("SP_GetTransport");
            CMD.Parameters.AddWithValue("@Action", "ALL_TRANSPORT");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewTransportAjax()
        {
            return PartialView("TransportTable", GetAllTransports());
        }

        [HttpPost]
        public ActionResult Delete(long ID, String ImageUrl)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManageTransport");
                CMD.Parameters.AddWithValue("@Action", "DELETE_TRANSPORT");
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
            return PartialView("TransportTable", GetAllTransports());
            // return Json(Result);
        }

       

        [HttpPost]
        public ActionResult Edit(CombinedTransportModel CModel)
        {
            String[] Result = new String[2];
            String ImagePath = CModel.Tport.ImageLink;
            if (ModelState.IsValid)
            {
                try
                {

                    if (CModel.Tport.TransportImage != null && CModel.Tport.TransportImage.ContentLength > 0)
                    {

                        if (CModel.Tport.TransportImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 KB";
                            return Json(Result);
                        }
                        else if (CModel.Tport.TransportImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFile(CModel.Tport.TransportImage, "transport");
                            if (ImagePath == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManageTransport");
                    CMD.Parameters.AddWithValue("@Action", "UPDATE_TRANSPORT");
                    CMD.Parameters.AddWithValue("@Name", CModel.Tport.Name);
                    CMD.Parameters.AddWithValue("@ImageURL", ImagePath);
                    CMD.Parameters.AddWithValue("@ID", CModel.Tport.ID);
                    CMD.Parameters.AddWithValue("@Features", CModel.Tport.Features);
                    CMD.Parameters.AddWithValue("@Rate", CModel.Tport.Rate);
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
                        new FileUploadController().DeleteFile(CModel.Tport.ImageLink);
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


    }
}
