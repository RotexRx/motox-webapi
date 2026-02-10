namespace MotoX.Application.Features.Bikes.Queries.GetLatestBikes;

public record BikeDto
{
    public int Id { get; set; }
    public string Category { get; set; }
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public string Price { get; set; }
    public string Image { get; set; }
    public string Badge { get; set; }
}
