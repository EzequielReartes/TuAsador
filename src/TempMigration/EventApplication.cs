using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class EventApplication
{
    public Guid Id { get; set; }

    public Guid EventId { get; set; }

    public Guid AsadorProfileId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public virtual AsadorProfile AsadorProfile { get; set; } = null!;

    public virtual Event Event { get; set; } = null!;
}
