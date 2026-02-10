using Domain.Entities;

public class AdvertisementImage
{
    public int Id { get; set; }

    public string Url { get; set; } = default!;

    public int AdvertisementId { get; set; }
    public Advertisement Advertisement { get; set; } = default!;
}
