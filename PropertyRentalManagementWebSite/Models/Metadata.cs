using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

    //[MetadataType(typeof(UserMetadata))]
    //public partial class User
    //{
    //    // The actual User model properties will be in another part of this partial class
    //}

    //public class UserMetadata
    //{
    //    [Required(ErrorMessage = "First name is required.")]
    //    [Display(Name = "First Name")]
    //    [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
    //    public string FirstName { get; set; }

    //    [Required(ErrorMessage = "Last name is required.")]
    //    [Display(Name = "Last Name")]
    //    [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
    //    public string LastName { get; set; }

    //    [Required(ErrorMessage = "Username is required.")]
    //    [Display(Name = "Username")]
    //    [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
    //    public string UserName { get; set; }

    //    [Required(ErrorMessage = "Email is required.")]
    //    [EmailAddress(ErrorMessage = "Invalid Email Address")]
    //    public string Email { get; set; }

    //    [Required(ErrorMessage = "Password is required.")]
    //    [DataType(DataType.Password)]
    //    public string Password { get; set; }


    //}

    [MetadataType(typeof(ApartmentMetadata))]
    public partial class Apartment
    {
        // The actual Apartment model properties will be in another part of this partial class
    }

    public class ApartmentMetadata
    {
        //[Required(ErrorMessage = "Apartment name is required.")]
        //[Display(Name = "Apartment Name")]
        //[StringLength(100, ErrorMessage = "Apartment name cannot exceed 100 characters.")]
        //public string Name { get; set; }

        [Display(Name = "Type")]
        [DataType(DataType.MultilineText)]
        public string Type { get; set; }
        
        [Required(ErrorMessage = "Price is required.")]
        [DataType(DataType.Currency)]
        [Range(typeof(decimal), "0.01", "1000000.00", ErrorMessage = "Price must be between 0.01 and 1,000,000.00")]
        public decimal Price { get; set; }
    }

    [MetadataType(typeof(BuildingMetadata))]
    public partial class Building
    {
        // The actual Building model properties will be in another part of this partial class
    }

    public class BuildingMetadata
    {
        [Required(ErrorMessage = "Province is required.")]
        [Display(Name = "Province")]
        [StringLength(10, ErrorMessage = "Building Province cannot exceed 10 characters.")]
        public string Province { get; set; }

        [Display(Name = "City")]
        [StringLength(10, ErrorMessage = "Building city  cannot exceed 10 characters.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Postal code is required.")]
        [DataType(DataType.PostalCode)]
        [StringLength(10, ErrorMessage = "Postal Code cannot exceed 10 characters.")]
        public string PostalCode { get; set; }
    }
    //[MetadataType(typeof(MessageMetadata))]
    //public partial class Message
    //{
    //    // Assuming your Message model is defined here.
    //    // This class will be linked with the metadata class below.
    //}

    //public class MessageMetadata
    //{
       

    //   // [Required]
    //    [ForeignKey("User")] // Assuming a User table with UserId as the primary key.
    //    [Display(Name = "Receiver ID")]
    //    public int Receiver { get; set; }

    //    //[Required]
    //    [ForeignKey("User")] // Assuming a User table with UserId as the primary key.
    //    [Display(Name = "Sender ID")]
    //    public int Sender { get; set; }

    //    [Required(ErrorMessage = "Message content is required.")]
    //    [DataType(DataType.MultilineText)]
    //    [Display(Name = "Message Content")]
    //    public string Message1 { get; set; }

    //    //[Required]
    //    [DataType(DataType.DateTime)]
    //    [Display(Name = "Send Date")]
    //    public DateTime SendDate { get; set; }

    //    //[Required]
    //    //[ForeignKey("Status")] // Assuming a Status table with StatusId as the primary key.
    //    //[Display(Name = "Status ID")]
    //    //public int StatusId { get; set; }
    //}
}
