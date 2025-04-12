using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void SaveOrUpdate(string token, int systemId, DateTime expiresAt)
        {
            var existing = _context.Set<ExternalServiceRefreshToken>()
                .FirstOrDefault(r => r.ExternalSystemId == systemId);

            if (existing != null)
            {
                existing.Token = token;
                existing.ExpiresAt = expiresAt;
                existing.CreatedAt = DateTime.UtcNow;
                _context.Update(existing);
            }
            else
            {
                var refreshToken = new ExternalServiceRefreshToken
                {
                    Token = token,
                    ExternalSystemId = systemId,
                    ExpiresAt = expiresAt,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Add(refreshToken);
            }

            _context.SaveChanges();
        }

        public bool Exists(string token, out int systemId)
        {
            var existing = _context.Set<ExternalServiceRefreshToken>().FirstOrDefault(r => r.Token == token);

            if (existing != null && existing.ExternalSystemId.HasValue)
            {
                systemId = existing.ExternalSystemId.Value;
                return true;
            }

            systemId = 0;
            return false;
        }

        public void Delete(string token)
        {
            var tokenToDelete = _context.Set<ExternalServiceRefreshToken>().FirstOrDefault(r => r.Token == token);
            if (tokenToDelete != null)
            {
                _context.Remove(tokenToDelete);
                _context.SaveChanges();
            }
        }
    }
}
