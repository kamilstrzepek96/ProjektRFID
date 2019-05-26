using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.ViewModels
{
    public class SummaryViewModel
    {
        public string Employee { get; set; }
        public double Salary { get; set; }
        public string Department { get; set; }
        public string Job { get; set; }
        public int WorkHours { get; set; }
    }
}