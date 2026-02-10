
using Domain.Entities;
using Domain.Entities.Contact;

namespace Application.Interfaces;

public interface IContactRepository

{
    Task CreateAsync(
        Contact contact,
        CancellationToken cancellationToken);
}
