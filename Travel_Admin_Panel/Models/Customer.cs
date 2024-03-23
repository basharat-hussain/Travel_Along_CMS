using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace Travel_Admin_Panel.Models
{
    public class Customer
    {
        
        public long ID { get; set; }
        [Required]
        public String Name { get; set; }
        [Required]
        public String Email { get; set; }
        [Required]
        public String Address { get; set; }
        [Required]
        public String Phone { get; set; }

    }

    public class CombinedCustomerModel
    {
        public Customer Cust { get; set; }
        public DataTable DT { get; set; }
    }
}