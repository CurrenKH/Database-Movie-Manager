using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XMM2
{
    class Movie
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public int Length { get; set; }
        public string Director { get; set; }
        public double Rating { get; set; }
        public string ImagePath { get; set; }
        public List<Genre> Genres { get; set; }
        public List<Member> Members { get; set; }
        public Movie()
        {
            ID = 0;
            Title = "";
            Year = 0;
            Length = 0;
            Director = "";
            Rating = 0;
            ImagePath = "";
            Genres = new List<Genre>();
            Members = new List<Member>();
        }
    }
}
