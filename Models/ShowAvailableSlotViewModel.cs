using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Childcare.Models
{
    public class ShowAvailableSlotViewModel{
        public ShowAvailableSlotViewModel(){}
        public IDictionary<int,Slot> Slots {get;set;}
        [Required]
        public int ServiceId{get;set;}
        [Required]
        public DateTime ChosenDate{get;set;}
    }

    public struct Slot{
        public DateTime StartTime{get; set;}
        public DateTime EndTime{get; set;}
    }
}