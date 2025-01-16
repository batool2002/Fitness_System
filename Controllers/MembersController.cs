using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness.Models;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Security.Claims;

namespace Fitness.Controllers
{
    public class MembersController : Controller
    {
        private readonly ModelContext _context;

        public MembersController(ModelContext context)
        {
            _context = context;
        }

        // GET: Member Dashboard
        public async Task<IActionResult> Index()
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: View All Subscription Plans
        public async Task<IActionResult> ViewSubscriptions()
        {
            var subscriptions = await _context.Subscriptions.ToListAsync();
            return View(subscriptions);
        }

        // POST: Subscribe to a Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(decimal subscriptionId)
        {
            var memberId = GetLoggedInUserId();
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            var subscription = await _context.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
            {
                return NotFound();
            }

            var membership = new Membership
            {
                MemberId = member.MemberId,
                SubscriptionId = subscription.SubscriptionId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(int.Parse(subscription.Duration)),
                PaymentStatus = "Pending"
            };

            _context.Memberships.Add(membership);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManageProfile));
        }

        // GET: View Profile
        public async Task<IActionResult> ManageProfile()
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Edit Profile
        public async Task<IActionResult> EditProfile()
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var member = await _context.Members.Include(m => m.User).FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(Member member)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
                return RedirectToAction(nameof(ManageProfile));
            }
            return View(member);
        }

        // GET: View Testimonials
        public async Task<IActionResult> ViewTestimonials()
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var testimonials = await _context.Testimonials.Where(t => t.Member.UserId == userId).ToListAsync();
            return View(testimonials);
        }

        // POST: Submit Testimonial
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitTestimonial(string content)
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == userId);

            if (member == null)
            {
                return NotFound();
            }

            var testimonial = new Testimonial
            {
                MemberId = member.MemberId,
                Content = content,
                Status = "Pending",
                CreatedAt = DateTime.Now
            };

            _context.Testimonials.Add(testimonial);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ViewTestimonials));
        }

        // Helper Method: Get Logged-in UserId
        private decimal GetLoggedInUserId()
        {

            // Assuming a simple method to retrieve the logged-in user's ID
            var userIdString = HttpContext.Session.GetString("UserId");
            return decimal.Parse(userIdString);
         
        }
    }
}
