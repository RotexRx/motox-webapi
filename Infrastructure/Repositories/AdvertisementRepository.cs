using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
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
            foreach (var item in ad.Images)
            {
                await _context.AdvertisementImages.AddAsync(item, cancellationToken);
            }

            await _context.Advertisements.AddAsync(ad, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<Advertisement?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            return await _context.Advertisements
                .Where(w => w.Id == id).FirstOrDefaultAsync();
        }

        public async Task<List<AdvertisementDto>> GetAllAsync(int? Count, CancellationToken cancellationToken)
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

            }).ToList();
        }

        public async Task<List<AdvertisementDto>> GetAllAsyncAdmin(int? Count, CancellationToken cancellationToken)
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

            }).ToList();
        }

        public async Task<bool> Approve(
          int id,
          HealthDto healthData,
          List<HistoryDto> historyData,
          CancellationToken cancellationToken)
        {
            var ad = await _context.Advertisements
                .Include(a => a.Histories) // لود کردن تاریخچه‌ها
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

        public async Task<bool> ApproveAndEditAsync(EditAdvertisementDto model, CancellationToken cancellationToken)
        {
            // 1. پیدا کردن آگهی شامل عکس‌ها و تاریخچه‌ها
            var ad = await _context.Advertisements
                .Include(a => a.Images)
                .Include(a => a.Histories)
                .FirstOrDefaultAsync(w => w.Id == model.Id, cancellationToken);

            if (ad == null) return false;

            // 2. بروزرسانی فیلدهای متنی و عددی
            ad.Brand = model.Brand;
            ad.Model = model.Model;
            ad.Year = model.Year;
            ad.Price = model.Price;
            ad.Mileage = model.Mileage;
            ad.Description = model.Description;

            // تغییر وضعیت به تایید شده
            ad.Published = true;
            ad.Status = AdvertisementStatus.Approved;

            // 3. بروزرسانی سلامت فنی
            if (model.Health != null)
            {
                ad.EngineHealth = (byte)model.Health.Engine;
                ad.SuspensionHealth = (byte)model.Health.Suspension;
                ad.TireHealth = (byte)model.Health.Tires;
            }

            // ==========================================================
            // 4. مدیریت پیشرفته تصاویر (حذف، نگهداری، افزودن جدید)
            // ==========================================================

            if (model.Images != null)
            {
                // الف) حذف تصاویری که در لیست جدید نیستند
                // لیست URLهایی که کلاینت فرستاده و عکس قدیمی هستند (Base64 نیستند)
                var existingUrlsInRequest = model.Images
                    .Where(img => !img.Contains("base64"))
                    .ToList();

                // عکس‌هایی که در دیتابیس هستند اما در لیست ارسالی کلاینت نیستند -> باید حذف شوند
                var imagesToDelete = ad.Images
                    .Where(dbImg => !existingUrlsInRequest.Any(reqUrl => reqUrl.Contains(Path.GetFileName(dbImg.Url))))
                    .ToList();

                foreach (var img in imagesToDelete)
                {
                    // 1. حذف فایل فیزیکی
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
                        // لاگ کردن خطا یا نادیده گرفتن در صورت عدم وجود فایل
                    }

                    // 2. حذف از دیتابیس
                    _context.AdvertisementImages.Remove(img);
                }

                // ب) افزودن تصاویر جدید (Base64)
                var newImagesBase64 = model.Images
                    .Where(img => img.Contains("base64"))
                    .ToList();

                foreach (var base64Str in newImagesBase64)
                {
                    try
                    {
                        // تشخیص فرمت و تبدیل Base64 به بایت
                        var base64Data = base64Str.Substring(base64Str.IndexOf(",") + 1);
                        var imageBytes = Convert.FromBase64String(base64Data);

                        // تعیین پسوند فایل
                        string extension = ".jpg";
                        if (base64Str.Contains("image/png")) extension = ".png";
                        else if (base64Str.Contains("image/jpeg")) extension = ".jpg";
                        else if (base64Str.Contains("image/webp")) extension = ".webp";

                        // ایجاد نام یکتا
                        var fileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(_uploadFolder, fileName);

                        // ذخیره فایل در دیسک
                        await File.WriteAllBytesAsync(filePath, imageBytes, cancellationToken);

                        // ذخیره در دیتابیس
                        // فرض بر این است که Url به صورت نسبی یا کامل ذخیره می‌شود (مثلا: /Uploads/filename.jpg)
                        // در اینجا فرض کردیم آدرس نسبی ذخیره می‌شود تا با فرانت هماهنگ باشد
                        var fileUrl = $"/Uploads/{fileName}"; // یا آدرس کامل سرور بسته به تنظیمات شما

                        await _context.AdvertisementImages.AddAsync(new AdvertisementImage
                        {
                            Url = fileUrl,
                            AdvertisementId = ad.Id
                        }, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        // هندل کردن خطا در صورت مشکل در آپلود عکس خاص
                        Console.WriteLine($"Error saving image: {ex.Message}");
                    }
                }
            }

            // ==========================================================

            // 5. مدیریت تاریخچه (Recreate strategy - حذف همه و ساخت مجدد ساده‌ترین راه برای لیست‌های کوچک است)
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

            // 6. ذخیره نهایی در دیتابیس
            _context.Advertisements.Update(ad);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }


        public async Task<bool> Reject(int adId, CancellationToken cancellationToken)
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

        public async Task<bool> Delete(int adId, CancellationToken cancellationToken)
        {

            var query = await _context.Advertisements
                .Where(w => w.Id == adId).FirstOrDefaultAsync(cancellationToken);

            if (query == null) return false;


            _context.Advertisements.Remove(query);
            await _context.SaveChangesAsync(cancellationToken);
            return true;

        }

        public async Task<AdvertisementDto> GetAsync(int Id, CancellationToken cancellationToken)
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

                SellerName = _context.Users
                    .Where(u => u.Id == query.UserId)
                    .Select(u => u.UserName)
                    .FirstOrDefault() ?? "Unknown",

                Images = queryImages,
                Histories = histories
            };
        }

    }
}
