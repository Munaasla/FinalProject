using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Project.Models;
using Project.Services;
using Project.Data;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private static Dictionary<string, string> _otpStore = new();

        public AccountController(IAuthService authService, AppDbContext context, IConfiguration config)
        {
            _authService = authService;
            _context = context;
            _config = config;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var parent = _authService.Login(email, password);
            if (parent != null)
            {
                HttpContext.Session.SetInt32("ParentId", parent.Id);
                return RedirectToAction("Index", "Baby");
            }
            ViewBag.Error = "Invalid email or password";
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string email, string password, string name, string phoneNumber)
        {
            var success = _authService.Register(email, password, name, phoneNumber);
            var parent = _authService.Login(email, password);
            if (!success)
            {
                ViewBag.Error = "Email already exists.";
                return View();
            }
            if (parent == null)
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
            HttpContext.Session.SetInt32("ParentId", parent.Id);
            return RedirectToAction("Index", "Baby");
        }

        public IActionResult Dashboard()
        {
            var parentId = HttpContext.Session.GetInt32("ParentId");
            if (!parentId.HasValue)
                return RedirectToAction("Login", "Account");
            var parent = _authService.GetParentById(parentId.Value);
            var babies = _context.Babies.Where(b => b.ParentId == parentId).ToList();
            var model = new DashboardModel
            {
                ParentName = parent.Name,
                Babies = babies,
                AverageWeight = babies.Any() ? babies.Average(b => b.Weight) : 0,
                AverageHeight = babies.Any() ? babies.Average(b => b.Height) : 0
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var user = _context.Parents.FirstOrDefault(p => p.Email == model.ContactInfo);
            if (user == null)
            {
                ModelState.AddModelError("", "No user found with that email.");
                return View(model);
            }
            string code = new Random().Next(100000, 999999).ToString();
            _otpStore[model.ContactInfo] = code;
            var smtpHost = _config["Smtp:Host"];
            var smtpPort = int.Parse(_config["Smtp:Port"]);
            var smtpUser = _config["Smtp:Username"];
            var smtpPass = _config["Smtp:Password"];
            var mail = new MailMessage();
            mail.From = new MailAddress(smtpUser);
            mail.To.Add(model.ContactInfo);
            mail.Subject = "Reset Code";
            mail.Body = $"Your verification code is: {code}";
            using (var smtp = new SmtpClient(smtpHost, smtpPort))
            {
                smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                smtp.EnableSsl = true;
                await smtp.SendMailAsync(mail);
            }
            TempData["ContactInfo"] = model.ContactInfo;
            return RedirectToAction("VerifyCode");
        }

        [HttpGet]
        public IActionResult VerifyCode()
        {
            return View(new VerifyCodeViewModel { ContactInfo = TempData["ContactInfo"]?.ToString() });
        }

        [HttpPost]
        public IActionResult VerifyCode(VerifyCodeViewModel model)
        {
            if (_otpStore.TryGetValue(model.ContactInfo, out var code) && code == model.Code)
            {
                TempData["VerifiedContact"] = model.ContactInfo;
                _otpStore.Remove(model.ContactInfo);
                return RedirectToAction("ResetPassword");
            }
            ViewBag.Error = "Invalid code.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel { ContactInfo = TempData["VerifiedContact"]?.ToString() });
        }

        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel model)
        {
            var user = _context.Parents.FirstOrDefault(p => p.Email == model.ContactInfo);
            if (user != null)
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "User not found.");
            return View(model);
        }
    }
}
