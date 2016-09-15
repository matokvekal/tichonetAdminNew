using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Entities
{
    public class WeekDays
    {
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }

        public bool NothingSelected
        {
            get
            {
                return Monday != true &&
                       Tuesday != true &&
                       Wednesday != true &&
                       Thursday != true &&
                       Friday != true &&
                       Saturday != true &&
                       Sunday != true;
            }
        }
    }
}
