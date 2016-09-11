using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic.Entities
{
    public class DetachResultInfo
    {
        public List<Station> Stations { get; set; }
        public List<Line> Lines { get; set; }
    }
}
