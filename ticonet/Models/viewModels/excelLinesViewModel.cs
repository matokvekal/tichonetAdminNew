using System.Collections.Generic;
using Business_Logic;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ticonet
{
    public class excelLinesViewModel
    {
        public int[] SelectedValues { get; set; }
      //  public Line LineTable { get; set; }
        public List<Line> allLines { get; set; }
    }


}