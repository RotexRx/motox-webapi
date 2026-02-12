using Application.Features.Comments.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Comments;
using MediatR;

public class CommentCommandHandler
    : IRequestHandler<CommentCommand, int>
{
    private readonly ICommentRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CommentCommandHandler(
        ICommentRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    public async Task<int> Handle(
        CommentCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        var newComment = new Comment
        {
            AdvertisementId = request.Id,
            Message = request.Message,
            Username = request.Username,
            
        };

        await _repository.NewComment(newComment, cancellationToken);

        return newComment.Id;
    }
}
