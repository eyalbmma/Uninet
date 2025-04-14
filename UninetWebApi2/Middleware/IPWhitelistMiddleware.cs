using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uninet.DATA.Services.MultipleContext;

namespace UninetWebApi2.Middleware
{
    public class IPWhitelistMiddleware : IMiddleware
    {
        private readonly UninetContext _context;

        public IPWhitelistMiddleware(UninetContext context)
        {
            _context = context;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Only apply to specific routes
            if (path != "/api/auth/registerinit" && path != "/api/auth/loginwithsecret")
            {
                await next(context);
                return;
            }

            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            Debug.WriteLine($"[IPMiddleware] Request to {path} from {remoteIp}");

            if (remoteIp == "::1" || remoteIp == "127.0.0.1")
            {
                await next(context);
                return;
            }

            var allowedIps = _context.ExternalSystem
                .Where(e => e.IsActive && e.AllowedIP != null)
                .Select(e => e.AllowedIP)
                .ToList();

            if (!allowedIps.Contains(remoteIp))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Forbidden: Your IP is not whitelisted.");
                return;
            }

            await next(context);
        }

    }
}
