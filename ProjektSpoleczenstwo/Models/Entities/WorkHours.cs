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
        public PunchEnum PunchType { get; set; }
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public TimeSpan ElapsedTime { get; set; }
    }
}