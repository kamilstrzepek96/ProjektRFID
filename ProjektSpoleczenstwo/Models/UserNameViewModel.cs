using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjektSpoleczenstwo.Models
{
    public class UserNameViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string CardUID { get; set; }
        public bool CardRequest { get; set; }
    }
}