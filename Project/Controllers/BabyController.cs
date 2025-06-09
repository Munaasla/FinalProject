using Microsoft.AspNetCore.Mvc;
using Project.Models;
using Project.Data;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    public class BabyController : Controller
    {
        private readonly AppDbContext _context;

        public BabyController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchName)
        {
            var babies = await _context.Babies
                .Include(b => b.Measurements)
                .Include(b => b.Vaccinations)
                .ToListAsync();
            if (!string.IsNullOrEmpty(searchName))
            {
                babies = babies
                    .Where(b => b.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            return View(babies);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Baby baby, IFormFile Photo)
        {
            var parentId = HttpContext.Session.GetInt32("ParentId");
            if (parentId == null)
                return RedirectToAction("Login", "Account");
            if (baby.Weight <= 0 || baby.Height <= 0)
                ModelState.AddModelError(string.Empty, "Weight and Height must be greater than 0.");
            if (baby.DateOfBirth > DateTime.Today)
                ModelState.AddModelError(string.Empty, "Date of Birth cannot be in the future.");
            ModelState.Remove("Parent");
            ModelState.Remove("Photo");
            if (!ModelState.IsValid)
                return View(baby);
            if (Photo != null && Photo.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Photo.FileName}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(stream);
                }
                baby.PhotoPath = "/uploads/" + fileName;
            }
            baby.ParentId = parentId.Value;
            _context.Babies.Add(baby);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }



        public IActionResult Chart(int id)
        {
            var baby = _context.Babies
                .Include(b => b.Measurements)
                .FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            return View(new List<Baby> { baby });
        }

        public IActionResult Calendar()
        {
            var parentId = HttpContext.Session.GetInt32("ParentId");
            if (parentId == null)
                return RedirectToAction("Login", "Account");
            var babies = _context.Babies
                .Include(b => b.Vaccinations) 
                .Where(b => b.ParentId == parentId)
                .ToList();
            return View(babies);
        }

        public IActionResult Dashboard()
        {
            var parentId = HttpContext.Session.GetInt32("ParentId");
            if (parentId == null)
                return RedirectToAction("Login", "Account");
            var parent = _context.Parents.FirstOrDefault(p => p.Id == parentId);
            if (parent == null)
                return RedirectToAction("Login", "Account");
            var babies = _context.Babies
                .Where(b => b.ParentId == parentId)
                .ToList();
            var model = new DashboardModel
            {
                ParentName = parent.Name,
                Babies = babies,
                AverageWeight = babies.Count > 0 ? babies.Average(b => b.Weight) : 0,
                AverageHeight = babies.Count > 0 ? babies.Average(b => b.Height) : 0
            };
            return View(model);
        }

        public IActionResult Summary(int id)
        {
            var baby = _context.Babies
                .Include(b => b.Measurements)
                .Include(b => b.Photos)
                .Include(b => b.Vaccinations)
                .FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            return View(baby);
        }

        [HttpPost]
        public IActionResult UpdateVaccinationDate(int babyId, DateTime newDate)
        {
            var baby = _context.Babies
                .Include(b => b.Vaccinations)
                .FirstOrDefault(b => b.Id == babyId);
            if (baby == null)
                return NotFound();
            if (baby.Vaccinations == null)
                baby.Vaccinations = new List<Vaccination>();
            baby.Vaccinations.Add(new Vaccination
            {
                BabyId = baby.Id,
                Date = newDate,
                Note = "Manual update"
            });
            _context.SaveChanges();
            return RedirectToAction("Summary", new { id = babyId });
        }



        public IActionResult Photos(int id)
        {
            var baby = _context.Babies
                .Include(b => b.Photos)
                .FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            return View(baby);
        }

        public IActionResult Growth(int id)
        {
            var baby = _context.Babies
                .Include(b => b.Measurements)
                .FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            return View(baby);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var baby = _context.Babies.FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            return View(baby);
        }

        [HttpPost]
        public IActionResult Edit(Baby updatedBaby, IFormFile Photo)
        {
            var baby = _context.Babies.FirstOrDefault(b => b.Id == updatedBaby.Id);
            if (baby == null)
                return NotFound();
            ModelState.Remove("Parent");
            ModelState.Remove("Photo");
            if (updatedBaby.Weight <= 0 || updatedBaby.Height <= 0)
                ModelState.AddModelError(string.Empty, "Weight and Height must be greater than 0.");
            if (updatedBaby.DateOfBirth > DateTime.Today)
                ModelState.AddModelError(string.Empty, "Date of Birth cannot be in the future.");
            if (!ModelState.IsValid)
                return View(updatedBaby);
            baby.Name = updatedBaby.Name;
            baby.DateOfBirth = updatedBaby.DateOfBirth;
            baby.Weight = updatedBaby.Weight;
            baby.Height = updatedBaby.Height;
            if (Photo != null && Photo.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{Photo.FileName}";
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    Photo.CopyTo(stream);
                }
                baby.PhotoPath = "/uploads/" + fileName;
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var baby = _context.Babies.FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            return View(baby);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var baby = _context.Babies.FirstOrDefault(b => b.Id == id);
            if (baby == null)
                return NotFound();
            _context.Babies.Remove(baby);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
