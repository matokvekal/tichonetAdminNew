using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Business_Logic;
using ClosedXML.Excel;
using log4net;
using DEBS = Business_Logic.DictExpressionBuilderSystem;
using System.Threading;
using System.Net.Mail;
using System.Net;


namespace ticonet
{
    public class ManageController : Controller
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(ManageController));

        public ActionResult Index()
        {
            Session["lineList"] = null;
            ViewBag.totalStudents = tblStudentLogic.totalStudents();
            ViewBag.totalFamilies = tblFamilyLogic.totalRegistrationStudentsFamilies();
            ViewBag.totalRegistrationStudents = tblStudentLogic.totalRegistrationStudents();
            ViewBag.manageMessage = TempData["manageMessage"];


            excelLinesViewModel lineViewModel = new excelLinesViewModel();

            using (var logic = new LineLogic())
            {
                lineViewModel.allLines = logic.GetList();


                foreach (var item in lineViewModel.allLines)
                {
                    item.LineName = item.LineName + " " + item.LineNumber;
                }

                return View(lineViewModel);
            }
        }

        public void btnExportToExcel_Click2(object sender, EventArgs e)
        {
            logger.Info("btnExportToExcel_Click2 ");
            List<ViewAllStudentFamilyLinesStation> c = ViewAllStudentsLogic.GetAllStudents();
            logger.Info("after list ");
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));

            StringBuilder sb = new StringBuilder();
            sb.Append("<table>");

            //Compose header row
            sb.Append("<tr>");
            sb.Append("<th>" + DEBS.Translate("Report.totalPrice") + "</th>");
            sb.Append("</tr>");


            //   worksheet.Cell(row, "G").Value = stud.PayStatus == true ? "Yes" : "No";
            //foreach (tblMain row in list)
            //{
            sb.Append("<tr>");
            //sb.Append(String.Format("<td>{0}</td>", row.FirstName));
            //sb.Append(String.Format("<td>{0}</td>", row.LastName));
            //sb.Append(String.Format("<td>{0}</td>", row.Email1));
            sb.Append("</tr>");
            //}
            sb.Append("</table>");

            ExportToExcel(sb.ToString());
        }

        public ActionResult deleteDupStudentsTolines()
        {
            ManageLogic.deleteDuplicatedStudentToLines();
            return RedirectToAction("index", "Manage");
        }

        private void ExportToExcel(string content)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition", "attachment;filename=Book1.xls");
            Response.Charset = "";
            //  this.EnableViewState = false;

            System.IO.StringWriter sw = new System.IO.StringWriter();
            System.Web.UI.HtmlTextWriter htw = new System.Web.UI.HtmlTextWriter(sw);

            htw.WriteLine(content);

            Response.Write(sw.ToString());
            Response.End();
        }


        //[HttpPost]
        //[Authorize]
        public FileContentResult btnExportToExcel_Click()
        {

            LineLogic.updateAriveAndDepartureTime();//update table Lines- insert data to BasicArriveTime & BasicDepartureTime

            List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));



            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            worksheet.Cell("A1").Value = DEBS.Translate("General.siduri");
            worksheet.Cell("B1").Value = DEBS.Translate("tblStudent.studentId");
            worksheet.Cell("C1").Value = DEBS.Translate("tblStudent.firstName");
            worksheet.Cell("D1").Value = DEBS.Translate("tblStudent.lastName");
            worksheet.Cell("E1").Value = DEBS.Translate("tblStudent.class");
            worksheet.Cell("F1").Value = DEBS.Translate("tblStudent.city");
            worksheet.Cell("G1").Value = DEBS.Translate("tblStudent.street");
            worksheet.Cell("H1").Value = DEBS.Translate("tblStudent.houseNumber");
            worksheet.Cell("I1").Value = DEBS.Translate("tblStudent.CellPhone");
            worksheet.Cell("J1").Value = DEBS.Translate("tblStudent.Email");
            worksheet.Cell("K1").Value = DEBS.Translate("tblStudent.registrationStatus");
            worksheet.Cell("L1").Value = DEBS.Translate("tblFamily.subsidy");
            worksheet.Cell("M1").Value = DEBS.Translate("tblStudent.ActiveOnscreen");
            worksheet.Cell("N1").Value = DEBS.Translate("tblStudent.siblingAtSchool");
            worksheet.Cell("O1").Value = DEBS.Translate("excel.parent1FirstName");
            worksheet.Cell("P1").Value = DEBS.Translate("excel.parent1LastName");
            worksheet.Cell("Q1").Value = DEBS.Translate("excel.parent1Email");
            worksheet.Cell("R1").Value = DEBS.Translate("excel.parent1CellPhone");
            worksheet.Cell("S1").Value = DEBS.Translate("excel.parent2FirstName");
            worksheet.Cell("T1").Value = DEBS.Translate("excel.parent2Email");
            worksheet.Cell("U1").Value = DEBS.Translate("excel.parent2CellPhone");
            worksheet.Cell("V1").Value = DEBS.Translate("tblFamily.subsidy");
            worksheet.Cell("W1").Value = DEBS.Translate("tblFamily.Active");
            worksheet.Cell("X1").Value = DEBS.Translate("studentToLoines.distanceFromStation");
            worksheet.Cell("Y1").Value = DEBS.Translate("Line.LineName");
            worksheet.Cell("Z1").Value = DEBS.Translate("Line.LineNumber");
            worksheet.Cell("AA1").Value = DEBS.Translate("tblStudent.school");
            worksheet.Cell("AB1").Value = DEBS.Translate("Line.Duration");
            worksheet.Cell("AC1").Value = DEBS.Translate("Stations.StationName");
            worksheet.Cell("AD1").Value = DEBS.Translate("StationsToLines.ArrivalDate");
            worksheet.Cell("AE1").Value = DEBS.Translate("StationsToLines.ArrivalTimeSun");
            worksheet.Cell("AF1").Value = DEBS.Translate("StationsToLines.ArrivalTimeMon");
            worksheet.Cell("AG1").Value = DEBS.Translate("StationsToLines.ArrivalTimeTue");
            worksheet.Cell("AH1").Value = DEBS.Translate("StationsToLines.ArrivalTimeWed");
            worksheet.Cell("AI1").Value = DEBS.Translate("StationsToLines.ArrivalTimeThu");
            worksheet.Cell("AJ1").Value = DEBS.Translate("StationsToLines.ArrivalTimeFri");
            worksheet.Cell("AK1").Value = DEBS.Translate("StationsToLines.ArrivalTimeSat");


            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("A").Width = 6;
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("B").Width = 12;
            worksheet.Cell("C1").Style.Font.Bold = true;
            worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("C").Width = 12;
            worksheet.Cell("D1").Style.Font.Bold = true;
            worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("D").Width = 8;
            worksheet.Cell("E1").Style.Font.Bold = true;
            worksheet.Cell("E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("E").Width = 8;
            worksheet.Cell("F1").Style.Font.Bold = true;
            worksheet.Cell("F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("F").Width = 20;
            worksheet.Cell("G1").Style.Font.Bold = true;
            worksheet.Cell("G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("G").Width = 8;
            worksheet.Cell("H1").Style.Font.Bold = true;
            worksheet.Cell("H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("H").Width = 8;
            worksheet.Cell("I1").Style.Font.Bold = true;
            worksheet.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("I").Width = 8;
            worksheet.Cell("J1").Style.Font.Bold = true;
            worksheet.Cell("J1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("J").Width = 14;
            worksheet.Cell("K1").Style.Font.Bold = true;
            worksheet.Cell("K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("K").Width = 16;
            worksheet.Cell("L1").Style.Font.Bold = true;
            worksheet.Cell("L1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("L").Width = 20;

            worksheet.Cell("M1").Style.Font.Bold = true;
            worksheet.Cell("M1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("M").Width = 6;
            worksheet.Cell("N1").Style.Font.Bold = true;
            worksheet.Cell("N1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("N").Width = 12;
            worksheet.Cell("O1").Style.Font.Bold = true;
            worksheet.Cell("O1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("O").Width = 12;
            worksheet.Cell("P1").Style.Font.Bold = true;
            worksheet.Cell("P1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("P").Width = 8;
            worksheet.Cell("Q1").Style.Font.Bold = true;
            worksheet.Cell("Q1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("Q").Width = 8;
            worksheet.Cell("R1").Style.Font.Bold = true;
            worksheet.Cell("R1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("R").Width = 20;
            worksheet.Cell("S1").Style.Font.Bold = true;
            worksheet.Cell("S1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("S").Width = 8;
            worksheet.Cell("T1").Style.Font.Bold = true;
            worksheet.Cell("T1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("T").Width = 8;
            worksheet.Cell("U1").Style.Font.Bold = true;
            worksheet.Cell("U1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("U").Width = 8;
            worksheet.Cell("V1").Style.Font.Bold = true;
            worksheet.Cell("V1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("V").Width = 14;
            worksheet.Cell("W1").Style.Font.Bold = true;
            worksheet.Cell("W1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("W").Width = 16;
            worksheet.Cell("X1").Style.Font.Bold = true;
            worksheet.Cell("X1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("X").Width = 20;

            worksheet.Cell("Y1").Style.Font.Bold = true;
            worksheet.Cell("Y1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("Y").Width = 6;
            worksheet.Cell("Z1").Style.Font.Bold = true;
            worksheet.Cell("Z1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("Z").Width = 12;
            worksheet.Cell("AA1").Style.Font.Bold = true;
            worksheet.Cell("AA1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AA").Width = 12;
            worksheet.Cell("AB1").Style.Font.Bold = true;
            worksheet.Cell("AB1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AB").Width = 8;
            worksheet.Cell("AC1").Style.Font.Bold = true;
            worksheet.Cell("AC1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AC").Width = 8;
            worksheet.Cell("AD1").Style.Font.Bold = true;
            worksheet.Cell("AD1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AD").Width = 8;
            worksheet.Cell("AE1").Style.Font.Bold = true;
            worksheet.Cell("AE1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AE").Width = 8;
            worksheet.Cell("AF1").Style.Font.Bold = true;
            worksheet.Cell("AF1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AF").Width = 8;
            worksheet.Cell("AG1").Style.Font.Bold = true;
            worksheet.Cell("AG1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AG").Width = 8;
            worksheet.Cell("AH1").Style.Font.Bold = true;
            worksheet.Cell("AH1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AH").Width = 8;
            worksheet.Cell("AI1").Style.Font.Bold = true;
            worksheet.Cell("AI1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AI").Width = 8;
            worksheet.Cell("AJ1").Style.Font.Bold = true;
            worksheet.Cell("AJ1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AJ").Width = 8;
            worksheet.Cell("AK1").Style.Font.Bold = true;
            worksheet.Cell("AK1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("AK").Width = 8;


            var row = 2;
            foreach (var stud in lst)
            {

                worksheet.Cell(row, "A").Value = row - 1;
                worksheet.Cell(row, "B").Value = stud.studentId;
                worksheet.Cell(row, "C").Value = stud.studentFirstName;
                worksheet.Cell(row, "D").Value = stud.studentLastName;
                worksheet.Cell(row, "E").Value = stud.@class;
                worksheet.Cell(row, "F").Value = stud.city;
                worksheet.Cell(row, "G").Value = stud.street;
                worksheet.Cell(row, "H").Value = stud.houseNumber;
                worksheet.Cell(row, "I").Value = "'" + stud.studentCellPhone;
                worksheet.Cell(row, "J").Value = stud.studentEmail;
                worksheet.Cell(row, "K").Value = stud.registrationStatus == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "L").Value = stud.subsidy == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "M").Value = stud.studentActive == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "N").Value = stud.siblingAtSchool == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "O").Value = stud.parent1FirstName;
                worksheet.Cell(row, "P").Value = stud.parent1LastName;
                worksheet.Cell(row, "Q").Value = stud.parent1Email;
                worksheet.Cell(row, "R").Value = "'" + stud.parent1CellPhone;
                worksheet.Cell(row, "S").Value = stud.parent2FirstName;
                worksheet.Cell(row, "T").Value = stud.parent2Email;
                worksheet.Cell(row, "U").Value = "'" + stud.parent2CellPhone;
                worksheet.Cell(row, "V").Value = stud.subsidy == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "W").Value = stud.studentActive == true ? DEBS.Translate("general.Yes") : DEBS.Translate("general.No");
                worksheet.Cell(row, "X").Value = stud.distanceFromStation;
                worksheet.Cell(row, "Y").Value = stud.LineName;
                worksheet.Cell(row, "Z").Value = stud.LineNumber;
                worksheet.Cell(row, "AA").Value = stud.schoolName;
                worksheet.Cell(row, "AB").Value = stud.Duration;
                worksheet.Cell(row, "AC").Value = stud.StationName;
                worksheet.Cell(row, "AD").Value = stud.ArrivalDate;//time for bus in student station
                //     calculate diferent times in each day for same line(if there are)
                if (stud.LineNumber != null || stud.Linedirection != null)//student is connect to line
                {
                    //  logger.Info("in time span " + stud.LineNumber + " " + stud.Linedirection);
                    TimeSpan basic = new TimeSpan();
                    TimeSpan ArriveDate = new TimeSpan();
                    if (stud.ArrivalDate != null)
                        ArriveDate = stud.ArrivalDate.Value;
                    if (stud.Linedirection.Value == 0)
                        if (stud.BasicArriveTime != null)
                        {
                            basic = stud.BasicArriveTime.Value.TimeOfDay;
                            logger.Info("basicA " + basic);
                        }
                        else
                        {
                            if (stud.BasicDepartureTime != null)
                                basic = stud.BasicDepartureTime.Value.TimeOfDay;
                        }
                    if (stud.BasicArriveTime != null || stud.BasicDepartureTime != null)
                    {
                        //string p = (ArriveDate + stud.SunTime.Value.TimeOfDay - basic).ToString();
                        //TimeSpan r = (ArriveDate + stud.MonTime.Value.TimeOfDay - basic);
                        //TimeSpan v = (ArriveDate + stud.TueTime.Value.TimeOfDay - basic);
                        worksheet.Cell(row, "AE").Value = (stud.SunTime != null) ? (ArriveDate + stud.SunTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AF").Value = (stud.MonTime != null) ? (ArriveDate + stud.MonTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AG").Value = (stud.TueTime != null) ? (ArriveDate + stud.TueTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AH").Value = (stud.WedTime != null) ? (ArriveDate + stud.WedTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AI").Value = (stud.ThuTime != null) ? (ArriveDate + stud.ThuTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AJ").Value = (stud.FriTime != null) ? (ArriveDate + stud.FriTime.Value.TimeOfDay - basic).ToString() : null;
                        worksheet.Cell(row, "AK").Value = (stud.SutTime != null) ? (ArriveDate + stud.SutTime.Value.TimeOfDay - basic).ToString() : null;
                    }
                }

                row++;
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }



        public FileContentResult btnExportToExcel2()
        {

            LineLogic.updateAriveAndDepartureTime();

            List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));




            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            worksheet.Cell("A1").Value = DEBS.Translate("General.siduri");
            worksheet.Cell("B1").Value = DEBS.Translate("tblStudent.firstName");
            worksheet.Cell("C1").Value = DEBS.Translate("tblStudent.lastName");
            worksheet.Cell("D1").Value = DEBS.Translate("tblStudent.city");
            worksheet.Cell("E1").Value = DEBS.Translate("tblStudent.street");
            worksheet.Cell("F1").Value = DEBS.Translate("tblStudent.houseNumber");
            worksheet.Cell("G1").Value = DEBS.Translate("Line.LineName");
            worksheet.Cell("H1").Value = DEBS.Translate("Line.LineNumber");
            worksheet.Cell("I1").Value = DEBS.Translate("Line.Duration");
            worksheet.Cell("J1").Value = DEBS.Translate("Stations.StationName");
            worksheet.Cell("K1").Value = DEBS.Translate("StationsToLines.ArrivalDate");



            worksheet.Cell("A1").Style.Font.Bold = true;
            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("A").Width = 6;
            worksheet.Cell("B1").Style.Font.Bold = true;
            worksheet.Cell("B1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("B").Width = 12;
            worksheet.Cell("C1").Style.Font.Bold = true;
            worksheet.Cell("C1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("C").Width = 12;
            worksheet.Cell("D1").Style.Font.Bold = true;
            worksheet.Cell("D1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("D").Width = 8;
            worksheet.Cell("E1").Style.Font.Bold = true;
            worksheet.Cell("E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("E").Width = 8;
            worksheet.Cell("F1").Style.Font.Bold = true;
            worksheet.Cell("F1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("F").Width = 20;
            worksheet.Cell("G1").Style.Font.Bold = true;
            worksheet.Cell("G1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("G").Width = 8;
            worksheet.Cell("H1").Style.Font.Bold = true;
            worksheet.Cell("H1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("H").Width = 8;
            worksheet.Cell("I1").Style.Font.Bold = true;
            worksheet.Cell("I1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("I").Width = 8;
            worksheet.Cell("J1").Style.Font.Bold = true;
            worksheet.Cell("J1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("J").Width = 14;
            worksheet.Cell("K1").Style.Font.Bold = true;
            worksheet.Cell("K1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("K").Width = 16;

            string lineNumberPrev = null;
            var row = 2;
            lst = lst.OrderBy(c => c.LineNumber).ThenBy(c => c.Position).ToList();

            foreach (var stud in lst)
            {
                if (row > 2 && lineNumberPrev != stud.LineNumber)
                    row++;

                worksheet.Cell(row, "A").Value = row - 1;
                worksheet.Cell(row, "B").Value = stud.studentFirstName;
                worksheet.Cell(row, "C").Value = stud.studentLastName;
                worksheet.Cell(row, "D").Value = stud.city;
                worksheet.Cell(row, "E").Value = stud.street;
                worksheet.Cell(row, "F").Value = stud.houseNumber;
                worksheet.Cell(row, "G").Value = stud.LineName;
                worksheet.Cell(row, "H").Value = lineNumberPrev = stud.LineNumber;
                worksheet.Cell(row, "I").Value = stud.Duration;

                //     calculate diferent times in each day for same line(if there are)
                //if (stud.LineNumber != null || stud.Linedirection != null)//student is connect to line
                //{
                //    TimeSpan basic = new TimeSpan();
                //    TimeSpan ArriveDate = new TimeSpan();
                //    if (stud.ArrivalDate != null)
                //        ArriveDate = stud.ArrivalDate.Value;
                //    if (stud.Linedirection.Value == 0)
                //        if (stud.BasicArriveTime != null)
                //        {
                //            basic = stud.BasicArriveTime.Value.TimeOfDay;
                //            logger.Info("basicA " + basic);
                //        }
                //        else
                //        {
                //            if (stud.BasicDepartureTime != null)
                //                basic = stud.BasicDepartureTime.Value.TimeOfDay;
                //        }
                //    if (stud.BasicArriveTime != null || stud.BasicDepartureTime != null)
                //    {

                //        worksheet.Cell(row, "AE").Value = (stud.SunTime != null) ? (ArriveDate + stud.SunTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AF").Value = (stud.MonTime != null) ? (ArriveDate + stud.MonTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AG").Value = (stud.TueTime != null) ? (ArriveDate + stud.TueTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AH").Value = (stud.WedTime != null) ? (ArriveDate + stud.WedTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AI").Value = (stud.ThuTime != null) ? (ArriveDate + stud.ThuTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AJ").Value = (stud.FriTime != null) ? (ArriveDate + stud.FriTime.Value.TimeOfDay - basic).ToString() : null;
                //        worksheet.Cell(row, "AK").Value = (stud.SutTime != null) ? (ArriveDate + stud.SutTime.Value.TimeOfDay - basic).ToString() : null;
                //    }
                //}

                row++;
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }
        public FileContentResult btnExportToExcel3()//lines
        {

            LineLogic.updateAriveAndDepartureTime();

            List<ViewAlllinesByBusCompnyAndStation> lst = ViewAllStudentsLogic.GetAllLinesAndStations();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));



            var workbook = new XLWorkbook();
            //   var worksheet = workbook.Worksheets.Add("אקסל  קוים ותחנות ");
            //worksheet.Cell("A" + headerRow).Value = DEBS.Translate("BusCompany.Name");
            //worksheet.Cell("B" + headerRow).Value = DEBS.Translate("Line.LineName");
            //worksheet.Cell("C" + headerRow).Value = DEBS.Translate("Line.LineNumber");
            //worksheet.Cell("D" + headerRow).Value = DEBS.Translate("Line.Direction");
            //worksheet.Cell("E" + headerRow).Value = DEBS.Translate("Line.Duration");
            //worksheet.Cell("F" + headerRow).Value = DEBS.Translate("Line.totalStudents");
            //worksheet.Cell("G" + headerRow).Value = DEBS.Translate("Line.BasicArriveTime");
            //worksheet.Cell("H" + headerRow).Value = DEBS.Translate("Line.BasicDepartureTime");



            int headerRow = 0;
            //bool start = true;
            string lineNumberPrev = null;
            var row = 0;

            int prevSchoolId = 999999999;
            var worksheet = workbook.Worksheets.Add("start");
            lst = lst.OrderBy(c => c.schoolId).ThenBy(c => int.Parse(c.LineNumber)).ThenBy(c => c.Position).ToList();
            for (int i = 0; i < lst.Count; i++)
            {
                var stud = lst[i];
                if (stud.StationName == "סטודיו אנקורי")
                {

                }
                if (prevSchoolId != stud.schoolId)
                {
                    row = 0;
                    worksheet = workbook.Worksheets.Add(stud.name);
                    prevSchoolId = stud.schoolId != null ? stud.schoolId.Value : 1;
                    lineNumberPrev = null;
                    headerRow = row + 2;
                    worksheet.Cell("A" + headerRow).Value = DEBS.Translate("Line.SchoolId");
                    worksheet.Cell("B" + headerRow).Value = stud.schoolId;
                    worksheet.Cell("C" + headerRow).Value = DEBS.Translate("Line.SchoolName");
                    worksheet.Cell("D" + headerRow).Value = stud.name;
                    worksheet.Cell("E" + headerRow).Value = DEBS.Translate("Line.schoolAdress");
                    worksheet.Cell("F" + headerRow).Value = stud.adress;
                    worksheet.Cell("G" + headerRow).Value = DEBS.Translate("Line.schoolCity");
                    worksheet.Cell("H" + headerRow).Value = stud.city;

                    row = headerRow + 1;

                }
                if (!stud.LineNumber.Equals(lineNumberPrev))
                {//main header + data for line

                    headerRow = row + 2;
                    worksheet.Cell("A" + headerRow).Value = DEBS.Translate("BusCompany.Name");
                    worksheet.Cell("B" + headerRow).Value = stud.companyName;
                    worksheet.Cell("C" + headerRow).Value = DEBS.Translate("Line.LineName");
                    worksheet.Cell("D" + headerRow).Value = stud.LineName;
                    worksheet.Cell("E" + headerRow).Value = DEBS.Translate("Line.LineNumber");
                    worksheet.Cell("F" + headerRow).Value = stud.LineNumber;
                    worksheet.Cell("G" + headerRow).Value = DEBS.Translate("Line.Direction");
                    worksheet.Cell("H" + headerRow).Value = stud.Direction == 0 ? DEBS.Translate("General.To") : DEBS.Translate("General.From");
                    worksheet.Cell("I" + headerRow).Value = DEBS.Translate("Line.Duration");
                    worksheet.Cell("J" + headerRow).Value = stud.Duration != null ? stud.Duration.Value.ToString(@"dd\.hh\:mm\:ss") : null;
                    worksheet.Cell("K" + headerRow).Value = DEBS.Translate("Line.totalStudents");
                    worksheet.Cell("L" + headerRow).Value = stud.totalStudents;
                    //design
                    worksheet.Cell("A" + headerRow).Style.Font.Bold = true;
                    worksheet.Cell("C" + headerRow).Style.Font.Bold = true;
                    worksheet.Cell("E" + headerRow).Style.Font.Bold = true;
                    worksheet.Cell("G" + headerRow).Style.Font.Bold = true;
                    worksheet.Cell("I" + headerRow).Style.Font.Bold = true;
                    worksheet.Cell("K" + headerRow).Style.Font.Bold = true;


                    //headers for stations
                    worksheet.Cell("A" + (headerRow + 1)).Value = DEBS.Translate("Stations.Position");
                    worksheet.Cell("B" + (headerRow + 1)).Value = DEBS.Translate("Stations.StationName");
                    worksheet.Cell("C" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalDate");
                    worksheet.Cell("D" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalTimeSun");
                    worksheet.Cell("E" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalTimeMon");
                    worksheet.Cell("F" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalTimeTue");
                    worksheet.Cell("G" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalTimeWed");
                    worksheet.Cell("H" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalTimeThu");
                    worksheet.Cell("I" + (headerRow + 1)).Value = DEBS.Translate("StationsToLines.ArrivalTimeFri");


                    lineNumberPrev = stud.LineNumber;
                    row = headerRow + 1;
                }

                row++;




                //worksheet.Cell(row, "A").Value = stud.companyName;
                //worksheet.Cell(row, "B").Value = stud.LineName;
                //worksheet.Cell(row, "C").Value = lineNumberPrev = stud.LineNumber;
                //worksheet.Cell(row, "D").Value = stud.Direction==0 ? DEBS.Translate("General.To") : DEBS.Translate("General.From");
                //worksheet.Cell(row, "E").Value = stud.Duration != null ? stud.Duration.Value.ToString(@"dd\.hh\:mm\:ss") : null;
                //worksheet.Cell(row, "F").Value = stud.totalStudents;
                worksheet.Cell(row, "A").Value = stud.Position;
                worksheet.Cell(row, "B").Value = stud.StationName;
                worksheet.Cell(row, "C").Value = stud.ArrivalDate;
                worksheet.Cell(row, "D").Value = stud.Sun == true ? DEBS.Translate("general.Yes") : "";
                worksheet.Cell(row, "E").Value = stud.Mon == true ? DEBS.Translate("general.Yes") : "";
                worksheet.Cell(row, "F").Value = stud.Tue == true ? DEBS.Translate("general.Yes") : "";
                worksheet.Cell(row, "G").Value = stud.Wed == true ? DEBS.Translate("general.Yes") : "";
                worksheet.Cell(row, "H").Value = stud.Thu == true ? DEBS.Translate("general.Yes") : "";
                worksheet.Cell(row, "I").Value = stud.Fri == true ? DEBS.Translate("general.Yes") : "";


            }
            workbook.Worksheets.Delete("start");
            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "lines.xlsx"); ;

        }
        public FileContentResult btnExportToExcel4()//special for SMS all parents phones
        {
            string schoolName = tblSystemLogic.getSystemValueByKey("schoolName").value;
            LineLogic.updateAriveAndDepartureTime();

            List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));



            var workbook = new XLWorkbook();
            var worksheet = !string.IsNullOrEmpty(schoolName) ? workbook.Worksheets.Add(schoolName + " " + "Sms List") : workbook.Worksheets.Add("Sms List");
            worksheet.Cell("A1").Value = DEBS.Translate("sms.cellPhone");
            worksheet.Cell("B1").Value = DEBS.Translate("sms.firstName");
            worksheet.Cell("C1").Value = DEBS.Translate("sms.lastName");
            worksheet.Cell("D1").Value = DEBS.Translate("sms.LineNumber");
            worksheet.Cell("E1").Value = DEBS.Translate("sms.LineName");
            worksheet.Cell("F1").Value = DEBS.Translate("sms.StationName");
            worksheet.Cell("G1").Value = DEBS.Translate("sms.AriveTime");
            worksheet.Cell("H1").Value = DEBS.Translate("sms.listName");

            worksheet.Cell("A1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("A").Width = 20;
            // string lineNumberPrev = null;
            var row = 1;

            foreach (var stud in lst)
            {
                if (!string.IsNullOrEmpty(stud.parent1CellPhone))
                {

                    row++;
                    worksheet.Cell(row, "A").Value = "'" + stud.parent1CellPhone;
                    worksheet.Cell(row, "B").Value = stud.studentFirstName;
                    worksheet.Cell(row, "C").Value = stud.studentLastName;
                    worksheet.Cell(row, "D").Value = stud.LineNumber;
                    worksheet.Cell(row, "E").Value = stud.LineName;
                    worksheet.Cell(row, "F").Value = stud.StationName;
                    worksheet.Cell(row, "G").Value = stud.ArrivalDate.HasValue ? "'" + stud.ArrivalDate.Value.ToString(@"hh\:mm") : stud.ArrivalDate.ToString();
                    worksheet.Cell(row, "H").Value = " הורים " + schoolName;
                }
                if (!string.IsNullOrEmpty(stud.parent2CellPhone))
                {

                    row++;
                    worksheet.Cell(row, "A").Value = "'" + stud.parent2CellPhone;
                    worksheet.Cell(row, "B").Value = stud.studentFirstName;
                    worksheet.Cell(row, "C").Value = stud.studentLastName;
                    worksheet.Cell(row, "D").Value = stud.LineNumber;
                    worksheet.Cell(row, "E").Value = stud.LineName;
                    worksheet.Cell(row, "F").Value = stud.StationName;
                    worksheet.Cell(row, "G").Value = stud.ArrivalDate.HasValue ? "'" + stud.ArrivalDate.Value.ToString(@"hh\:mm") : stud.ArrivalDate.ToString();
                    worksheet.Cell(row, "H").Value = " הורים " + schoolName;
                }
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }


        public JsonResult selectLines(string[] val)
        {
            Session["lineList"] = null;
            Session["lineList"] = val;

            return Json(JsonRequestBehavior.AllowGet);
        }
        public FileContentResult btnExportToExcel5()
        {



            LineLogic.updateAriveAndDepartureTime();

            List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();
            Response.Cookies.Add(new HttpCookie("fileDownload", "true"));




            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Students");
            worksheet.Cell("A1").Value = DEBS.Translate("General.siduri");
            worksheet.Cell("B1").Value = DEBS.Translate("Line.LineName");
            worksheet.Cell("C1").Value = DEBS.Translate("Line.LineNumber");
            worksheet.Cell("D1").Value = DEBS.Translate("Stations.StationName");
            worksheet.Cell("E1").Value = DEBS.Translate("StationsToLines.ArrivalDate");
            worksheet.Cell("F1").Value = DEBS.Translate("tblStudent.firstName");
            worksheet.Cell("G1").Value = DEBS.Translate("tblStudent.lastName");
            worksheet.Cell("H1").Value = DEBS.Translate("tblStudent.class");
            worksheet.Cell("I1").Value = DEBS.Translate("tblStudent.CellPhone");
            worksheet.Cell("J1").Value = DEBS.Translate("tblStudent.Email");
            worksheet.Cell("K1").Value = DEBS.Translate("excel.parent1Email");
            worksheet.Cell("L1").Value = DEBS.Translate("excel.parent1CellPhone");
            worksheet.Cell("M1").Value = DEBS.Translate("excel.parent2Email");
            worksheet.Cell("N1").Value = DEBS.Translate("excel.parent2CellPhone");
            worksheet.Cell("O1").Value = DEBS.Translate("tblStudent.school");
            string lineNumberPrev = null;
            var row = 2;
            if (Session["lineList"] != null)
            {
                string[] val = Session["lineList"] as string[];
                //   lst.Where(x => val.Contains(x.LineNumber));
                lst = (from o in lst
                       where val.Contains(o.LineNumber)
                       select o).ToList();
            }

            lst = lst.OrderBy(c => c.LineNumber).ThenBy(c => c.Position).ToList();

            foreach (var stud in lst)
            {
                if (stud.lineIsActive == true && stud.StationName != null && stud.studentActive == true)
                {
                    row++;
                    worksheet.Cell(row, "A").Value = row - 1;
                    worksheet.Cell(row, "B").Value = stud.LineName;
                    worksheet.Cell(row, "C").Value = stud.LineNumber;
                    worksheet.Cell(row, "D").Value = stud.StationName;
                    worksheet.Cell(row, "E").Value = stud.ArrivalDate;//time for bus in student station
                    worksheet.Cell(row, "F").Value = stud.studentFirstName;
                    worksheet.Cell(row, "G").Value = stud.studentLastName;
                    worksheet.Cell(row, "H").Value = stud.@class;
                    worksheet.Cell(row, "I").Value = "'" + stud.studentCellPhone;
                    worksheet.Cell(row, "J").Value = stud.studentEmail;
                    worksheet.Cell(row, "K").Value = stud.parent1Email;
                    worksheet.Cell(row, "L").Value = "'" + stud.parent1CellPhone;
                    worksheet.Cell(row, "M").Value = stud.parent2Email;
                    worksheet.Cell(row, "N").Value = "'" + stud.parent2CellPhone;
                    worksheet.Cell(row, "O").Value = stud.schoolName;


                }
            }

            var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;
            var sr = new BinaryReader(ms);
            return File(sr.ReadBytes((int)ms.Length), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Students.xlsx"); ;

        }

        //public void sendEmail()
        //{

        //    LineLogic.updateAriveAndDepartureTime();//update table Lines- insert data to BasicArriveTime & BasicDepartureTime

        //    List<ViewAllStudentFamilyLinesStation> lst = ViewAllStudentsLogic.GetAllStudents();


        //    string EmailAdress = tblSystemLogic.getSystemValueByKey("EmailAdress").strValue;
        //    string Password = tblSystemLogic.getSystemValueByKey("Password").strValue;
        //    string mailServer = tblSystemLogic.getSystemValueByKey("mailServer").strValue;


        //    foreach (var item in lst)
        //    {
        //        System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
        //           new System.Net.Mail.MailAddress("harshamaHasaot@gmail.com", "Web Registration"),
        //              new System.Net.Mail.MailAddress(item.parent1Email));
        //        m.Subject = "נושא";
        //        m.Body = string.Format("Dear {0}" + "44444" + "{1}<BR/>" + " מה נשמע" + "שלום רב", "יופי 1", "   יופי 22");

        //        m.IsBodyHtml = true;

        //        System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(mailServer);
        //        smtp.Credentials = new System.Net.NetworkCredential(EmailAdress, Password);
        //        smtp.EnableSsl = true;
        //        try
        //        {
        //            smtp.SendMailAsync(m);
        //        }
        //        catch (Exception ex)
        //        {

        //        }
        //    }




        //}

        public async Task<ActionResult> SendEmail()//important - change the support EMAILS before sending
        {
            tblSystem Z = tblSystemLogic.getSystemValueByKey("sendEmail1Now");
            if (Z.strValue != "yes")
            {
                TempData["manageMessage"] = "you must change sendEmail1Now to yes at the system ";
                return RedirectToAction("index", "Manage");
            }
            else
            {
                Z.strKey = "sendEmail1Now";
                Z.strValue = "No";
                Z.key = "sendEmail1Now";
                Z.value = "No";
                tblSystemLogic.updateSystemValueByKey(Z);
            }

            LineLogic.updateAriveAndDepartureTime();//update table Lines- insert data to BasicArriveTime & BasicDepartureTime
            tblEmailSent c = new tblEmailSent();
            List<viewStudentWithStationForEmail> lst = ViewAllStudentsLogic.GetAllStudentForEmail();
            string from;
            string to;

            string EmailAdress = tblSystemLogic.getSystemValueByKey("EmailAdress").strValue;
            string Password = tblSystemLogic.getSystemValueByKey("Password").strValue;
            string mailServer = tblSystemLogic.getSystemValueByKey("mailServer").strValue;
            Random random = new Random();
            try
            {
                from = tblSystemLogic.getSystemValueByKey("startRandomSecond").value;
                to = tblSystemLogic.getSystemValueByKey("endRandomSecond").value;
            }
            catch
            {
                from = "500";
                to = "1000";
            }
            int direction;
            string prevStudentId = null;
            for (int i = 0; i < lst.Count; i++)
            {
                var item = lst[i];
                direction = 1;//0- to school    1 = from school
                //foreach (var item in lst)
                //{

                if (item.lineIsActive == true && item.StationName != null && item.parent1Email != null && item.Linedirection == direction && item.studentId != prevStudentId )
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("<body style=\"direction:rtl;font-family:Arial\">");
                    sb.Append("<h2 class='headings'> שלום,</h2>");
                    sb.Append(" <br>");
                    sb.Append("רצ\"ב השיבוץ המעודכן להסעות החוזרות מבית הספר.");
                    sb.Append(" <br>");
                    sb.Append(" <br>");
                    sb.Append("  השיבוץ העדכני של <%studentName%> הוא לקו <%lineNumber%>.");
                    sb.Append(" <br>");
                    sb.Append("  בתחנת  <%stationName%>. שעת החזרה המשוערת  <%pickUpTime%>  ");
                    sb.Append(" <br>");
                    sb.Append("מספרי הקווים בחזרה מתחילים מ-100 - 101 ואילך... לצרכי המערכת. כל הקווים יוצאים מבית הספר בשעה 14:40. ");
                    sb.Append(" <br>");
 
                    sb.Append("צוות הסעות תיכונט");
                    sb.Append(" <br>");
                    sb.Append("hasaot.tichonet@gmail.com");
                    //   sb.Append("support@hippo-campus.co.il");
                    sb.Append("</body>");

                    sb.Replace("<%studentName%>", item.studentFirstName + " " + item.studentLastName);
                    sb.Replace("<%lineNumber%>", item.LineNumber);
                    sb.Replace("<%stationName%>", item.StationName);
                    sb.Replace("<%pickUpTime%>", item.ArrivalDate != null ? item.ArrivalDate.ToString(@"hh\:mm") : item.ArrivalDate.ToString());


                    System.Net.Mail.MailMessage m = new System.Net.Mail.MailMessage(
                    new System.Net.Mail.MailAddress("harshamaHasaot@gmail.com", "Web Registration"),
                        //  new System.Net.Mail.MailAddress("hasaot.tichonet@gmail.com", "Web Registration"),
              new System.Net.Mail.MailAddress(item.parent1Email));
                //   new System.Net.Mail.MailAddress("mictavim@gmail.com")); 
                   // m.To.Add("hasaot.tichonet@gmail.com");
                    try
                    {
                        if (item.parent2Email != null)
                        {
                            m.To.Add(item.parent2Email);
                            c.sentToParent1 = true;
                        }
                        if (item.studentEmail != null)
                        {
                            m.To.Add(item.studentEmail);
                        }
                    }
                    catch { }
                    m.Subject = "שיבוץ להסעות חזרה מבית הספר   תשע\"ז ";
                    m.Body = sb.ToString();
                    m.IsBodyHtml = true;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(mailServer);
                    smtp.Credentials = new System.Net.NetworkCredential(EmailAdress, Password);
                    smtp.EnableSsl = true;
                    try
                    {
                        c.sentToParent1 = true;
                        c.statusParent1 = true;
                        c.sentToParent2 = true;
                        c.statusParent2 = true;



                        MailMessage mail = new MailMessage("harshamaHasaot@gmail.com", m.To.ToString(), m.Subject, m.Body);
                        mail.IsBodyHtml = true;
                        var client = new SmtpClient("smtp.gmail.com", 587)
                        {
                            Credentials = new NetworkCredential("harshamaHasaot@gmail.com", "zaqzaq8*"),
                            EnableSsl = true
                        };
                        client.Send(mail);
                        //  client.Send("mictavim@gmail.com", "giladdlv@gmail.com", "test", "testbody");




                        //   await smtp.SendMailAsync(m);
                    }
                    catch (Exception ex)
                    {
                        c.statusParent1 = false;
                        c.statusParent2 = false;
                    }



                    try
                    {
                        c.familyId = item.familyId;
                        c.itemInList = i;
                        c.dateSent = DateTime.Now;
                        c.parent1Email = item.parent1Email;
                        c.lineNumber = int.Parse(item.LineNumber);
                        c.parent2Email = item.parent2Email;
                        c.schoolName = item.schoolName;
                        c.studentName = item.studentLastName + " " + item.studentFirstName;
                        c.studentId = item.studentId;
                        c.message = string.Format(sb.ToString());
                        c.comment = "ticonet 2 message about lines and schedule for 2 parents)";
                    }
                    catch
                    {
                    }
                    try
                    {
                        ManageLogic.addEmailSent(c);
                    }
                    catch
                    {

                    }
                    prevStudentId = item.studentId;
                    int randomNumber = random.Next(int.Parse(from), int.Parse(to));
                    Thread.Sleep(randomNumber);
                }
                //else
                //{
                //    try
                //    {
                //        c.familyId = item.familyId;
                //        c.itemInList = i;
                //        c.dateSent = DateTime.Now;
                //        c.parent1Email = item.parent1Email;
                //        c.lineNumber = int.Parse(item.LineNumber);
                //        c.parent2Email = item.parent2Email;
                //        c.schoolName = item.schoolName;
                //        c.studentName = item.studentLastName + " " + item.studentFirstName;
                //        c.studentId = item.studentId;
                //        c.message = "message not sent";
                //        c.comment = "ticonet 2 message about lines and schedule for 2 parents)";
                //        c.sentToParent1 = false;
                //        c.statusParent1 = false;
                //        c.sentToParent2 = false;
                //        c.statusParent2 = false;
                //        c.statusParent1 = false;
                //        c.statusParent2 = false;
                //        ManageLogic.addEmailSent(c);
                //    }
                //    catch
                //    {

                //    }
                //}
            }
            TempData["manageMessage"] = "Emails Sent OK ";
            return RedirectToAction("index", "Manage");
        }
    }
}





