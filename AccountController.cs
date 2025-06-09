using System.Collections.Concurrent; // Add this directive at the top of the file  

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private static readonly ConcurrentDictionary<string, string> _otpStore = new(); // Add this field to store OTPs  

        // Existing code remains unchanged  
    }
}
