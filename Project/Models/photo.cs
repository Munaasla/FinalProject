namespace Project.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int BabyId { get; set; }
        public string FileName { get; set; } = "";
        public DateTime UploadedAt { get; set; }
    }
}
