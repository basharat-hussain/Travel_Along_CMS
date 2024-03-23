using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using Travel_Admin_Panel.App_Code;

namespace Travel_Admin_Panel.Controllers
{
    [SessionAuthorize]
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Dashboard()
        {
            SqlCommand CMD = new SqlCommand("sp_GetDashboardWidgets");
            DBConnection DB = new DBConnection();
            DataTable DT = DB.ExecuteSPQueryDT(CMD);
            return View(DT);
        }

    }
}
