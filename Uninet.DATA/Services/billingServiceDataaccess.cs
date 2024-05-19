using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;

namespace Uninet.DATA.Services
{
    public class billingServiceDataaccess: IbillingServiceDataaccess
    {
        private readonly IRepository<UninetContext> _repository;
        public billingServiceDataaccess(IRepository<UninetContext> repository, IConfiguration configuration)//, IloginRepository loginRepository
        {
            _repository = repository;
            

        }

        public async Task<UserCreditCardHolderList> ShowUserBillingDetails(UserDetails userDetails)
        {
            try
            {
                // Fetch the list of UserCreditCardHolderTable objects based on UserId and BusinessId
                var creditCardHolders = await _repository.GetListOfObjectsAsync<UserCreditCardHolder>(
                    x => x.UserId == userDetails.UserId && x.BusinessId == userDetails.BusinessId);

                // Map the fetched objects to a list of CreditCardHolderUser objects
                var creditCardHolderUsers = creditCardHolders.Select(c => new CreditCardHolderUser
                {
                    CreditcardLogo = c.CreditcardLogo,
                    CardName = c.CardName,
                    Last4Digits = c.lastdigits,
                    ExpirationDate = c.expirationdate
                }).ToList();

                // Create an instance of UserCreditCardHolderList and populate it
                var userCreditCardHolderList = new UserCreditCardHolderList
                {
                    CreditCardHolders = creditCardHolderUsers
                };

                // Return the populated UserCreditCardHolderList
                return userCreditCardHolderList;
            }
            catch (Exception ex)
            {
                // Handle the exception as needed
                // For example, log the exception and return an empty list or rethrow the exception
                // LogException(ex);
                return new UserCreditCardHolderList();
            }
        }


    }
}
