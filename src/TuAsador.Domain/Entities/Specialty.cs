namespace TuAsador.Domain.Entities;

public class Specialty
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ICollection<AsadorProfile> Asadores { get; set; } = new List<AsadorProfile>();
}
