using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Seminar
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Seminar Topic")]
        public string topic { get; set; }

        [Required]
        [Display(Name = "Seminar Platform")]
        [MaxLength(100)]
        public string platform { get; set; }

        [Required]
        [MaxLength(100)]
        [SeminarLink()]
        [Display(Name = "Seminar Link")]
        public string link { get; set; }

        [MaxLength(100)]
        [Display(Name = "Link ID")]
        public string linkId { get; set; }

        [MaxLength(100)]
        [Display(Name = "Seminar Password")]
        public string password { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Seminar Type")]
        public string type { get; set; }

        [Required]
        [Display(Name = "Seminar Date")]
        // add validation for date
        [SeminarDate()]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd MMM, yyyy}")]
        public DateTime Seminar_Date { get; set; }

        [Required]
        [Display(Name = "Seminar Start Time")]
        [DataType(DataType.Time)]
        public DateTime Starting_Time { get; set; }

        [Required]
        [Display(Name = "Seminar End Time")]
        [SeminarTime()]
        [DataType(DataType.Time)]
        public DateTime Ending_Time { get; set; }

        [Required]
        [Display(Name = "Organizer")]
        public int OrganizerId { get; set; }

        public virtual Organizer Organizer { get; set; }
    }

    // Custom Compare Data Annotation with Client Validation
    public class SeminarDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var seminar = (Seminar)validationContext!.ObjectInstance;
            if (seminar.Seminar_Date < DateTime.Now)
            {
                return new ValidationResult("Seminar Date must be in the future");
            }
            return ValidationResult.Success;
        }
    }

    public class SeminarTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var seminar = (Seminar)validationContext!.ObjectInstance;
            if (seminar.Starting_Time > seminar.Ending_Time)
            {
                return new ValidationResult("Seminar Start Time must be before Seminar End Time");
            }
            return ValidationResult.Success;
        }
    
    }

    // check if link is valid
    public class SeminarLinkAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var seminar = (Seminar)validationContext!.ObjectInstance;
            if (!seminar.link.Contains("http"))
            {
                return new ValidationResult("Seminar Link must start with http");
            }
            return ValidationResult.Success;
        }
    }
}
    
