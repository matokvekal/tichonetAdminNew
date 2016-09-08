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
using Business_Logic.Enums;
using ClosedXML.Excel;
using log4net;
using ticonet.Models;

namespace ticonet.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BusesApiController : ApiController
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(BusesApiController));

        [System.Web.Mvc.HttpGet]
        public HttpResponseMessage GetBuses(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var buses = new List<BusModel>();
            var totalRecords = 0;
            using (var logic = new tblBusLogic())
            {
                buses = logic.GetPaged(_search, rows, page, sidx, sord, filters)
                    .Select(z => new BusModel(z)).ToList();
                totalRecords = logic.Buses.Count();
            }
            return Request.CreateResponse(
                HttpStatusCode.OK,
                new
                {
                    total = (totalRecords + rows - 1) / rows,
                    page,
                    records = totalRecords,
                    rows = buses
                });
        }
        
        [System.Web.Mvc.HttpPost]
        public JsonResult EditBus(BusModel model)
        {
            using (var logic = new tblBusLogic())
            {
                switch ((GridOperation)Enum.Parse(typeof(GridOperation), model.Oper, true))
                {
                    case GridOperation.add:
                        logic.SaveBus(model.ToDbModel());
                        break;
                    case GridOperation.edit:
                        logic.Update(model.ToDbModel());
                        break;
                    case GridOperation.del:
                        logic.DeleteBus(model.Id);
                        break;
                }
            }
            return new JsonResult {Data = true};
        }

        public HttpResponseMessage GetExcel(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var buses = new BusModel[] {};
            var totalRecords = 0;
            using (var logic = new tblBusLogic())
            {
                totalRecords = logic.Buses.Count();
                buses = logic.GetPaged(_search, totalRecords, 1, sidx, sord, filters)
                    .Select(z => new BusModel(z)).ToArray();
            }

            string Name = "Buses";
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(Name + " Sheet");
            worksheet.Outline.SummaryVLocation = XLOutlineSummaryVLocation.Top;

            worksheet.Cell(1, 1).Value = DictExpressionBuilderSystem.Translate("Bus.BusId");
            worksheet.Cell(1, 2).Value = DictExpressionBuilderSystem.Translate("Bus.PlateNumber");
            worksheet.Cell(1, 3).Value = DictExpressionBuilderSystem.Translate("BusCompany.Name");
            worksheet.Cell(1, 4).Value = DictExpressionBuilderSystem.Translate("Bus.seats");
            worksheet.Cell(1, 5).Value = DictExpressionBuilderSystem.Translate("Bus.price");
            worksheet.Cell(1, 6).Value = DictExpressionBuilderSystem.Translate("Bus.munifacturedate");
            worksheet.Cell(1, 7).Value = DictExpressionBuilderSystem.Translate("Bus.LicensingDueDate");
            worksheet.Cell(1, 8).Value = DictExpressionBuilderSystem.Translate("Bus.insuranceDueDate");
            worksheet.Cell(1, 9).Value = DictExpressionBuilderSystem.Translate("Bus.winterLicenseDueDate");
            worksheet.Cell(1, 10).Value = DictExpressionBuilderSystem.Translate("Bus.brakeTesDueDate");

            for (int i = 0; i < buses.Length; i++)
            {
                var row = 2 + i;
                worksheet.Cell(row, 1).SetValue<string>(buses[i].BusId);
                worksheet.Cell(row, 2).SetValue<string>(buses[i].PlateNumber);
                worksheet.Cell(row, 3).SetValue<string>(buses[i].OwnerDescription);
                worksheet.Cell(row, 4).SetValue<int?>(buses[i].seats);
                worksheet.Cell(row, 5).SetValue<double?>(buses[i].price);
                worksheet.Cell(row, 6).SetValue<string>(buses[i].munifacturedate);
                worksheet.Cell(row, 7).SetValue<string>(buses[i].LicensingDueDate);
                worksheet.Cell(row, 8).SetValue<string>(buses[i].insuranceDueDate);
                worksheet.Cell(row, 9).SetValue<string>(buses[i].winterLicenseDueDate);
                worksheet.Cell(row, 10).SetValue<string>(buses[i].brakeTesDueDate);
            }

            worksheet.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            worksheet.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.None;
            worksheet.Columns().AdjustToContents();

            var result = new HttpResponseMessage(HttpStatusCode.OK);
            using (var memoryStream = new MemoryStream())
            {
                workbook.SaveAs(memoryStream);
                memoryStream.Position = 0;
                result.Content = new ByteArrayContent(memoryStream.ToArray());
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = Name + ".xlsx"
                };
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
            return result;
        }


        public BusModel[] GetPrint(bool _search, string nd, int rows, int page, string sidx, string sord, string filters = "")
        {
            var buses = new BusModel[] { };
            var totalRecords = 0;
            using (var logic = new tblBusLogic())
            {
                totalRecords = logic.Buses.Count();
                buses = logic.GetPaged(_search, totalRecords, page, sidx, sord, filters)
                    .Select(z => new BusModel(z)).ToArray();
            }

            return buses;
        }

        public JsonResult GetBusCompanies()
        {
            var busCompanies = new List<SelectItemModel>();
            busCompanies.Add(new SelectItemModel { Value = "0", Text = string.Empty, Title = string.Empty });
            using (var logic = new tblBusCompanyLogic())
            {
                busCompanies.AddRange(logic.GetBusCompanies()
                    .Select(z => new SelectItemModel
                    {
                        Value = z.pk.ToString(),
                        Text = z.companyName,
                        Title = string.Format("{0} ({1} - {2})", z.companyName, z.manager, z.tel)
                    }).ToList());
            }

            return new JsonResult { Data = busCompanies };
        }

    }
}