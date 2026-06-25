using System;
using System.Collections.Generic;

namespace TuAsador.Infrastructure;

public partial class PortfolioImage
{
    public Guid Id { get; set; }

    public Guid AsadorProfileId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public bool? IsApproved { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AsadorProfile AsadorProfile { get; set; } = null!;
}
