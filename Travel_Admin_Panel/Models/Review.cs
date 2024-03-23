using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace Travel_Admin_Panel.Models
{
    public class Review
    {

        public long ReviewID { get; set; }
        public long ID { get; set; }
        public long Rating { get; set; }
        public String Reviewer { get; set; }
        public String Address { get; set; }
        public String ReviewTitle { get; set; }
        public String ReviewDescription { get; set; }
    }

    
}