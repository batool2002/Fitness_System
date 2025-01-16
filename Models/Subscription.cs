using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Subscription
{
    public decimal SubscriptionId { get; set; }

    public string PlanName { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
}
