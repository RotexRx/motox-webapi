using Application.Interfaces;
using MediatR;

namespace Application.Features.Advertisements.Commands;

public class ApproveCommandHandler : IRequestHandler<ApproveCommand, bool>
{
    private readonly IAdvertisementRepository _repository;

    public ApproveCommandHandler(IAdvertisementRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(ApproveCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.Approve(
            request.Id,
            request.Health,
            request.History,
            cancellationToken
        );

        return result;
    }
}