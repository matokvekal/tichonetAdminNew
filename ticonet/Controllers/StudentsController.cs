using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.Http;
using Business_Logic;
using Business_Logic.Entities;
using Business_Logic.Helpers;
using log4net;
using ticonet.Models;
using ticonet.Models.viewModels;

namespace ticonet
{
    public class StudentsController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(StudentsController));




        [ActionName("StudentsForMap")]
        public List<StudentShortInfo> GetStudentsForMap()
        {

            var res = new List<StudentShortInfo>();
            using (var context = new BusProjectEntities())
            {
                res = context.tblStudents.Select(data => new StudentShortInfo
                {
                    Id = data.pk,
                    StudentId = data.studentId,
                    Lat = data.Lat,
                    Lng = data.Lng,
                    Color = string.IsNullOrEmpty(data.Color.Trim()) ? "FF0000" : data.Color.Trim().Replace("#", ""),
                    Name = data.lastName + ", " + data.firstName,
                    CellPhone = data.CellPhone,
                    Email = data.Email,
                    Address = (data.city ?? "") + ", " + (data.street ?? "") + ", " + data.houseNumber,
                    Shicva = data.Shicva,
                    Class = data.@class,
                    Active = data.Active ?? false,
                    SchoolName = data.schoolName
                }).ToList();
            }
            return res;
        }

        [ActionName("SaveCoords")]
        public void GetSaveCoords(int id, string lat, string lng)
        {
            double pLat = 0;
            double pLng = 0;

            string strLat = StringHelper.FixDecimalSeparator(lat);
            string strLng = StringHelper.FixDecimalSeparator(lng);


            if (!double.TryParse(strLat, out pLat)) return;
            if (!double.TryParse(strLng, out pLng)) return;
            using (var context = new BusProjectEntities())
            {
                var st = context.tblStudents.FirstOrDefault(z => z.pk == id);
                if (st != null)
                {
                    st.Lat = pLat;
                    st.Lng = pLng;
                    try
                    {
                        context.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }
        }

        [ActionName("Family")]
        public object GetFamily(int id)
        {
            tblFamily res = null;
            using (var context = new BusProjectEntities())
            {

                var st = context.tblStudents.FirstOrDefault(z => z.pk == id);
                if (st != null)
                {
                    res = context.tblFamilies.FirstOrDefault(z => z.familyId == st.familyId);
                }
            }

            return new StudentFamilyInfo { Id = id, Family = (res != null ? new FamilyModel(res) : new FamilyModel()) };
        }

        [ActionName("Address")]
        public StudentAddressViewModel GetAddress(int id)
        {
            StudentAddressViewModel res = null;
            using (var logic = new tblStudentLogic())
            {
                var st = logic.getStudentByPk(id);
                if (st != null)
                {
                    res = new StudentAddressViewModel
                    {
                        StudentId = st.pk,
                        City = st.city,
                        CityId = st.cityId ?? 0,
                        Street = st.street,
                        StreetId = st.streetId ?? 0,
                        HouseNumber = st.houseNumber,
                        Confirm = st.registrationStatus
                    };
                }
            }
            return res;
        }

        [ActionName("Address")]
        public AttachStudentResultModel PostAddress(StudentAddressViewModel data)
        {
            var res = new AttachStudentResultModel
            {
                Done = false,
                Lines = new List<LineModel>(),
                Stations = new List<StationModel>()

            };
            try
            {
                using (var logic = new tblStudentLogic())
                {
                    var st = logic.getStudentByPk(data.StudentId);
                    if (st != null)
                    {
                        if (st.cityId != data.CityId ||
                            st.streetId != data.StreetId ||
                            st.houseNumber != data.HouseNumber)
                        {
                            st.street = data.Street;
                            st.streetId = data.StreetId;
                            st.city = data.City;
                            st.cityId = data.CityId;
                            st.houseNumber = data.HouseNumber;
                            st.Lat = null;
                            st.Lng = null;
                            tblStudentLogic.update(st);

                            //disconnect from all lines
                            var detachResult = logic.RemoveStudentFromAllStations(st.pk);
                            res.Student = new StudentShortInfo(st);

                            res.Stations = detachResult.Stations.Select(d => new StationModel(d)).ToList();
                            res.Lines = detachResult.Lines.Select(d => new LineModel(d)).ToList();

                            res.Done = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                res.Done = false;
            }
            return res;
        }
    }

    public class StudentFamilyInfo
    {
        public int Id { get; set; }
        public FamilyModel Family { get; set; }
    }
}
