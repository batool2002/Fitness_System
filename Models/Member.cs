using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Member
{
    public decimal MemberId { get; set; }

    public decimal UserId { get; set; }

    public string FullName { get; set; } = null!;

    public DateTime? DateOfBirth { get; set; }

    public string? ContactNumber { get; set; }

    public string? Address { get; set; }

    public decimal? SubscriptionId { get; set; }

    public string? ProfilePicture { get; set; }

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();

    public virtual Subscription? Subscription { get; set; }

    public virtual ICollection<Testimonial> Testimonials { get; set; } = new List<Testimonial>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Workout> Workouts { get; set; } = new List<Workout>();
}
