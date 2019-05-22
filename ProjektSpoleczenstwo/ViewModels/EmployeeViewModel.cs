using ProjektSpoleczenstwo.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjektSpoleczenstwo.ViewModels
{
    public class EmployeeViewModel
    {
        [Required]
        [Display(Name = "Imię")]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }
        [Required]
        [Display(Name = "Wiek")]
        [Range(18, int.MaxValue, ErrorMessage = "Wymagany wiek to 18 lat")]
        public int Age { get; set; }
        [Required]
        [Display(Name = "Wydział")]
        public int DepartmentId { get; set; }
        [Required]
        [Display(Name = "Stanowisko")]
        public int JobId { get; set; }
        public List<SelectListItem> Jobs { get; set; }
        public List<SelectListItem> Deps { get; set; }
        //public List<string> DepartmentName { get; set; }
        //public List<string> JobName { get; set; }
    }
}