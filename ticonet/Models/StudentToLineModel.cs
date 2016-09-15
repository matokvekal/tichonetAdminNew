using System;
using System.Deployment.Internal;
using Business_Logic;

namespace ticonet.Models
{
    public class StudentToLineModel
    {
        public StudentToLineModel() { }

        public StudentToLineModel(StudentsToLine data)
        {
            Id = data.Id;
            StudentId = data.StudentId;
            if (data.LineId == -1)
            {
                LineId = null;
            }
            else
            {
                LineId = data.LineId;
            }
            StationId = data.StationId;
            Direction = data.Direction;
            Date = data.Date;
            Distance = data.distanceFromStation;
            Geometry = data.PathGeometry;
            Mon = data.mon==true; //false if false or null
            Tue = data.tue == true;
            Wed = data.wed == true;
            Thu = data.thu == true;
            Fri = data.fri == true;
            Sat = data.sat == true;
            Sun = data.sun == true;
        }

        public int Id { get; set; }

        public int StudentId { get; set; }

        public int? LineId { get; set; }

        public int StationId { get; set; }

        public int Direction { get; set; }

        public DateTime? Date { get; set; }

        public int? Distance { get; set; }

        public string Geometry { get; set; }

        public string StrDate
        {
            get
            {
                if (!Date.HasValue) return "--";
                return Date.Value.ToShortDateString();
            }
        }

        public string StrTime
        {
            get
            {
                if (!Date.HasValue) return "--";
                return Date.Value.ToString("HH:mm");
            }
        }

        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }

        public bool isDefault
        {
            get
            {
                return Date == null && Mon != true && Tue != true && Wed != true && Thu != true && Fri != true &&
                       Sat != true && Sun != true;
            }
        }
    }
}