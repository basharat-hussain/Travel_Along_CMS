using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;
using Travel_Admin_Panel.Models;
namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class PackageController : Controller
    {
        //
        // GET: /Package/

        public ActionResult Add()
        {
            CombinedPackageModel Mdl = new CombinedPackageModel();
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
                LstLocation.Add(new SelectListItem() { Text = row["LocationName"].ToString(), Value = row["LocationID"].ToString()});
            }
            return LstLocation;
        }


        [HttpPost]
        public ActionResult Add(FormCollection FormValues, CombinedPackageModel Model)
        {

            //CombinedPackageModel Model = new CombinedPackageModel();
            String[] Result = new String[2];
            String[] ImagePath = new String[2];
            if (ModelState.IsValid)
            {
                try
                {

                    if (Model.Pkg.PackageImage != null && Model.Pkg.PackageImage.ContentLength > 0)
                    {
                        if (Model.Pkg.PackageImage.ContentLength > 204800)
                        {
                            Result[0] = "False";
                            Result[1] = "Please select a file less than 200 kb";
                            return Json(Result);
                        }
                        else if (Model.Pkg.PackageImage.ContentType != "image/jpeg")
                        {
                            Result[0] = "False";
                            Result[1] = "Invalid file selected. Only jpg allowed";
                            return Json(Result);
                        }
                        else
                        {
                            ImagePath = new FileUploadController().UploadFileWithThumb(Model.Pkg.PackageImage, "package");
                            if (ImagePath[0] == null || ImagePath[1] == null)
                            {
                                Result[0] = "False";
                                Result[1] = "Image uploading error.";
                                return Json(Result);
                            }

                        }
                    }



                    DataTable DTPackageDays = new DataTable();
                    DTPackageDays.Columns.Add("DayTitle");
                    DTPackageDays.Columns.Add("DayDescription");
                    DTPackageDays.Columns.Add("DayNumber");
                    int DayCount = Convert.ToInt32(FormValues.Get("DayCount"));
                    for (int i = 1; i <= DayCount; i++)
                    {
                        String DayTitle = FormValues.Get("Pkg_DayTitle" + i);
                        String DayDescription = FormValues.Get("Pkg_Day" + i);
                        String DayNumber = "Day" + i;
                        DTPackageDays.Rows.Add(DayTitle, DayDescription, DayNumber);
                    }

                    SqlCommand CMD = new SqlCommand("SP_ManagePackage");
                    CMD.Parameters.AddWithValue("@Action", "ADD_PACKAGE");
                    CMD.Parameters.AddWithValue("@LocationID", Model.Pkg.LocationID);
                    CMD.Parameters.AddWithValue("@Title", Model.Pkg.Title);
                    CMD.Parameters.AddWithValue("@Duration", Model.Pkg.Duration);
                    CMD.Parameters.AddWithValue("@Description", Model.Pkg.Description);
                    CMD.Parameters.AddWithValue("@Inclusions", Model.Pkg.Inclusions);
                    CMD.Parameters.AddWithValue("@Exclusions", Model.Pkg.Exclusions);
                    CMD.Parameters.AddWithValue("@ImageUrl", ImagePath[0]);
                    CMD.Parameters.AddWithValue("@ThumbnailUrl", ImagePath[1]);
                    CMD.Parameters.AddWithValue("@Rate", Model.Pkg.Rate);
                    CMD.Parameters.AddWithValue("@PackageType", DTPackageDays);
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
            DataTable DT = GetAllPackages();
            CombinedPackageModel MDL = new CombinedPackageModel();
            MDL.LocationList = GetLocations();
            MDL.DT = DT;
            return View(MDL);

        }
        private DataTable GetAllPackages()
        {
            SqlCommand CMD = new SqlCommand("SP_GetPackage");
            CMD.Parameters.AddWithValue("@Action", "ALL_PACKAGES");

            DBConnection CON = new DBConnection();
            DataTable DT = CON.ExecuteSPQueryDT(CMD);
            return DT;
        }

        public ActionResult ViewPackagesAjax()
        {
            return PartialView("DestinationTable", GetAllPackages());
        }

        public ActionResult GetPackageView(long id)
        {
            return PartialView("PackageFullView", GetPackageByID(id));
        }

        public ActionResult GetPackageEdit(long id)
        {
            CombinedPackageModel CModel = new CombinedPackageModel();
            CModel.DS = GetPackageByID(id);
            CModel.LocationList = GetLocations();
            return PartialView("PackageEditView", CModel);
        }



        private DataSet GetPackageByID(long id)
        {
            SqlCommand CMD = new SqlCommand("SP_GetPackage");
            CMD.Parameters.AddWithValue("@Action", "GET_BY_ID");
            CMD.Parameters.AddWithValue("@ID", id);

            DBConnection CON = new DBConnection();
            DataSet DS = CON.ExecuteSPQueryDS(CMD);
            return DS;
        }

        [HttpPost]
        public ActionResult Delete(long ID, String ImageUrl, String ThumbUrl)
        {
            String[] Result = new String[2];
            try
            {
                SqlCommand CMD = new SqlCommand("SP_ManagePackage");
                CMD.Parameters.AddWithValue("@Action", "DELETE_PACKAGE");
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
            return PartialView("PackageTable", GetAllPackages());
            // return Json(Result);
        }

       
    }
}
