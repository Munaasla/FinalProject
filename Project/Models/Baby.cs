using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Project.Models
{
    public class Baby
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public double Weight { get; set; }
        public double Height { get; set; }
        public string? PhotoPath { get; set; }
        public int ParentId { get; set; }
        public ICollection<Vaccination> Vaccinations { get; set; } = new List<Vaccination>();
        public ICollection<Measurement> Measurements { get; set; } = new List<Measurement>();
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        [BindNever]
        public Parent Parent { get; set; }
    }
}
