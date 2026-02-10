using Application.Features.Contact.Commands;
using Infrastructure.Persistence;
using Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MotoX.Domain.Entities;

namespace Api.Controllers
{

    [ApiController]
    [Route("api/contact")]
    public class ContactController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IJwtTokenService _jwt;
        private readonly AppDbContext _db;
        private readonly EmailService _emailService;
        private readonly IMediator _mediator;
 
        public ContactController(
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwt, AppDbContext db, EmailService emailService, IMediator mediator)
        {
            _userManager = userManager;
            _jwt = jwt;
            _db = db;
            _emailService = emailService;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] CreateContactCommand command)
        {
            var id = await _mediator.Send(command);
            var response = new ApiResponse<int>
            {
                Success = true,
                Message = "پیام شما با موفقیت ارسال شد",
                Data = id
            };
            return Ok(response);
        }
    }
}
