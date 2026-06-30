using MediatR;
using Microsoft.EntityFrameworkCore;
using TuAsador.Application.Common.Interfaces;

namespace TuAsador.Application.Features.Contracts.Commands.FinishContract;

public record FinishContractCommand(Guid ContractId, string UserId) : IRequest<FinishContractResult>;

public class FinishContractResult
{
    public string Message { get; init; } = "Evento finalizado correctamente";
    public Guid ContractId { get; init; }
}

public class FinishContractCommandHandler : IRequestHandler<FinishContractCommand, FinishContractResult>
{
    private readonly IApplicationDbContext _db;

    public FinishContractCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<FinishContractResult> Handle(FinishContractCommand request, CancellationToken cancellationToken)
    {
        var contract = await _db.Contracts
            .FirstOrDefaultAsync(c => c.Id == request.ContractId, cancellationToken);

        if (contract == null)
            throw new KeyNotFoundException("Contrato no encontrado");

        if (contract.ClientId != request.UserId)
            throw new UnauthorizedAccessException("No eres el dueño de este contrato");

        if (contract.Status != "Pendiente")
            throw new InvalidOperationException("Solo se pueden finalizar contratos pendientes");

        contract.Status = "Finalizado";
        await _db.SaveChangesAsync(cancellationToken);

        return new FinishContractResult { ContractId = contract.Id };
    }
}
