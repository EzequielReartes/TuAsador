namespace TuAsador.Domain.Entities;

public class Availability
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid AsadorProfileId { get; set; }
    public AsadorProfile AsadorProfile { get; set; } = null!;
    public DateOnly Date { get; set; }
    public bool IsAvailable { get; set; } = true;
}
