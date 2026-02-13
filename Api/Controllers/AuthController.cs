using Api.Contracts.Auth;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using MotoX.Domain.Entities;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenService _jwt;
    private readonly AppDbContext _db;
    private readonly EmailService _emailService;


    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenService jwt, AppDbContext db, EmailService emailService)
    {
        _userManager = userManager;
        _jwt = jwt;
        _db = db;
        _emailService = emailService;
    }

    [EnableRateLimiting("auth-policy")]
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(Api.Contracts.Auth.RegisterRequest req)
    {
        var existingUser = await _userManager.FindByEmailAsync(req.Email);

        if (existingUser != null && !existingUser.EmailConfirmed)
        {
            return await SendVerificationCode(existingUser);
        }

        var user = new ApplicationUser
        {
            UserName = req.Username,
            Email = req.Email,
            PhoneNumber = req.Number,
            CreatedAt = DateTime.UtcNow 
        };

        var result = await _userManager.CreateAsync(user, req.Password);

        if (!result.Succeeded)
        {
            string customMessage = "خطا در ثبت نام: ";

            var error = result.Errors.FirstOrDefault();
            if (error != null)
            {
                customMessage = error.Code switch
                {
                    "DuplicateEmail" => "این ایمیل قبلاً ثبت شده است",
                    "DuplicateUserName" => "این نام کاربری توسط شخص دیگری انتخاب شده است",
                    "PasswordTooShort" => "رمز عبور بسیار کوتاه است",
                    "PasswordRequiresDigit" => "رمز عبور باید حداقل شامل یک عدد باشد",
                    _ => error.Description
                };
            }

            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = customMessage,
                Data = result.Errors
            });
        }

        return await SendVerificationCode(user);
    }

    private async Task<IActionResult> SendVerificationCode(ApplicationUser user)
    {
        var code = new Random().Next(10000, 99999).ToString();
         
        var oldCodes = _db.EmailVerificationCodes.Where(x => x.UserId == user.Id);
        _db.EmailVerificationCodes.RemoveRange(oldCodes);

        var entity = new EmailVerificationCode
        {
            Code = code,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddMinutes(10)
        };

        _db.EmailVerificationCodes.Add(entity);
        await _db.SaveChangesAsync();

        await _emailService.SendEmailAsync(user.Email, "کد تایید حساب", $"کد شما: {code}");

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "کد تایید به ایمیل ارسال شد",
            Data = new { email = user.Email }
        });
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyCodeRequest req)
    {
        var codeEntity = await _db.EmailVerificationCodes
            .Include(u => u.User)
            .FirstOrDefaultAsync(x => x.Code == req.Code && x.Expires > DateTime.UtcNow);

        if (codeEntity == null)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "کد نامعتبر یا منقضی شده" });

        codeEntity.User.EmailConfirmed = true;
        _db.EmailVerificationCodes.Remove(codeEntity);
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object> { Success = true, Message = "حساب شما با موفقیت فعال شد" });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(Api.Contracts.Auth.LoginRequest req)
    {
        var user = await _userManager.FindByNameAsync(req.Username);
        if (user == null)
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "نام کاربری یا رمز اشتباه است" });

        var valid = await _userManager.CheckPasswordAsync(user, req.Password);
        if (!valid)
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "نام کاربری یا رمز اشتباه است" });

        var locked = user.Suspended;
        if (locked)
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "حساب شما مسدود است" });

        var confirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!confirmed)
        {
            await SendVerificationCode(user);

            return Ok(new ApiResponse<object>
            {
                Success = false,
                Message = "ایمیل شما تایید نشده است. کد جدید ارسال شد.",
                Data = new { requiresVerification = true, email = user.Email }  
            });
        }

        var tokens = await _jwt.GenerateTokens(user);

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "ورود موفق",
            Data = new { accessToken = tokens.accessToken, refreshToken = tokens.refreshToken }
        });
    }


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
    {
        try
        {
            var newAccessToken = await _jwt.RefreshAccessToken(req.RefreshToken);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "توکن جدید ساخته شد",
                Data = new { accessToken = newAccessToken }
            });
        }
        catch
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "توکن نامعتبر است",
                Data = null
            });
        }
    }


    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] Api.Contracts.Auth.ForgotPasswordRequest req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null)
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "اگر ایمیل موجود باشد، کد ارسال می‌شود",
                Data = null
            });

        var code = new Random().Next(10000, 99999).ToString();
        var entity = new ForgotPasswordCode
        {
            Code = code,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddMinutes(5)
        };

        _db.ForgotPasswordCodes.Add(entity);
        await _db.SaveChangesAsync();
        await _emailService.SendEmailAsync(user.Email, "بازنشانی رمز عبور", $"کد شما: <b>{code}</b>");

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "کد به ایمیل شما ارسال شد",
            Data = null
        });
    }



    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] Api.Contracts.Auth.ResetPasswordRequest req)
    {
        var codeEntity = await _db.ForgotPasswordCodes
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Code == req.Code && !x.Used && x.Expires > DateTime.UtcNow);

        if (codeEntity == null)
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "کد نامعتبر یا منقضی شده است",
                Data = null
            });

        var resetResult = await _userManager.ResetPasswordAsync(
            codeEntity.User,
            await _userManager.GeneratePasswordResetTokenAsync(codeEntity.User),
            req.NewPassword
        );

        if (!resetResult.Succeeded)
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "خطا در تغییر رمز عبور",
                Data = resetResult.Errors
            });

        codeEntity.Used = true;
        await _db.SaveChangesAsync();

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "رمز عبور با موفقیت تغییر کرد",
            Data = null
        });
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyCode([FromBody] Api.Contracts.Auth.VerifyCodeRequest req)
    {
        var codeEntity = await _db.ForgotPasswordCodes
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Code == req.Code && !x.Used && x.Expires > DateTime.UtcNow);

        if (codeEntity == null)
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "کد نامعتبر یا منقضی شده است",
                Data = null
            });

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "کد معتبر است",
            Data = null
        });
    }


     


}


