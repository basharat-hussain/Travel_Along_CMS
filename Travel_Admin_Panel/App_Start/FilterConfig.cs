﻿using System.Web;
using System.Web.Mvc;

namespace Travel_Admin_Panel
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}