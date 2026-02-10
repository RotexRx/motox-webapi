using MediatR;

namespace MotoX.Application.Features.Home.Queries.GetHomeInfo;

public record GetHomeInfoQuery() : IRequest<HomeInfoDto>;
