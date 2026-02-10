using MediatR;

public record RejectedCommand(int Id) : IRequest<bool>;