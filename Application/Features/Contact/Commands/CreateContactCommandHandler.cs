using Application.Features.Contact.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Contact;
using MediatR;
 
public class CreateContactCommandHandler
    : IRequestHandler<CreateContactCommand, int>
{
    private readonly IContactRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateContactCommandHandler(
        IContactRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    public async Task<int> Handle(
        CreateContactCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        var contact = new Contact
        {
             Body = request.Body,
             Created = DateTime.UtcNow,
             Name = request.Name,
             Phone = request.Phone,
             Subject = request.Subject
        };

  
         
        await _repository.CreateAsync(contact, cancellationToken);

        return contact.Id;
    }
 

}
