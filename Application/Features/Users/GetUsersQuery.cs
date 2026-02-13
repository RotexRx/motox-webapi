using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Application.Features.Users.Queries;

public record GetUsersQuery(int Count) : IRequest<List<UserDto>>;
