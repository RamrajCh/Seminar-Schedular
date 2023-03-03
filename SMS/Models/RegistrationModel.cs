using System.ComponentModel.DataAnnotations;

namespace SMS.Models
{
    public class Registration
    {
        [Key]
        public int id { get; set; }

        [Required]
        [Display(Name="Attendee")]
        public int attendeeId { get; set; }
        public virtual Person attendee { get; set; }

        [Required]
        [Display(Name = "Seminar")]
        public int seminarId { get; set; }
        public virtual Seminar seminar { get; set; }
    }
}