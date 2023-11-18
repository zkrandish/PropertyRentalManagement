using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace PropertyRentalManagementWebSite.Models
{
    public class AppointmentMetadata
    {
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "You need to specify an appointment date.")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Required(ErrorMessage = "You need to specify a start time for the appointment.")]
        public TimeSpan from { get; set; }

        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH:mm}")]
        [Required(ErrorMessage = "You need to specify an end time for the appointment.")]
        public TimeSpan to { get; set; }

        [Display(Name = "Purpose of Appointment")]
        [Required(ErrorMessage = "You need to provide the purpose of the appointment.")]
        [StringLength(255, ErrorMessage = "The purpose of the appointment is too long.")]
        public string Purpose { get; set; }

    }
}