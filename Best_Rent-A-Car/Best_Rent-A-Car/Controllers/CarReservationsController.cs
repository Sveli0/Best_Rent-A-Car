﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Best_Rent_A_Car.Data;
using Best_Rent_A_Car.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;

namespace Best_Rent_A_Car.Controllers
{
    public class CarReservationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CarReservations
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CarReservations.Include(c => c.Car).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: CarReservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations
                .Include(c => c.Car)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (carReservation == null)
            {
                return NotFound();
            }

            return View(carReservation);
        }

        // GET: CarReservations/Create
        public IActionResult Create()
        {
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["LoggedInUserId"] = loggedInUserId;

            ViewData["CarID"] = new SelectList(_context.Cars.OrderBy(x => x.Brand).ThenBy(x => x.Model).Select(c => new
            {
                Id = c.Id,
                FullBrandAndModel = $"{c.Brand} {c.Model}"
            }), "Id", "FullBrandAndModel"); ;
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: CarReservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
        {
            if (ModelState.IsValid)
            {

                _context.Add(carReservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarID"] = new SelectList(_context.Cars, "Id", "Brand", carReservation.CarID);
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id", carReservation.VisibleUserID);
            return View(carReservation);
        }

        // GET: CarReservations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations.FindAsync(id);
            if (carReservation == null)
            {
                return NotFound();
            }
            ViewData["CarID"] = new SelectList(_context.Cars, "Id", "Brand", carReservation.CarID);
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id", carReservation.VisibleUserID);
            return View(carReservation);
        }

        // POST: CarReservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarID,StartDate,EndDate,VisibleUserID")] CarReservation carReservation)
        {
            if (id != carReservation.CarID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(carReservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarReservationExists(carReservation.CarID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CarID"] = new SelectList(_context.Cars, "Id", "Brand", carReservation.CarID);
            ViewData["VisibleUserID"] = new SelectList(_context.Users, "Id", "Id", carReservation.VisibleUserID);
            return View(carReservation);
        }

        // GET: CarReservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carReservation = await _context.CarReservations
                .Include(c => c.Car)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (carReservation == null)
            {
                return NotFound();
            }

            return View(carReservation);
        }

        // POST: CarReservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carReservation = await _context.CarReservations.FindAsync(id);
            _context.CarReservations.Remove(carReservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarReservationExists(int id)
        {
            return _context.CarReservations.Any(e => e.CarID == id);
        }
    }
}