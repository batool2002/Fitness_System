using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class Payment
{
    public decimal PaymentId { get; set; }

    public decimal MembershipId { get; set; }

    public decimal Amount { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? InvoicePdf { get; set; }

    public virtual Membership Membership { get; set; } = null!;
}
