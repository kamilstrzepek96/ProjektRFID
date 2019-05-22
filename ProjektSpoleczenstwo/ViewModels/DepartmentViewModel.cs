using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.ViewModels
{
    public class DepartmentViewModel
    {
        [Required]
        [Display(Name = "Miasto")]
        public string Location { get; set; }
        [Required]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
    }
}