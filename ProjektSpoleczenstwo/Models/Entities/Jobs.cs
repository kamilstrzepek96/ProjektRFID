using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.Models.Entities
{
    public class Jobs
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double Salary { get; set; }
        //public int EmployeeId { get; set; }
        //public virtual ICollection<Employee> Employee { get; set; }
    }
}