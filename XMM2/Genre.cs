using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMM2
{
    class Genre
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Genre()
        {
            Code = "";
            Name = "";
            Description = "";
        }

    }
}
