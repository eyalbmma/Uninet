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

        

        public void SaveOrUpdate(string token, int systemId, DateTime expiresAt)
        {
            _repository.SaveOrUpdate(token, systemId, expiresAt);
        }

        public bool Exists(string token, out int systemId)
        {
            return _repository.Exists(token, out systemId);
        }

        public void Delete(string token)
        {
            _repository.Delete(token);
        }
    }
}
