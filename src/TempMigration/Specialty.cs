using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class Specialty
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<AsadorProfile> Asadores { get; set; } = new List<AsadorProfile>();
}
