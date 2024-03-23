using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Travel_Admin_Panel.Models
{
    public class Hotel
    {
        [Required]
        public long LocationID { get; set; }
        public long ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Address { get; set; }
        [Required]
        public String Description { get; set; }
        public HttpPostedFileBase HotelImage { get; set; }
        public String ImageLink { get; set; }
        public String ThumbLink { get; set; }
        [Required]
        public string HotelCategory { get; set; }
        [Required]
        public String Features { get; set; }
    }

    public class CombinedHotelModel
    {
        public Hotel Htl { get; set; }
        public DataTable DT { get; set; }
        public List<SelectListItem> LocationList { get; set; }
    }
}