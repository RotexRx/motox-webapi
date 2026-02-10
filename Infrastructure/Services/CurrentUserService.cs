using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// بازمی‌گرداند شناسه کاربر جاری.
        /// این مقدار می‌تواند از claim "sub" یا "NameIdentifier" گرفته شود.
        /// </summary>
        public string? UserId
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user == null || !user.Identity?.IsAuthenticated == true)
                    return null;

                // اول از claim "sub" بررسی می‌کنیم (JWT استاندارد)
                var subClaim = user.FindFirst("sub")?.Value;
                if (!string.IsNullOrEmpty(subClaim))
                    return subClaim;

                // بعد از ClaimTypes.NameIdentifier بررسی می‌کنیم (ASP.NET Identity / WS-Federation)
                var nameIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(nameIdClaim))
                    return nameIdClaim;

                // در غیر این صورت null برمی‌گردانیم
                return null;
            }
        }

        /// <summary>
        /// بررسی می‌کند کاربر وارد شده است یا نه.
        /// </summary>
        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

        /// <summary>
        /// بازمی‌گرداند نام کاربر (اگر موجود باشد).
        /// </summary>
        public string? UserName => _httpContextAccessor.HttpContext?.User?.Identity?.Name;
    }
}
