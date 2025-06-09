using Project.Data;
using Project.Models;
using BCrypt.Net;

namespace Project.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;

        public AuthService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Parent Login(string email, string password)
        {
            var user = _context.Parents.FirstOrDefault(p => p.Email == email);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
                return user;
            return null;
        }

        public bool Register(string email, string password, string name, string phoneNumber)
        {
            if (_context.Parents.Any(p => p.Email == email))
                return false;
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            var parent = new Parent
            {
                Name = name,
                Email = email,
                Password = hashedPassword,
                PhoneNumber = phoneNumber
            };
            _context.Parents.Add(parent);
            _context.SaveChanges();
            return true;
        }


        bool IAuthService.Register(string email, string password, string name, string phoneNumber)
        {
            return Register(email, password, name, phoneNumber);
        }


        public Parent GetParentById(int id)
        {
            return _context.Parents.FirstOrDefault(p => p.Id == id);
        }
    }
}
