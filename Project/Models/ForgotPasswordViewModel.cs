using System.ComponentModel.DataAnnotations;

namespace Project.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string ContactInfo { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string ContactInfo { get; set; }
        [Required]
        public string Code { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        public string ContactInfo { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
