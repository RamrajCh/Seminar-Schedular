using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Organizer
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Email")]
        public string email { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Full Name")]
        public string name { get; set; }

        [Required]
        [Display(Name = "Verified")]
        public bool isVerified { get; set; }

        public virtual ICollection<Seminar> Seminar { get; set; }
    }
}