using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Data;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Travel_Admin_Panel.Models
{
    public class Destination
    {
        [Required]
        public long LocationID { get; set; }
        public long ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        public String ImageLink { get; set; }
        public String ThumbLink { get; set; }
        public HttpPostedFileBase DestinationImage { get; set; }
    }

    public class CombinedDestinationModel
    {
        public Destination Dest { get; set; }
        public List<SelectListItem> LocationList { get; set; }
        public DataTable DT { get; set; }
    }
}