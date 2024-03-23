using System;
using System.ComponentModel.DataAnnotations;

namespace Travel_Admin_Panel.Models
{
    public class User
    {
        [Required]
        public String UserName { get; set; }
        [Required]
        public String Password { get; set; }
    }

    public class ChangePassword
    {
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
        [Required]
        public string RepeatPassword { get; set; }

    }
}
