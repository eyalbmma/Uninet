using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.Domain.Models;

namespace Uninet.APP.Services
{
    public class billingServiceApp : IbillingService
    {

        private readonly IbillingServiceDataaccess _billingServiceDataaccess;
        public billingServiceApp(IbillingServiceDataaccess billingServiceDataaccess)
        {
            _billingServiceDataaccess = billingServiceDataaccess;
            //_loginRepository = loginRepository;
        }
        public Task<UserCreditCardHolderList> ShowUserBillingDetails(UserDetails userDetails)
        {
            return _billingServiceDataaccess.ShowUserBillingDetails(userDetails);
        }

        public async Task<bool> ContinueFreeBilling(ContinueFreeInput continueFreeInput, int userId)
        {
            try
            {
                return await _billingServiceDataaccess.ContinueFreeBilling(continueFreeInput, userId);
            }
            catch
            {
                return false;
            }
        }
    }
}
