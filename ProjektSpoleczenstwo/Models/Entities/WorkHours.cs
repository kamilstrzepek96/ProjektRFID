using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.Models.Entities
{
    public class WorkHours
    {
        public int Id { get; set; }
        public DateTime PunchTime { get; set; }
        public Employee Employee { get; set; }
    }
}