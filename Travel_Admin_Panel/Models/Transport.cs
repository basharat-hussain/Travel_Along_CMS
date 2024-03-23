using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace Travel_Admin_Panel.Models
{
    public class Transport
    {
        public long ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Features { get; set; }
        public String ImageLink { get; set; }
        public HttpPostedFileBase TransportImage { get; set; }
        [Required]
        public String Rate { get; set; }

    }

    public class CombinedTransportModel
    {
        public DataTable DT { get; set; }
        public Transport Tport { get; set; }
    }
}