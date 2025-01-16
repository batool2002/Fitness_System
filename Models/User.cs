using System;
using System.Collections.Generic;

namespace Fitness.Models;

public partial class User
{
    public decimal UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Member? Member { get; set; }

    public virtual Trainer? Trainer { get; set; }


    //public virtual ICollection<Member> Members { get; set; } = new List<Member>();

    //public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}

