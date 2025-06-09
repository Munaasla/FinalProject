using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Controllers
{
    public class PhotoController : Controller
    {
        private readonly AppDbContext _ctx;
        private readonly IWebHostEnvironment _env;
        public PhotoController(AppDbContext ctx, IWebHostEnvironment env)
        {
            _ctx = ctx;
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(int babyId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file selected.");
            var uploads = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploads))
                Directory.CreateDirectory(uploads);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploads, fileName);
            using var stream = System.IO.File.Create(filePath);
            await file.CopyToAsync(stream);
            _ctx.Photos.Add(new Photo
            {
                BabyId = babyId,
                FileName = fileName,
                UploadedAt = DateTime.UtcNow
            });
            await _ctx.SaveChangesAsync();
            return RedirectToAction("Summary", "Baby", new { id = babyId });
        }

        [HttpPost]
        public IActionResult Delete(int id, int babyId)
        {
            var photo = _ctx.Photos.Find(id);
            if (photo != null)
            {
                var path = Path.Combine(_env.WebRootPath, "uploads", photo.FileName);
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
                _ctx.Photos.Remove(photo);
                _ctx.SaveChanges();
            }
            return RedirectToAction("Summary", "Baby", new { id = babyId });
        }
    }
}
