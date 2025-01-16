using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Membership
{
    public decimal MembershipId { get; set; }

    public decimal MemberId { get; set; }

    public decimal SubscriptionId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public virtual Member Member { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Subscription Subscription { get; set; } = null!;
}
