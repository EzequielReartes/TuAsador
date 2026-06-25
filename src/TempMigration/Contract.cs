using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class Contract
{
    public Guid Id { get; set; }

    public string ClientId { get; set; } = null!;

    public Guid AsadorProfileId { get; set; }

    public Guid? EventId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime Date { get; set; }

    public TimeOnly Time { get; set; }

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public int PeopleCount { get; set; }

    public string EventType { get; set; } = null!;

    public string? ServiceDesired { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AsadorProfile AsadorProfile { get; set; } = null!;

    public virtual AspNetUser Client { get; set; } = null!;

    public virtual Event? Event { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
}
