using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Entities.Comments;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

namespace Infrastructure.Repositories
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AppDbContext _context;
        private readonly string _uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");


        public AdvertisementRepository(AppDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_uploadFolder))
                Directory.CreateDirectory(_uploadFolder);
        }

        public async Task CreateAsync(Advertisement ad, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in ad.Images)
                {
                    await _context.AdvertisementImages.AddAsync(item, cancellationToken);
                }

                await _context.Advertisements.AddAsync(ad, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task NewComment(Comment comment, CancellationToken cancellationToken)
        {
            try
            {
                if (comment == null)
                {
                    new ArgumentNullException(nameof(comment));
                }
                await _context.Comments.AddAsync(comment, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

            }
            catch (Exception)
            {

                throw;
            }

        }


        public async Task<List<CommentDto>> GetCommentsAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {
                var ads = await _context.Comments
                .Where(w => w.AdvertisementId == Id).ToListAsync();

                var adsDto = ads.Select(b => new CommentDto
                {
                    Message = b.Message,
                    Id = b.Id,
                    Reply = b.Reply,
                    Username = b.Username

                }).ToList();

                return adsDto;
            }
            catch (Exception)
            {
                return new List<CommentDto>();
            }
            
        }

        public async Task<Advertisement?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.Advertisements
                .Where(w => w.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public async Task<List<AdvertisementDto>> GetAllAsync(int? Count, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Advertisements
                .Where(w => w.Published == true)
                .Include(a => a.Images)
                .Include(a => a.Histories)
                .AsQueryable();


                if (Count.HasValue)
                    query = query.Take(Count.Value);

                var ads = await query.ToListAsync(cancellationToken);

                return ads.Select(b => new AdvertisementDto
                {
                    Id = b.Id,
                    Brand = b.Brand,
                    Description = b.Description,
                    Images = b.Images.Select(img => img.Url).ToList(),
                    Mileage = b.Mileage,
                    Model = b.Model,
                    Price = b.Price,
                    Year = b.Year,
                    Published = b.Published,
                    Status = b.Status,
                    SellerName = _context.Users.Where(u => u.Id == b.UserId).Select(s => s.UserName).FirstOrDefault() ?? "Unknown",
                    DocumentStatus = b.DocumentStatus,
                    PhoneNumber = b.PhoneNumber,
                    CreatedAt = b.CreatedAt,
                    EngineHealth = b.EngineHealth,
                    Histories = b.Histories
                    .Select(h => new HistoryDto(
                        h.Description,
                        h.Date
                    ))
                    .ToList(),
                    SuspensionHealth = b.SuspensionHealth,
                    TireHealth = b.TireHealth,
                    Features = b.Features,
                }).ToList();
            }
            catch (Exception)
            {
                return new List<AdvertisementDto>(); 
            }
            
        }

        public async Task<List<AdvertisementDto>> GetAllAsyncAdmin(int? Count, CancellationToken cancellationToken)
        {
            try
            {
                var query = _context.Advertisements
                .Include(a => a.Images)
                .Include(a => a.Histories)
                .AsQueryable();


                if (Count.HasValue)
                    query = query.Take(Count.Value);

                var ads = await query.ToListAsync(cancellationToken);

                return ads.Select(b => new AdvertisementDto
                {
                    Id = b.Id,
                    Brand = b.Brand,
                    Description = b.Description,
                    Images = b.Images.Select(img => img.Url).ToList(),
                    Mileage = b.Mileage,
                    Model = b.Model,
                    Price = b.Price,
                    Year = b.Year,
                    Published = b.Published,
                    Status = b.Status,
                    SellerName = _context.Users.Where(u => u.Id == b.UserId).Select(s => s.UserName).FirstOrDefault() ?? "Unknown",
                    DocumentStatus = b.DocumentStatus,
                    PhoneNumber = b.PhoneNumber,
                    CreatedAt = b.CreatedAt,
                    EngineHealth = b.EngineHealth,
                    Histories = b.Histories
                    .Select(h => new HistoryDto(
                        h.Description,
                        h.Date
                    )).ToList(),
                    SuspensionHealth = b.SuspensionHealth,
                    TireHealth = b.TireHealth,
                    Features = b.Features,

                }).ToList();
            }
            catch (Exception)
            {
                return new List<AdvertisementDto>();    
            }
            
        }



        public async Task<bool> Reply(
             int id,
             string reply,
             CancellationToken cancellationToken)
        {
            try
            {
                var comments = await _context.Comments
                .FirstOrDefaultAsync(w => w.AdvertisementId == id, cancellationToken);

                if (comments == null) return false;


                comments.Reply = reply;

                _context.Comments.Update(comments);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async Task<bool> Approve(
          int id,
          HealthDto healthData,
          List<HistoryDto> historyData,
          CancellationToken cancellationToken)
        {
            try
            {
                var ad = await _context.Advertisements
                .Include(a => a.Histories)
                .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);

                if (ad == null) return false;

                ad.Published = true;
                ad.Status = AdvertisementStatus.Approved;

                ad.EngineHealth = (byte)healthData.Engine;
                ad.SuspensionHealth = (byte)healthData.Suspension;
                ad.TireHealth = (byte)healthData.Tires;

                if (ad.Histories != null && ad.Histories.Any())
                {
                    _context.VehicleHistory.RemoveRange(ad.Histories);
                }

                if (historyData != null && historyData.Any())
                {
                    ad.Histories = historyData.Select(h => new VehicleHistory
                    {
                        Description = h.Description,
                        Date = h.Date,
                        AdvertisementId = ad.Id
                    }).ToList();
                }

                _context.Advertisements.Update(ad);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async Task<bool> ApproveAndEditAsync(EditAdvertisementDto model, CancellationToken cancellationToken)
        {
            try
            {
                var ad = await _context.Advertisements
               .Include(a => a.Images)
               .Include(a => a.Histories)
               .FirstOrDefaultAsync(w => w.Id == model.Id, cancellationToken);

                if (ad == null) return false;

                ad.Brand = model.Brand;
                ad.Model = model.Model;
                ad.Year = model.Year;
                ad.Price = model.Price;
                ad.Mileage = model.Mileage;
                ad.Description = model.Description;

                ad.Published = true;
                ad.Status = AdvertisementStatus.Approved;

                if (model.Health != null)
                {
                    ad.EngineHealth = (byte)model.Health.Engine;
                    ad.SuspensionHealth = (byte)model.Health.Suspension;
                    ad.TireHealth = (byte)model.Health.Tires;
                }

                if (model.Images != null)
                {

                    var existingUrlsInRequest = model.Images
                        .Where(img => !img.Contains("base64"))
                        .ToList();

                    var imagesToDelete = ad.Images
                        .Where(dbImg => !existingUrlsInRequest.Any(reqUrl => reqUrl.Contains(Path.GetFileName(dbImg.Url))))
                        .ToList();

                    foreach (var img in imagesToDelete)
                    {
                        try
                        {
                            var fileName = Path.GetFileName(img.Url);
                            var filePath = Path.Combine(_uploadFolder, fileName);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                            }
                        }
                        catch
                        {
                        }

                        _context.AdvertisementImages.Remove(img);
                    }

                    var newImagesBase64 = model.Images
                        .Where(img => img.Contains("base64"))
                        .ToList();

                    foreach (var base64Str in newImagesBase64)
                    {
                        try
                        {
                            var base64Data = base64Str.Substring(base64Str.IndexOf(",") + 1);
                            var imageBytes = Convert.FromBase64String(base64Data);

                            string extension = ".jpg";
                            if (base64Str.Contains("image/png")) extension = ".png";
                            else if (base64Str.Contains("image/jpeg")) extension = ".jpg";
                            else if (base64Str.Contains("image/webp")) extension = ".webp";

                            var fileName = $"{Guid.NewGuid()}{extension}";
                            var filePath = Path.Combine(_uploadFolder, fileName);

                            await File.WriteAllBytesAsync(filePath, imageBytes, cancellationToken);

                            var fileUrl = $"/Uploads/{fileName}";

                            await _context.AdvertisementImages.AddAsync(new AdvertisementImage
                            {
                                Url = fileUrl,
                                AdvertisementId = ad.Id
                            }, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error saving image: {ex.Message}");
                        }
                    }
                }

                if (ad.Histories != null && ad.Histories.Any())
                {
                    _context.VehicleHistory.RemoveRange(ad.Histories);
                }

                if (model.History != null && model.History.Any())
                {
                    var newHistories = model.History.Select(h => new VehicleHistory
                    {
                        Description = h.Description,
                        Date = h.Date,
                        AdvertisementId = ad.Id
                    }).ToList();

                    await _context.VehicleHistory.AddRangeAsync(newHistories, cancellationToken);
                }

                if (model.Features != null)
                {
                    ad.Features = model.Features;
                }
                else
                {
                    ad.Features = new List<string>();
                }

                _context.Advertisements.Update(ad);
                await _context.SaveChangesAsync(cancellationToken);

                return true;
            }
            catch (Exception)
            {

                return false;
            }
           
        }


        public async Task<bool> Reject(int adId, CancellationToken cancellationToken)
        {
            try
            {
                var query = await _context.Advertisements
               .Where(w => w.Id == adId).FirstOrDefaultAsync(cancellationToken);

                if (query == null) return false;

                query.Published = false;
                query.Status = AdvertisementStatus.Rejected;

                _context.Advertisements.Update(query);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
           

        }

        public async Task<bool> Delete(int adId, CancellationToken cancellationToken)
        {
            try
            {
                var query = await _context.Advertisements
                .Where(w => w.Id == adId).FirstOrDefaultAsync(cancellationToken);

                if (query == null) return false;


                _context.Advertisements.Remove(query);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            

        }

        public async Task<AdvertisementDto> GetAsync(int Id, CancellationToken cancellationToken)
        {
            try
            {
                var query = await _context.Advertisements
                .Where(w => w.Id == Id && w.Published)
                .FirstOrDefaultAsync(cancellationToken);


                if (query == null)
                    return new AdvertisementDto();

                var queryImages = await _context.AdvertisementImages
                    .Where(w => w.AdvertisementId == Id)
                    .Select(img => img.Url)
                    .ToListAsync(cancellationToken);

                var histories = await _context.VehicleHistory
                    .Where(h => h.AdvertisementId == Id)
                    .Select(h => new HistoryDto(
                        h.Description,
                        h.Date
                    ))
                    .ToListAsync(cancellationToken);

                return new AdvertisementDto
                {
                    Id = query.Id,
                    Brand = query.Brand,
                    Model = query.Model,
                    Year = query.Year,
                    Mileage = query.Mileage,
                    Price = query.Price,
                    PhoneNumber = query.PhoneNumber,
                    Description = query.Description,
                    Status = query.Status,
                    DocumentStatus = query.DocumentStatus,
                    CreatedAt = query.CreatedAt,
                    Published = query.Published,
                    EngineHealth = query.EngineHealth,
                    SuspensionHealth = query.SuspensionHealth,
                    TireHealth = query.TireHealth,
                    Features = query.Features,

                    SellerName = _context.Users
                        .Where(u => u.Id == query.UserId)
                        .Select(u => u.UserName)
                        .FirstOrDefault() ?? "Unknown",

                    Images = queryImages,
                    Histories = histories
                };
            }
            catch (Exception)
            {
                return null;
            }
            
        }


    }
}
