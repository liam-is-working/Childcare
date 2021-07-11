using System;
using System.Collections.Generic;

namespace Childcare.Models
{
    public class ShowAvailableSlotPartialViewModel{
        public IDictionary<int,Slot> Slots {get;set;}
    }

    public struct Slot{
        public DateTime StartTime{get; set;}
        public DateTime EndTime{get; set;}
    }
}