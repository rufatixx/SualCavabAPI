using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SualCavabAPI.Model.Structs
{
    public class NewPublication
    {
        public string mail { get; set; }
        public string pass { get; set; }
        public string name { get; set; }
        public int topicID { get; set; }
        public int backgroundImageID { get; set; }

    }
}
