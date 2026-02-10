using Application.Interfaces;
using Domain.Entities;
using MediatR;

public class CreateAdvertisementCommandHandler
    : IRequestHandler<CreateAdvertisementCommand, int>
{
    private readonly IAdvertisementRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public CreateAdvertisementCommandHandler(
        IAdvertisementRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    public async Task<int> Handle(
        CreateAdvertisementCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();

        var ad = new Advertisement
        {
            Brand = request.Brand,
            Model = request.Model,
            Year = request.Year,
            Mileage = request.Mileage,
            DocumentStatus = request.DocumentStatus,
            Price = request.Price,
            PhoneNumber = request.PhoneNumber,
            Description = request.Description,
            Status = AdvertisementStatus.Pending,
            UserId = userId,
            Images = new List<AdvertisementImage>()
        };

 
        foreach (var base64DataUrl in request.ImagesBase64)
        {
            string base64 = base64DataUrl;
            if (base64.Contains(","))
                base64 = base64.Substring(base64.IndexOf(",") + 1);

            byte[] bytes = Convert.FromBase64String(base64);

            string uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}.png";
            string filePath = Path.Combine(uploadsFolder, fileName);

            await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);

            ad.Images.Add(new AdvertisementImage
            {
                Url = $"/Uploads/{fileName}"
            });
        }


        await _repository.CreateAsync(ad, cancellationToken);

        return ad.Id;
    }
 

}
