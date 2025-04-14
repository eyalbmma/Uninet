using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using System;

namespace Uninet.APP.Services
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _repository;

        public RefreshTokenService(IRefreshTokenRepository repository)
        {
            _repository = repository;
        }

        

        public void SaveOrUpdate(string token, Guid systemGuid, DateTime expiresAt)
        {
            _repository.SaveOrUpdate(token, systemGuid, expiresAt);
        }

        public bool Exists(string token, out Guid systemGuid)
        {
            return _repository.Exists(token, out systemGuid);
        }

        public void Delete(string token)
        {
            _repository.Delete(token);
        }
    }
}
