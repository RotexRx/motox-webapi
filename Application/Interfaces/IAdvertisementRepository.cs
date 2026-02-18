
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Comments;
using Domain.Entities.Receipts;

namespace Application.Interfaces;

public interface IAdvertisementRepository

{
    Task CreateAsync(
        Advertisement advertisement,
        CancellationToken cancellationToken);

    Task<Advertisement?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<List<AdvertisementDto>> GetAllAsync(
            CancellationToken cancellationToken,
            int page,
            int pageSize,
            string? brand);


    Task<AdvertisementDto> GetAsync(int Id, CancellationToken cancellationToken);

 
    Task<List<AdvertisementDto>> GetAllAsyncAdmin(int? Count, CancellationToken cancellationToken);

    Task<bool> Approve(
            int id,
            HealthDto healthData,
            List<HistoryDto> historyData,
            CancellationToken cancellationToken);

    Task<bool> Reject(int adId, CancellationToken cancellationToken);
    Task<bool> Delete(int adId, CancellationToken cancellationToken);
    Task<bool> ApproveAndEditAsync(EditAdvertisementDto model, CancellationToken cancellationToken);

    Task UploadReceipt(Receipts Receipt, CancellationToken cancellationToken);

    Task<Receipts> GetReceipt(int Id, CancellationToken cancellationToken);
    Task<List<Receipts>> GetAllReceipts(CancellationToken cancellationToken);

}
