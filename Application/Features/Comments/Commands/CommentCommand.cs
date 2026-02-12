using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Application.Features.Comments.Commands
{
    public class CommentCommand : IRequest<int>
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public int Id { get; set; }
    }
}
 