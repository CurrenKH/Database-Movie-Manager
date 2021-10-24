using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMM2
{
    class Member
    {
        //  Properties for Member
        public int ID { get; set; }
        public string Name {get;set;}
        public DateTime DOB { get; set; }
        public int Type { get; set; }
        public string ImagePath { get; set; }

        //  Constructor
        public Member()
        {
            ID = 0;
            Name = "";
            DOB = new DateTime();
            Type = 0;
            ImagePath = "";
        }
    }
}
