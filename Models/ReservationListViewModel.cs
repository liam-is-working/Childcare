using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class ReservationListViewModel{
        public List<ReservationCard> ReservationCards{get;set;}
        public Filter Filter{get;set;}
    }

    public class ReservationCard : IComparable<ReservationCard>{
        public int ReservationID{get;set;}
        public int CustomerID{get;set;}
        public int ServiceID{get;set;}
        public string ServiceName{get;set;}
        public System.DateTime ReservationDate{get;set;}
        public int SlotNumber{get;set;}
        public DateTime CheckinTime{get;set;}
        public bool IsInThePast{get => CheckinTime<DateTime.Now;}

        public int CompareTo(ReservationCard other)
        {
            return (this.CheckinTime < other.CheckinTime)?1:-1;
        }
    }
    public class Filter{
        public bool? ShowOldReservation{get;set;}
        public int? ReservationID{get;set;}
        public int? ServiceID{get;set;}
        public int? PatientID{get;set;}
        [DataType(DataType.DateTime)]
        public System.DateTime? ReservationDate {get;set;}
        public int? SlotNumber{get;set;}
        public int? CustomerID{get;set;}
        public int? SpecialtyID{get;set;}
        [DataType(DataType.DateTime)]
        public System.DateTime? FromTime{get;set;}
        [DataType(DataType.DateTime)]
        public System.DateTime? ToTime{get;set;}

    }
}