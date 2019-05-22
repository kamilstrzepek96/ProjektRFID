using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.ViewModels
{
    public class JobViewModel
    {
        [Required]
        [Display(Name = "Stanowisko")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "Pensja")]
        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być liczbą dodatnią")]
        public double Salary { get; set; }
    }
}