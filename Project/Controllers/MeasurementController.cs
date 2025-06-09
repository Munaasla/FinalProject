using Microsoft.AspNetCore.Mvc;
using Project.Data;
using Project.Models;
using System;
using System.Linq;

namespace Project.Controllers
{
    public class MeasurementController(AppDbContext context) : Controller
    {
        private readonly AppDbContext _context = context;

        [HttpGet]
        public IActionResult Create(int babyId)
        {
            ViewBag.BabyId = babyId;
            return View(new Measurement { BabyId = babyId });   
        }

        [HttpPost]
        public IActionResult Create(Measurement measurement)
        {
            if (ModelState.IsValid)
            {
                if (measurement.Weight < 2 || measurement.Weight > 30)
                    ModelState.AddModelError("Weight", "Weight must be between 2 and 30 kg");
                if (measurement.Height < 40 || measurement.Height > 120)
                    ModelState.AddModelError("Height", "Height must be between 40 and 120 cm");
                if (measurement.Weight <= 0 || measurement.Height <= 0)
                {
                    ModelState.AddModelError(string.Empty, "Weight and Height must be greater than zero.");
                    ViewBag.BabyId = measurement.BabyId;
                    return View(measurement);
                }
                if (measurement.BabyId == 0)
                {
                    return BadRequest("Missing BabyId");
                }
                _context.Measurements.Add(measurement);
                _context.Vaccinations.Add(new Vaccination
                {
                    BabyId = measurement.BabyId,
                    Date = measurement.Date,
                    Note = "Auto-generated from measurement"
                });
                var baby = _context.Babies.FirstOrDefault(b => b.Id == measurement.BabyId);
                if (baby != null)
                {
                    baby.Weight = measurement.Weight;
                    baby.Height = measurement.Height;
                }
                _context.SaveChanges();
                return RedirectToAction("Summary", "Baby", new { id = measurement.BabyId });
            }
            if (!ModelState.IsValid)
            {
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error in {key}: {error.ErrorMessage}");
                    }
                }
            }
            ViewBag.BabyId = measurement.BabyId;
            return View(measurement);
        }
    }
}