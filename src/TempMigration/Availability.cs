using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class Availability
{
    public Guid Id { get; set; }

    public Guid AsadorProfileId { get; set; }

    public DateOnly Date { get; set; }

    public bool IsAvailable { get; set; }

    public virtual AsadorProfile AsadorProfile { get; set; } = null!;
}
