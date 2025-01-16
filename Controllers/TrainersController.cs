using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Fitness.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Fitness.Controllers
{
  //  [Authorize(Roles = "Trainer")]
    public class TrainersController : Controller
    {
        private readonly ModelContext _context;

        public TrainersController(ModelContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var userId = GetLoggedInUserId(); // Assumes a method to get logged-in user's ID
            var trainerId = GetLoggedInTrainerId();                                 // var trainer = await _context.Trainers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId == userId);

            var trainer = await _context.Trainers
      .Include(t => t.Workouts) // Include workouts if needed
      .Include(t => t.User) // Include user details
      .FirstOrDefaultAsync(t => t.UserId == userId && t.TrainerId== trainerId);
            

            if (trainer == null)
            {
                return NotFound();
            }

            // Fetch members assigned to the trainer
            var members = await _context.Members
                .Include(m => m.User)
                .Where(m => m.Workouts.Any(w => w.TrainerId == trainer.TrainerId)) // Members with this trainer's workouts
                .ToListAsync();

            return View(members); // Pass the list of members

           // return View(trainer);
        }

        // GET: Trainer - View Members List
        public async Task<IActionResult> ViewMembers()
        {
            var userId = GetLoggedInUserId();
            var members = await _context.Members.Include(m => m.User).ToListAsync();
            return View(members);
        }

        // GET: Trainer - View Member Profile and Workouts
        public async Task<IActionResult> ViewMemberProfile(decimal memberId)
        {
            var member = await _context.Members
                .Include(m => m.User)
                .Include(m => m.Workouts)
                .FirstOrDefaultAsync(m => m.MemberId == memberId);

            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // GET: Trainer - Create Workout Plan
        public IActionResult CreateWorkoutPlan(decimal memberId)
        {
            //
            //var userId = GetLoggedInUserId();
            ViewBag.MemberId = memberId;
            ViewBag.TrainerId = memberId;
            return View();
        }

        // POST: Trainer - Create Workout Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateWorkoutPlan(decimal memberId, [Bind("PlanName, Description, StartDate, EndDate")] Workout workout)
        {
            if (ModelState.IsValid)
            {
                var trainerId = GetLoggedInTrainerId();
              //  memberId = GetLoggedInMemberId();
                //var trainerId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the trainer's ID
                //workout.TrainerId = Convert.ToDecimal(trainerId); // Assuming the trainer's ID is stored in the user's claims
                  workout.TrainerId = trainerId;
                  workout.MemberId = memberId;
                //workout.TrainerId = 1;
                //workout.MemberId = 2;

                _context.Add(workout);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(ViewMemberProfile), new { memberId });
            }
            return View(workout);
        }

        // GET: Trainer - Edit Workout Plan
        public async Task<IActionResult> EditWorkoutPlan(decimal workoutId)
        {
            var workout = await _context.Workouts.FindAsync(workoutId);
            if (workout == null)
            {
                return NotFound();
            }
            return View(workout);
        }

        // POST: Trainer - Edit Workout Plan
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditWorkoutPlan(decimal workoutId, [Bind("WorkoutId, PlanName, Description, StartDate, EndDate")] Workout workout)
        {
            if (workoutId != workout.WorkoutId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(workout);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Workouts.Any(w => w.WorkoutId == workout.WorkoutId))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(ViewMemberProfile), new { memberId = workout.MemberId });
            }
            return View(workout);
        }

        // GET: Trainer - View Profile
        public async Task<IActionResult> ViewProfile()
        {
            var trainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainer = await _context.Trainers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId.ToString() == trainerId);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainer - Edit Profile
        public async Task<IActionResult> EditProfile()
        {
            var trainerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainer = await _context.Trainers.Include(t => t.User).FirstOrDefaultAsync(t => t.UserId.ToString() == trainerId);

            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainer - Edit Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(decimal trainerId, [Bind("TrainerId, Specialization, ExperienceYears, ContactNumber, ProfilePictureFile")] Trainer trainer)
        {
            if (trainerId != trainer.TrainerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (trainer.ProfilePictureFile != null)
                    {
                        // Handle Profile Picture Upload (save the file path to the database)
                        // Example: Save the file and set trainer.ProfilePicture to the path.
                    }

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
                return RedirectToAction(nameof(ViewProfile));
            }
            return View(trainer);
        }

        private decimal GetLoggedInUserId()
        {

            // Assuming a simple method to retrieve the logged-in user's ID
            var userIdString = HttpContext.Session.GetString("UserId");
            return decimal.Parse(userIdString);

        }

        private decimal GetLoggedInTrainerId()
        {

            // Assuming a simple method to retrieve the logged-in user's ID
            var trainerIdString = HttpContext.Session.GetString("TrainerId");
            return decimal.Parse(trainerIdString);

        }

        private decimal GetLoggedInMemberId()
        {
            // Assuming a simple method to retrieve the logged-in user's ID
            var memberIdString = HttpContext.Session.GetString("TrainerId");
            return decimal.Parse(memberIdString);

        }

    }
}