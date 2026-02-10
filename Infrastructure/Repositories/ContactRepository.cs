using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Contact;
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
            await _context.Contacts.AddAsync(contact, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

     
    }
}
