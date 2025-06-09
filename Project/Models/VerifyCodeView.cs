using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class VerifyCodeView
    {
        [Required]
        public string Code { get; set; }
    }
}
