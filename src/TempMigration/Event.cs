using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class Event
{
    public Guid Id { get; set; }

    public string ClientId { get; set; } = null!;

    public DateTime Date { get; set; }

    public TimeOnly Time { get; set; }

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public int PeopleCount { get; set; }

    public string EventType { get; set; } = null!;

    public string? ServiceDesired { get; set; }

    public string? Notes { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AspNetUser Client { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<EventApplication> EventApplications { get; set; } = new List<EventApplication>();
}
