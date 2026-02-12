using MediatR;

public class CreateAdvertisementCommand : IRequest<int>
{
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public int Mileage { get; set; }

    public DocumentStatus DocumentStatus { get; set; }

    public long Price { get; set; }
    public string PhoneNumber { get; set; } = default!;
    public string Description { get; set; } = default!;
    public bool Published { get; set; }
    public List<string> Features { get; set; } = new();
    public List<string> ImagesBase64 { get; set; } = new();
}
