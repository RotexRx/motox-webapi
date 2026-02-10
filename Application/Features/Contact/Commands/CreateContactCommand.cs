using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Application.Features.Contact.Commands;

public class CreateContactCommand : IRequest<int>
{
    public string Name { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public string Phone { get; set; }

}


