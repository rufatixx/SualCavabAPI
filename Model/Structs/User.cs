using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SualCavabAPI.Model.Database
{
    public class User
    {

        public int ID { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public string mail { get; set; }
        public string phone { get; set; }
        public DateTime birthDate { get; set; }

        public string gender { get; set; }
        public string city { get; set; }
        public string profession { get; set; }

        public DateTime regDate { get; set; }
    }
}
