﻿using System;
using System.Collections.Generic;
using System.Linq;
using Business_Logic;
using Business_Logic.Enums;
using Business_Logic.Helpers;

namespace ticonet.Models
{
    public class GridLineBusModel
    {

        public GridLineBusModel()
        {
        }

        public GridLineBusModel(Line data)
        {
            var bus = data.BusesToLines.Select(x => x.Bus).FirstOrDefault();
            Id = data.Id;
            LineName = data.LineName;
            LineNumber = data.LineNumber;
            Direction = (LineDirection)data.Direction;
            IsActive = data.IsActive;
            totalStudents = data.totalStudents ?? 0;
            Duration = data.Duration;
            Bus = bus != null ? bus.Id : (int)0;
            BusDescription = bus != null ? DescriptionHelper.GetBusDescription(bus) : string.Empty;
            BusId = bus != null ? bus.BusId : string.Empty;
            PlateNumber = bus != null ? bus.PlateNumber : string.Empty;
            BusCompanyName = bus != null ? (bus.BusCompany != null ? bus.BusCompany.companyName : string.Empty) : string.Empty;
            seats = bus != null ? bus.seats : (int?)null;
            price = bus != null ? bus.price : (double?)null;
        }

        public int Id { get; set; }

        public string LineName { get; set; }

        public string LineNumber { get; set; }

        public LineDirection Direction { get; set; }

        public bool IsActive { get; set; }

        public int totalStudents { get; set; }

        public TimeSpan? Duration { get; set; }

        public int Bus { get; set; }
        public string BusDescription { get; set; }

        public string BusId { get; set; }

        public string PlateNumber { get; set; }

        public string BusCompanyName { get; set; }

        public int? seats { get; set; }

        public double? price { get; set; }

        public string Oper { get; set; }
    }
}