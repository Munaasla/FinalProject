using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class Vaccination
    {
        public int Id { get; set; }
        [Required] 
        public int BabyId { get; set; }
        public DateTime Date { get; set; }
        public string? Note { get; set; }
        public Baby Baby { get; set; } 
    }
}
