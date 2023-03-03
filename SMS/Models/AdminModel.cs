using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Admin
    {
        [Key]
        public int id { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name = "Password")]
        public string password { get; set; }
    }
}