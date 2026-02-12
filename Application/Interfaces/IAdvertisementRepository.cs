
using Application.DTOs;
using Domain.Entities;
using Domain.Entities.Comments;

namespace Application.Interfaces;

public interface IAdvertisementRepository

{
    Task CreateAsync(
        Advertisement advertisement,
        CancellationToken cancellationToken);

    Task<Advertisement?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<List<AdvertisementDto>> GetAllAsync(int? Count,CancellationToken cancellationToken);

    
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
}
