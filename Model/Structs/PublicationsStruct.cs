using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SualCavabAPI.Model.Structs
{
    public class PublicationsStruct
    {

        public int id { get; set; }

        public string publisher { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int reaction { get; set; }
        public int aTypeId { get; set; }
        public int commentCount { get; set; }
        public int reactionCount { get; set; }
        public int topicId { get; set; }
        public string topicName { get; set; }
        public DateTime cDate { get; set; }

        public int views { get; set; }


    }
}
