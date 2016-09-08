﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Mvc;
using Business_Logic;
using Business_Logic.Dtos;
using ClosedXML.Excel;
using log4net;
using ticonet.Models;
using Business_Logic.Enums;
using Business_Logic.Helpers;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class LinesApiController : ApiController {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LinesApiController));

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetLines(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "") {
            var lines = new List<GridLineModel>();
            var totalRecords = 0;
            using (var logic = new LineLogic()) {
                lines = logic.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(z => new GridLineModel(z)).ToList();
                totalRecords = logic.Lines.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new {
                    total = (totalRecords + rows - 1) / rows,
                    page,
                    records = totalRecords
                    ,
                    rows = lines
                });
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetTotal(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "") {
            TotalDto total;
            using (var logic = new LineLogic()) {
                total = logic.GetTotal(_search, rows, page, sidx, sord, filters);
            }
            return new JsonResult { Data = total };
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult EditLine(GridLineModel model) {
            int LineIdToAutoCorrect = -1;
            using (var logic = new LineLogic()) {
                switch ((GridOperation)Enum.Parse(typeof(GridOperation), model.Oper, true)) {
                    case GridOperation.edit:
                        var existingLine = logic.GetLine(model.Id);
                        if (existingLine != null) {
                            model.UpdateDbModel(existingLine);
                            logic.SaveChanges();
                            using (var busesToLinesLogic = new BusToLineLogic()) {
                                busesToLinesLogic.UpdateBusToLine(model.Id, model.Bus);
                            }
                            LineIdToAutoCorrect = existingLine.Id;
                        }
                        break;
                    case GridOperation.del:
                        logic.DeleteLine(model.Id);
                        break;
                }
            }
            if (LineIdToAutoCorrect != -1) {
                var weekStart = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                using (var l = new tblScheduleLogic()) {
                    l.AutoCorrectLineSchedules(LineIdToAutoCorrect, weekStart, weekStart.AddDays(7));
                }
            }
            return new JsonResult { Data = true };
        }

        public HttpResponseMessage GetExcel(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "") {
            var lines = new GridLineModel[] { };
            var totalRecords = 0;
            TotalDto totalStat;
            using (var logic = new LineLogic()) {
                totalRecords = logic.Lines.Count();
                lines = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new GridLineModel(z)).ToArray();
                totalStat = logic.GetTotal(_search, totalRecords, 1, sidx, sord, filters);
            }

            string Name = "Lines";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(Name + " Sheet");
            worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            worksheet.Cell(1, 1).Value = DictExpressionBuilderSystem.Translate("Line.LineName");
            worksheet.Cell(1, 2).Value = DictExpressionBuilderSystem.Translate("Line.LineNumber");
            worksheet.Cell(1, 3).Value = DictExpressionBuilderSystem.Translate("Line.Direction");
            worksheet.Cell(1, 4).Value = DictExpressionBuilderSystem.Translate("Line.IsActive");
            worksheet.Cell(1, 5).Value = DictExpressionBuilderSystem.Translate("Line.totalStudents");
            worksheet.Cell(1, 6).Value = DictExpressionBuilderSystem.Translate("Line.Duration");
            worksheet.Cell(1, 7).Value = DictExpressionBuilderSystem.Translate("Line.Sun");
            worksheet.Cell(1, 8).Value = DictExpressionBuilderSystem.Translate("Line.Mon");
            worksheet.Cell(1, 9).Value = DictExpressionBuilderSystem.Translate("Line.Tue");
            worksheet.Cell(1, 10).Value = DictExpressionBuilderSystem.Translate("Line.Wed");
            worksheet.Cell(1, 11).Value = DictExpressionBuilderSystem.Translate("Line.Thu");
            worksheet.Cell(1, 12).Value = DictExpressionBuilderSystem.Translate("Line.Fri");
            worksheet.Cell(1, 13).Value = DictExpressionBuilderSystem.Translate("Line.Sut");
            worksheet.Cell(1, 14).Value = DictExpressionBuilderSystem.Translate("Bus.Id");
            worksheet.Cell(1, 14).Value = DictExpressionBuilderSystem.Translate("Bus.BusId");
            worksheet.Cell(1, 15).Value = DictExpressionBuilderSystem.Translate("Bus.PlateNumber");
            worksheet.Cell(1, 16).Value = DictExpressionBuilderSystem.Translate("BusCompany.Name");
            worksheet.Cell(1, 17).Value = DictExpressionBuilderSystem.Translate("Bus.seats");
            worksheet.Cell(1, 18).Value = DictExpressionBuilderSystem.Translate("Bus.price");

            for (int i = 0; i < lines.Length; i++) {
                var row = 2 + i;
                worksheet.Cell(row, 1).SetValue<string>(lines[i].LineName);
                worksheet.Cell(row, 2).SetValue<string>(lines[i].LineNumber);
                worksheet.Cell(row, 3).SetValue<string>(DictExpressionBuilderSystem.Translate("General." + lines[i].Direction));
                worksheet.Cell(row, 4).SetValue<bool>(lines[i].IsActive);
                worksheet.Cell(row, 5).SetValue<int>(lines[i].totalStudents);
                worksheet.Cell(row, 6).SetValue<string>(Convert.ToString(lines[i].Duration));
                worksheet.Cell(row, 7).SetValue<bool>(lines[i].Sun);
                worksheet.Cell(row, 8).SetValue<bool>(lines[i].Mon);
                worksheet.Cell(row, 9).SetValue<bool>(lines[i].Tue);
                worksheet.Cell(row, 10).SetValue<bool>(lines[i].Wed);
                worksheet.Cell(row, 11).SetValue<bool>(lines[i].Thu);
                worksheet.Cell(row, 12).SetValue<bool>(lines[i].Fri);
                worksheet.Cell(row, 13).SetValue<bool>(lines[i].Sut);
                worksheet.Cell(row, 14).SetValue<int>(lines[i].Bus);
                worksheet.Cell(row, 15).SetValue<string>(lines[i].BusId);
                worksheet.Cell(row, 15).SetValue<string>(lines[i].PlateNumber);
                worksheet.Cell(row, 16).SetValue<string>(lines[i].BusCompanyName);
                worksheet.Cell(row, 17).SetValue<int?>(lines[i].seats);
                worksheet.Cell(row, 18).SetValue<double?>(lines[i].price);
            }

            var totalRowIndex = lines.Length + 2;
            worksheet.Cell(totalRowIndex, 1).Value = DictExpressionBuilderSystem.Translate("grid.Total");
            worksheet.Cell(totalRowIndex, 5).SetValue(totalStat.Students);

            for (int i = 0; i < 7; i++) {
                worksheet.Cell(totalRowIndex, 7 + i).SetValue(totalStat.WeekDayPrices[i]);
            }

            worksheet.Cell(totalRowIndex, 17).SetValue(totalStat.Seats);
            worksheet.Cell(totalRowIndex, 18).SetValue(totalStat.Price);

            worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
            worksheet.Columns().AdjustToContents();

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var memoryStream = new MemoryStream()) {
                workbook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                result.Content = new ByteArrayContent(memoryStream.ToArray());
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment") {
                    FileName = Name + ".xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return result;
        }

        public GridLineModel[] GetPrint(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "") {
            var lines = new GridLineModel[] { };
            var totalRecords = 0;
            using (var logic = new LineLogic()) {
                totalRecords = logic.Lines.Count();
                lines = logic.GetPaged(_search, totalRecords, page, sidx, sord, filters)
                    .Select(z => new GridLineModel(z)).ToArray();
            }

            return lines;
        }

        public JsonResult GetAvailableBuses(int lineId) {
            var buses = new List<SelectItemModel>();
            buses.Add(new SelectItemModel { Value = "0", Text = string.Empty, Title = string.Empty });
            using (var logic = new LineLogic()) {
                buses.AddRange(logic.GetAvailableBuses(lineId)
                    .ToList()
                    .Select(z => new SelectItemModel {
                        Value = z.Id.ToString(),
                        Text = DescriptionHelper.GetBusDescription(z),
                        Title = DescriptionHelper.GetBusDescription(z)
                    }));
            }

            return new JsonResult { Data = buses };
        }

        public JsonResult GetCompaniesFilter() {
            var companies = new List<SelectItemModel>();
            companies.Add(new SelectItemModel { Value = "", Text = DictExpressionBuilderSystem.Translate("Search.All") });
            using (var logic = new LineLogic()) {
                var getCompanies = logic.GetCompaniesFilter();
                companies.AddRange(getCompanies
                    .Where(w => w != null)
                    .Select(z => new SelectItemModel {
                        Value = z.pk.ToString(),
                        Text = z.companyName
                    }).ToList());
            }

            return new JsonResult { Data = companies };
        }

        [System.Web.Mvc.HttpGet]
        public JsonResult GetCurrentStartOfWeek() {
            var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            return new JsonResult { Data = startOfWeek };
        }

        [System.Web.Mvc.HttpPost]
        public JsonResult AutoCorrectLineWeekSchedules(int lineID, DateTime weekStart) {
            using (var l = new tblScheduleLogic()) {
                l.AutoCorrectLineSchedules(lineID,weekStart,weekStart.AddDays(7));
            }
            return new JsonResult { Data = true };
        }
    }
}