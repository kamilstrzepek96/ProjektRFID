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
        [Display(Name = "Stawka/h")]
        [Range(0, int.MaxValue, ErrorMessage = "Wartość musi być liczbą dodatnią")]
        public double Salary { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Praca od")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeSpan FromTime { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [Display(Name = "Praca do")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        public TimeSpan ToTime { get; set; }
    }
}