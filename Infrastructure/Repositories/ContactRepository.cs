using Application.Features.Contact;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Contact;
using Domain.Entities.Receipts;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _context;

        public ContactRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Contact contact, CancellationToken cancellationToken)
        {
            try
            {
                if (contact == null)
                {
                    throw new ArgumentNullException(nameof(contact));
                }
                await _context.Contacts.AddAsync(contact, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            { 
                throw;
            }
            
        }

        public async Task<List<ContactDto>> GetCommentsAsync(CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Contacts
                .AsQueryable();
                var contacts = await query.ToListAsync(cancellationToken);

                 
                if (query == null)
                    return new List<ContactDto>();

                return query.Select(b => new ContactDto
                {
                    Name = b.Name,
                    Body = b.Body,
                    Created = b.Created,
                    Subject = b.Subject,
                    Phone = b.Phone 
                }).ToList();
                
                 
            }
            catch (Exception)
            {
                throw;
            }

        }

        
    }
}
