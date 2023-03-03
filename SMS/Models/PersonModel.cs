using System.ComponentModel.DataAnnotations;
namespace SMS.Models
{
    public class Person
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "KOI ID")]
        public string koiId { get; set; }

        [Required]
        // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,15}$")]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Department Name")]
        public string faculty { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Your Role")]
        public string role { get; set; }

        [Required]
        [Display(Name = "Joined Date")]
        [DataType(DataType.Date)]
        [JoinedDate()]
        public DateTime joinedDate { get; set; }

    }

    public class JoinedDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var person = (Person)validationContext!.ObjectInstance;
            if (person.joinedDate > DateTime.Now)
            {
                return new ValidationResult("Joined Date must be in the past");
            }
            return ValidationResult.Success;
        }
    }
}