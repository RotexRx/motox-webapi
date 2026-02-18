using Application.DTOs;
using Application.Interfaces;
using Domain.Entities.Receipts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;


namespace Api.Controllers
{
    [ApiController]
    [Route("api/advertisement")]
    public class AdvertisementController : Controller
    {

        private readonly IMediator _mediator;
        private readonly IAdvertisementRepository _repo;

        public AdvertisementController(IMediator mediator, IAdvertisementRepository repo)
        {
            _mediator = mediator;
            _repo = repo;
        }
         

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateAdvertisement([FromBody] CreateAdvertisementCommand command)
        {
            var id = await _mediator.Send(command);
            var response = new ApiResponse<int>
            {
                Success = true,
                Message = "اگهی با موفقیت ساخته شد",
                Data = id
            };
            return Ok(response);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromBody] UploadImageCommand req)
        {

            var id = await _mediator.Send(req);
            var response = new ApiResponse<int>
            {
                Success = true,
                Message = "رسید با موفقیت اپلود شد",
            };
            return Ok(response);
        }

        [HttpGet("receipt-image/{fileName}")]
        public IActionResult GetImageReceipt(string fileName)
        {
            var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Receipts");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "فایل پیدا نشد"
                });
            }

            var mimeType = "image/png";
            return PhysicalFile(filePath, mimeType);
        }

        [HttpGet("receipt/{Id}")]
        public async Task<IActionResult> GetReceipt(
                [FromServices] IAdvertisementRepository repo,
                [FromRoute] int Id,
                CancellationToken ct)
        {
            var receipt = await repo.GetReceipt(Id, ct);
            var response = new ApiResponse<Receipts>
            {
                Success = true,
                Data = receipt
            };
            return Ok(response);
        }

        [HttpGet("receipts")]
        public async Task<IActionResult> GetReceipt(
                [FromServices] IAdvertisementRepository repo,
                CancellationToken ct)
        {
            var receipts = await repo.GetAllReceipts( ct);
            var response = new ApiResponse<List<Receipts>>
            {
                Success = true,
                Data = receipts
            };
            return Ok(response);
        }


        [HttpGet("list")]
        public async Task<IActionResult> GetAdvertisements(
            [FromServices] IAdvertisementRepository repo, CancellationToken ct,
            [FromQuery] int page = 1,     
            [FromQuery] int pageSize = 6,
            [FromQuery] string? brand = null 
            )
        {
       
            var ads = await repo.GetAllAsync(ct,page, pageSize, brand);

            var response = new ApiResponse<List<AdvertisementDto>>
            {
                Success = true,
                Data = ads
            };
            return Ok(response);
        }



        [HttpGet("get-single/{Id}")]
        public async Task<IActionResult> GetSingleAdvertisement(
            [FromServices] IAdvertisementRepository repo,
            [FromRoute] int Id,
            CancellationToken ct)
        {

            var ad = await repo.GetAsync(Id, ct);
            var response = new ApiResponse<AdvertisementDto>
            {
                Success = true,
                Data = ad
            };
            return Ok(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("get")]
        public async Task<IActionResult> GetAdvertisementsAdmin(
            [FromServices] IAdvertisementRepository repo,
            [FromQuery] int? count,
            CancellationToken ct)
        {
            var ads = await repo.GetAllAsyncAdmin(count, ct);
            var response = new ApiResponse<List<AdvertisementDto>>
            {
                Success = true,
                Data = ads
            };
            return Ok(response);
        }


        [HttpGet("image/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var uploadsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads");
            var filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new ApiResponse<string>
                {
                    Success = false,
                    Message = "فایل پیدا نشد"
                });
            }

            var mimeType = "image/png";
            return PhysicalFile(filePath, mimeType);
        }

        


        [Authorize(Roles = "Admin")]
        [HttpPost("approve/{id}")]
        public async Task<IActionResult> ApproveAdvertisement(int id, [FromBody] EditAdvertisementDto req, CancellationToken ct)
        {
            req.Id = id;

            if (!ModelState.IsValid)
                return BadRequest(new { Success = false, Message = "اطلاعات نامعتبر است" });

            var result = await _repo.ApproveAndEditAsync(req, ct);

            if (result)
            {
                return Ok(new { Success = true, Message = "آگهی با موفقیت ویرایش و منتشر شد" });
            }

            return BadRequest(new { Success = false, Message = "آگهی یافت نشد یا عملیات شکست خورد" });
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("reject")]
        public async Task<IActionResult> Reject([FromBody] RejectedCommand command)
        {
            var success = await _mediator.Send(command);
            if (success)
            {
                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "وضعیت انتشار با موفقیت تغییر کرد",
                    Data = true
                };
                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<bool>
                {
                    Success = false,
                    Message = "خطا در تغییر وضعیت انتشار",
                    Data = false
                };
                return BadRequest(response);
            }

        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteCommand command)
        {
            var success = await _mediator.Send(command);
            if (success)
            {
                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "با موفقیت حذف شد",
                    Data = true
                };
                return Ok(response);
            }
            else
            {
                var response = new ApiResponse<bool>
                {
                    Success = false,
                    Message = "خطا در حذف",
                    Data = false
                };
                return BadRequest(response);
            }

        }
    }
}
