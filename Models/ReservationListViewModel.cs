using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;

namespace Childcare.Models
{
    public class ReservationListViewModel{
        public List<ReservationCard> ReservationCards{get;set;}
        public Filter Filter{get;set;}
    }

    public class ReservationCard{
        public int ReservationID{get;set;}
        public int CustomerID{get;set;}
        public int ServiceID{get;set;}
        public string ServiceName{get;set;}
        public System.DateTime ReservationDate{get;set;}
        public int SlotNumber{get;set;}
        public DateTime CheckinTime{get;set;}
    }
    public class Filter{
        
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