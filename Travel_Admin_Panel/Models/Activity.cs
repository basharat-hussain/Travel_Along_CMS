using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Travel_Admin_Panel.Models
{
    public class Activity
    {
        [Required]
        public long LocationID { get; set; }
        public long ID { get; set; }
        [Required]
        public String Title { get; set; }
        [Required]
        public String Description { get; set; }
        public String ImageLink { get; set; }
        //[Required]
        public HttpPostedFileBase ActivityImage { get; set; }
    }

    public class CombinedActivityModel
    {
        public Activity Actvty { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public DataTable DT { get; set; }
    }
}