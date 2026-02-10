using MediatR;

namespace MotoX.Application.Features.Home.Queries.GetHomeInfo;

public class GetHomeInfoQueryHandler
    : IRequestHandler<GetHomeInfoQuery, HomeInfoDto>
{
    public Task<HomeInfoDto> Handle(
        GetHomeInfoQuery request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new HomeInfoDto
        {
            Title = "MotoX Platform",
            Description = "High security vehicle system"
        });
    }
}
