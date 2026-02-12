
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Comments;

namespace Application.Interfaces;

public interface ICommentRepository

{
    Task<List<CommentDto>> GetCommentsAsync(int Id, CancellationToken cancellationToken);
    Task<List<CommentDto>> GetAllCommentsAsync( CancellationToken cancellationToken);

    Task<bool> Reply(
            int Id,
            string reply,
            CancellationToken cancellationToken);
    Task<bool> Delete(int Id, CancellationToken cancellationToken);
    Task NewComment(
        Comment comment,
        CancellationToken cancellationToken);
}
 