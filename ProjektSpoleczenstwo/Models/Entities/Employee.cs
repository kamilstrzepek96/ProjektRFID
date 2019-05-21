using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.Models.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Age { get; set; }
        public string Token { get; set; }
        public ICollection<WorkHours> WorkHours { get; set; }
    }
}