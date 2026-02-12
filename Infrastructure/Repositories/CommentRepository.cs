using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Comments;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _context;
        private readonly string _uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");


        public CommentRepository(AppDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_uploadFolder))
                Directory.CreateDirectory(_uploadFolder);
        }

        public async Task NewComment(Comment comment, CancellationToken cancellationToken)
        {

            await _context.Comments.AddAsync(comment, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }


        public async Task<List<CommentDto>> GetCommentsAsync(int Id, CancellationToken cancellationToken)
        {
            var ads = await _context.Comments
            .Where(w => w.AdvertisementId == Id).ToListAsync();

            var adsDto = ads.Select(b => new CommentDto
            {
                Message = b.Message,
                Id = b.Id,
                Reply = b.Reply,
                Username = b.Username

            }).ToList();

            return adsDto;
        }

        public async Task<List<CommentDto>> GetAllCommentsAsync( CancellationToken cancellationToken)
        {
            var ads = await _context.Comments
            .ToListAsync();

            var adsDto = ads.Select(b => new CommentDto
            {
                Message = b.Message,
                Id = b.Id,
                Reply = b.Reply,
                Username = b.Username,
                AdId = b.AdvertisementId

            }).ToList();

            return adsDto;
        }

        

        public async Task<bool> Reply(
             int id,
             string reply,
             CancellationToken cancellationToken)
        {
            var comments = await _context.Comments
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

            if (comments == null) return false;


            comments.Reply = reply;

            _context.Comments.Update(comments);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

      
        public async Task<bool> Delete(int Id, CancellationToken cancellationToken)
        {

            var query = await _context.Comments
                .Where(w => w.Id == Id).FirstOrDefaultAsync(cancellationToken);

            if (query == null) return false;


            _context.Comments.Remove(query);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }


    }
}
