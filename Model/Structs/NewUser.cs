using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SualCavabAPI.Model.Structs
{
    public class NewUser
    {
        public string mail { get; set; }
        public string pass { get; set; }
        public string name { get; set; }
        public string surname { get; set; }
        public int professionID { get; set; }

    }
}
