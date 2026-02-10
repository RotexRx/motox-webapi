using MediatR;

public record DeleteCommand(int Id) : IRequest<bool>;