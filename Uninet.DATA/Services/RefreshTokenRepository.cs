using Microsoft.Data.SqlClient;
using System;
using System.Linq;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;

namespace Uninet.DATA.Services
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly UninetContext _context;

        public RefreshTokenRepository(UninetContext context)
        {
            _context = context;
        }

        public void SaveOrUpdate(string token, Guid systemGuid, DateTime expiresAt)
        {
            var existing = _context.Set<ExternalServiceRefreshToken>()
                .FirstOrDefault(r => r.ExternalSystemGuid == systemGuid);

            if (existing != null)
            {
                existing.Token = token;
                existing.ExpiresAt = expiresAt;
                existing.CreatedAt = DateTime.UtcNow;
                _context.Update(existing);
            }
            else
            {
                var newToken = new ExternalServiceRefreshToken
                {
                    ExternalSystemGuid = systemGuid,
                    Token = token,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Add(newToken);
            }

            _context.SaveChanges();
        }

        public bool Exists(string token, out Guid systemGuid)
        {
            var existing = _context.Set<ExternalServiceRefreshToken>()
                .FirstOrDefault(r => r.Token == token);

            if (existing != null)
            {
                systemGuid = existing.ExternalSystemGuid;
                return true;
            }

            systemGuid = Guid.Empty;
            return false;
        }

        public void Delete(string token)
        {
            var tokenToDelete = _context.Set<ExternalServiceRefreshToken>()
                .FirstOrDefault(r => r.Token == token);

            if (tokenToDelete != null)
            {
                _context.Remove(tokenToDelete);
                _context.SaveChanges();
            }
        }
    }

}
