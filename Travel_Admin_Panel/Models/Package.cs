using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace Travel_Admin_Panel.Models
{
    public class Package
    {
        public long ID { get; set; }

        [Required]
        public long LocationID { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        public string Duration { get; set; }

        public String Description { get; set; }

        [Required]
        public string Inclusions { get; set; }

        [Required]
        public string Exclusions { get; set; }

        [Required]
        public HttpPostedFileBase PackageImage { get; set; }

        public String ImageLink { get; set; }

        public string Rate { get; set; }
    }
    public class CombinedPackageModel
    {
        public Package Pkg { get; set; }
        public DataTable DT { get; set; }
        public DataSet DS { get; set; }
        public List<SelectListItem> LocationList { get; set; }
    }
}