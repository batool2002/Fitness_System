using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fitness.Models;

namespace Fitness.Controllers

{
    public class AdminsController : Controller
    {
        private readonly ModelContext _context;

        public AdminsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Admin Dashboard - Key statistics


        public async Task<IActionResult> Index()
        {
            if (_context == null)
            {
                return Problem("Database context is not initialized.");
            }

            try
            {
                var memberCount = await _context.Members.CountAsync();
                var activeSubscriptions = await _context.Memberships.CountAsync(m => m.PaymentStatus == "Paid");
                var totalRevenue = await _context.Payments.SumAsync(p => p.Amount);

                ViewBag.MemberCount = memberCount;
                ViewBag.ActiveSubscriptions = activeSubscriptions;
                ViewBag.TotalRevenue = totalRevenue;
            }
            catch (Exception ex) {
                Console.WriteLine($"Error fetching data for Admin Dashboard: {ex.Message}");
                return Problem("An error occurred while fetching dashboard data.");
            }


            return View();
        }

        // GET: Admin - View Member Profiles
        public async Task<IActionResult> ManageMembers()
        {
            var members = await _context.Members.Include(m => m.User).ToListAsync();
            return View(members);
        }

        // GET: Admin - Create Member
        public IActionResult CreateMember()
        {
            return View();
        }

        // POST: Admin - Create Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMember([Bind("FullName, DateOfBirth, ContactNumber, Address, SubscriptionId")] Member member)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Username = member.FullName.ToLower().Replace(" ", "_"),
                    Password = "defaultPassword", // This should be a default password or a method to generate secure passwords
                    Role = "Member",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Add(user);
                await _context.SaveChangesAsync();

                member.UserId = user.UserId;
                _context.Add(member);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ManageMembers));
            }

            return View(member);
        }

        // GET: Admin - Edit Member
        public async Task<IActionResult> EditMember(int id)
        {
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Admin - Edit Member
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMember(int id, [Bind("MemberId, FullName, DateOfBirth, ContactNumber, Address, SubscriptionId")] Member member)
        {
            if (id != member.MemberId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _context.Users.FindAsync(member.UserId);
                    user.UpdatedAt = DateTime.Now;
                    _context.Update(user);
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Members.Any(m => m.MemberId == member.MemberId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(ManageMembers));
            }
            return View(member);
        }

        // GET: Admin - Delete Member
        public async Task<IActionResult> DeleteMember(int id)
        {
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.MemberId == id);
            if (member == null)
            {
                return NotFound();
            }
            return View(member);
        }

        // POST: Admin - Delete Member
        [HttpPost, ActionName("DeleteMember")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMemberConfirmed(int id)
        {
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.MemberId == id);
            if (member != null)
            {
                _context.Users.Remove(member.User);
                _context.Members.Remove(member);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageMembers));
        }

        // GET: Admin - Manage Trainers (Create, Update, Delete Trainers)
        public async Task<IActionResult> ManageTrainers()
        {
            var trainers = await _context.Trainers.Include(t => t.User).ToListAsync();
            return View(trainers);

        }

        // GET: Admin - Create Trainer
        public IActionResult CreateTrainer()
        {
            return View(new Trainer());
        }

        // POST: Admin - Create Trainer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTrainer([Bind("ExperienceYears,Specialization, ContactNumber")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTrainers));
            }
            return View(trainer);
        }

        // GET: Admin - Edit Trainer
        public async Task<IActionResult> EditTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Admin - Edit Trainer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(int id, [Bind("TrainerId,ExperienceYears, ProfilePicture, Specialization, ContactNumber")] Trainer trainer)
        {
            if (id != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Trainers.Any(t => t.TrainerId == trainer.TrainerId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(ManageTrainers));
            }
            return View(trainer);
        }

        // GET: Admin - Delete Trainer
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Admin - Delete Trainer
        [HttpPost, ActionName("DeleteTrainer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTrainerConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTrainers));
        }

        // Approve or Reject Testimonials
        public async Task<IActionResult> ManageTestimonials()
        {
            var testimonials = await _context.Testimonials.Include(t => t.Member).ToListAsync();
            return View(testimonials);
        }

        // Approve Testimonial
        public async Task<IActionResult> ApproveTestimonial(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.Status = "Approved";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTestimonials));
        }

        // Reject Testimonial
        public async Task<IActionResult> RejectTestimonial(int id)
        {
            var testimonial = await _context.Testimonials.FindAsync(id);
            if (testimonial != null)
            {
                testimonial.Status = "Rejected";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ManageTestimonials));
        }

        // Manage Static Pages
        public async Task<IActionResult> ManageStaticPages()
        {
            var pages = await _context.Staticpages.ToListAsync();
            return View(pages);
        }

        // Edit Static Page
        public async Task<IActionResult> EditStaticPage(int id)
        {
            var page = await _context.Staticpages.FindAsync(id);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }

        // POST: Update Static Page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaticPage(int id, [Bind("PageId, PageName, Content")] Staticpage page)
        {
            if (id != page.PageId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    page.UpdatedAt = DateTime.Now;
                    _context.Update(page);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Staticpages.Any(p => p.PageId == page.PageId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(ManageStaticPages));
            }
            return View(page);
        }

        // Generate Report (monthly and annual)
        public IActionResult ViewReports()
        {
            var reports = _context.Reports.ToList();
            return View(reports);
        }
    }
}
