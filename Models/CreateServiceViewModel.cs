using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Childcare.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Childcare.Models
{
    public class CreateServiceViewModel{

        //Specialty for user to choose from
        public IList<Specialty> Specialties {get;set;}

        //For user to fill
        [Required]
        public string ServiceName { get; set; }
        [Required]
        public int? SpecialtyID { get; set; }
        //[Required]
        public string Thumbnail { get; set; }
        //[Required]
        public string Description { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        public float Price { get; set; }
        [Required]
        [DataType(DataType.Time)]
        public DateTime StartTime {get;set;}
        [Required]
        [DataType(DataType.Time)]
        [TimeCustom("StartTime")]
        public DateTime EndTime {get;set;}
        [Required]
        [Display(Name = "ServiceTime(in minutes)")]
        [Range(0,24*60)]
        //in minute
        public int ServiceTime{get;set;}

    }

    public class TimeCustomAttribute : ValidationAttribute, IClientModelValidator{
        public readonly string _earlierPropName;
        public TimeCustomAttribute(string earlierPropertyName){
            _earlierPropName = earlierPropertyName;
        }

        public void AddValidation(ClientModelValidationContext context)
        {
            var error = FormatErrorMessage(context.ModelMetadata.GetDisplayName());
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-error", error);
        }

        public string GetErrorMessage() => "EndTime must be after StartTime";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var currentValue = (DateTime)value;
            var earlierProp = validationContext.ObjectType.GetProperty(_earlierPropName);

            if(earlierProp==null)
                return null;
            
            var earlierValue = (DateTime)earlierProp.GetValue(validationContext.ObjectInstance);
            //compare
            if(currentValue < earlierValue)
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
            
        }
    }
}