using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Receipts;
using MediatR;

public class UploadImageCommandHandler
    : IRequestHandler<UploadImageCommand, int>
{
    private readonly IAdvertisementRepository _repository;
    private readonly ICurrentUserService _currentUser;

    public UploadImageCommandHandler(
        IAdvertisementRepository repository,
        ICurrentUserService currentUser)
    {
        _repository = repository;
        _currentUser = currentUser;
    }
    public async Task<int> Handle(
        UploadImageCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException();



        string base64 = request.ImagesBase64;
            if (base64.Contains(","))
                base64 = base64.Substring(base64.IndexOf(",") + 1);

            byte[] bytes = Convert.FromBase64String(base64);

            string uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Receipts");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = $"{Guid.NewGuid()}.png";
            string filePath = Path.Combine(uploadsFolder, fileName);

        var receipt = new Receipts()
        {
            UserId = userId,
            Price = request.Price,
            Url = filePath,
            CreatedAt = DateTime.Now
        };
            

        await File.WriteAllBytesAsync(filePath, bytes, cancellationToken);
        await _repository.UploadReceipt(receipt, cancellationToken);

        return receipt.Id;
    }
 

}
