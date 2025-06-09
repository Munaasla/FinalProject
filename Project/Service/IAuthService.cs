using Project.Models;

namespace Project.Services
{
    public interface IAuthService
    {
        Parent Login(string email, string password);
        bool Register(string email, string password, string name, string phoneNumber);
        Parent GetParentById(int id); 
    }
}
