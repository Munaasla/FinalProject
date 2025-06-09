using System.ComponentModel.DataAnnotations;
namespace Project.Models
{
    public class Measurement
    {
        public int Id { get; set; }
        [Required]
        public int BabyId { get; set; }
        public Baby? Baby { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        public double Height { get; set; }
    }

}
