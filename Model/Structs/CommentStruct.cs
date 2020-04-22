using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SualCavabAPI.Model.Structs
{
    public class CommentStruct
    {
       public int ID { get; set; }
        public int publicationID { get; set; }
        public int userID { get; set; }
       
        public string comment { get; set; }
        public DateTime cDate { get; set; }
    }
}
