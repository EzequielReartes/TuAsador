using MediatR;

namespace TuAsador.Application.Features.Health.Queries.GetHealth;

public record GetHealthQuery : IRequest<HealthResponse>;

public class HealthResponse
{
    public string Status { get; init; } = "operativa";
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Version { get; init; } = "1.0.0";
}

public class GetHealthQueryHandler : IRequestHandler<GetHealthQuery, HealthResponse>
{
    public Task<HealthResponse> Handle(GetHealthQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HealthResponse());
    }
}
