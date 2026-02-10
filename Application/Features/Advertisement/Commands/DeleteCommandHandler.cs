using Application.Interfaces;
using MediatR;

namespace Application.Features.Advertisements.Commands;

public class DeleteCommandHandler : IRequestHandler<DeleteCommand, bool>
{
    private readonly IAdvertisementRepository _repository;

    public DeleteCommandHandler(IAdvertisementRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var result = await _repository.Delete(request.Id, cancellationToken);
        return result;
    }
}