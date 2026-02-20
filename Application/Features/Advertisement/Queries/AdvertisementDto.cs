using Domain.Entities;
using Domain.Entities.Comments;

public class AdvertisementDto
{
    public int Id { get; set; }
    public string Brand { get; set; } = default!;
    public string Model { get; set; } = default!;
    public int Year { get; set; }
    public int Mileage { get; set; }

    public int EngineVolume { get; set; }

    public long Price { get; set; }
    public string PhoneNumber { get; set; } = default!;

    public string Description { get; set; } = default!;
    public AdvertisementStatus Status { get; set; } = AdvertisementStatus.Pending;
    public DocumentStatus DocumentStatus { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string SellerName { get; set; } = "Unknown";

    public bool Published { get; set; } = false;
    public List<string> Images { get; set; } = new(); // فقط URL ها

    public byte EngineHealth { get; set; }
    public byte SuspensionHealth { get; set; }
    public byte TireHealth { get; set; }
    public List<string> Features { get; set; } = new();
    public List<HistoryDto>? Histories { get; set; } = new();

    public ICollection<Comment>? Comments { get; set; } = new List<Comment>();

}

