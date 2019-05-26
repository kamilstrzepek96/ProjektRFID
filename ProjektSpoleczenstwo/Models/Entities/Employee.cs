using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.Models.Entities
{
    public class Employee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public string CardUID { get; set; }
        public bool RegisterCard { get; set; }
        public virtual Department Department { get; set; }
        public virtual Jobs Job { get; set; }
        public virtual ICollection<WorkHours> WorkHours { get; set; }
    }
}