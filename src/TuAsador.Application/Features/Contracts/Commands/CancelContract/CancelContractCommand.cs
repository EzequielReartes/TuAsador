using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Contracts.Commands.CancelContract;

public record CancelContractCommand(Guid ContractId, string UserId) : IRequest<CancelContractResult>;

public class CancelContractResult
{
    public string Message { get; init; } = "Contrato cancelado exitosamente";
}

public class CancelContractCommandHandler : IRequestHandler<CancelContractCommand, CancelContractResult>
{
    private readonly IApplicationDbContext _db;

    public CancelContractCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<CancelContractResult> Handle(CancelContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _db.Contracts
            .Include(c => c.Event)
                .ThenInclude(e => e.Applications)
            .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

        if (contract == null)
            throw new KeyNotFoundException("Contrato no encontrado");

        if (contract.ClientId != request.UserId)
            throw new UnauthorizedAccessException("No eres el dueño de este contrato");

        if (contract.Status != "Pendiente")
            throw new InvalidOperationException("Solo se pueden cancelar contratos pendientes");

        contract.Status = "Cancelado";

        if (contract.Event != null)
        {
            contract.Event.Status = "Disponible";
            if (contract.Event.Applications != null)
            {
                foreach (var app in contract.Event.Applications)
                {
                    app.Status = "Pendiente";
                }
            }
        }

        await _db.SaveChangesAsync(cancellationToken);

        return new CancelContractResult();
    }
}
