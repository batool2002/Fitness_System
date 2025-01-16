using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Fitness.Controllers
{
    public class GuestsController : Controller
    {
        private readonly ModelContext _context;

        public GuestsController(ModelContext context)
        {
            _context = context;
        }


        // GET: GuestUsers/Index
        public IActionResult Index()
        {
            // Pass any data you'd like to display on the homepage (e.g., highlights, promotions)
            ViewBag.Highlight = "Join the best fitness community and achieve your goals!";
            ViewBag.FeaturedTrainers = _context.Trainers.Take(3).ToList(); // Example: Fetch top 3 trainers
            return View();
        }

        // GET: View Membership Plans
        // GET: View Membership Plans
        public async Task<IActionResult> MembershipPlans()
        {
            // Fetch all subscription plans and include related memberships for demonstration purposes
            var plans = await _context.Subscriptions
                .Include(s => s.Memberships)
                .ToListAsync();
            return View(plans);
        }


        // GET: Read Testimonials
        public async Task<IActionResult> Testimonials()
        {
            var approvedTestimonials = await _context.Testimonials
                .Where(t => t.Status == "Approved")
                .Include(t => t.Member)
                .ToListAsync();
            return View(approvedTestimonials);
        }

        // GET: View Trainer Profiles
        public async Task<IActionResult> Trainers()
        {
            var trainers = await _context.Trainers.Include(t => t.User).ToListAsync();
            return View(trainers);
        }

        // GET: Contact Form
        public IActionResult Contact()
        {
            return View();
        }

        // POST: Submit Contact Form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(string name, string email, string message)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Process the contact form (e.g., save to database or send email)
            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction(nameof(Contact));
        }

        // GET: Static Pages (e.g., About Us, FAQ)
        public IActionResult StaticPage(string pageName)
        {
            var page = _context.Staticpages.FirstOrDefault(p => p.PageName == pageName);
            if (page == null)
            {
                return NotFound();
            }
            return View(page);
        }
    }
}
