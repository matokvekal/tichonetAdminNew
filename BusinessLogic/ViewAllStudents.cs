using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using Business_Logic.Entities;
using Business_Logic.Helpers;

namespace Business_Logic
{
    public class ViewAllStudentsLogic : baseLogic
    {

        public static List<ViewAllStudentFamilyLinesStation> GetAllStudents()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<ViewAllStudentFamilyLinesStation> c = db.ViewAllStudentFamilyLinesStations.ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
        public static List<ViewAlllinesByBusCompnyAndStation> GetAllLinesAndStations()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<ViewAlllinesByBusCompnyAndStation> c = db.ViewAlllinesByBusCompnyAndStations.ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
        public static List<viewStudentWithStationForEmail> GetAllStudentForEmail()
        {
            try
            {
                BusProjectEntities db = new BusProjectEntities();
                List<viewStudentWithStationForEmail> c = db.viewStudentWithStationForEmails.ToList();
                return c;
            }
            catch
            {
                return null;
            }

        }
    }

}
