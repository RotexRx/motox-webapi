using Application.Interfaces;
using MediatR;

namespace Application.Features.Advertisements.Commands;

public class RejectedCommandHandler : IRequestHandler<RejectedCommand, bool>
{
    private readonly IAdvertisementRepository _repository;

    public RejectedCommandHandler(IAdvertisementRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(RejectedCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.Reject(request.Id, cancellationToken);
        return result;
    }
}