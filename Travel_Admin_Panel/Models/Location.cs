using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace Travel_Admin_Panel.Models
{
    public class Location
    {
        public long ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Description { get; set; }
        public String ImageLink { get; set; }
        public HttpPostedFileBase LocationImage { get; set; }
    }

    public class CombinedLocationModel
    {
        public Location Loc { get; set; }
        public DataTable DT { get; set; }
    }
}