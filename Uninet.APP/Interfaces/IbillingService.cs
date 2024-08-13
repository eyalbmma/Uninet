using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public interface IbillingService
    {
        public Task<bool> ContinueFreeBilling(ContinueFreeInput continueFreeInput, int userId);
        public Task<UserCreditCardHolderList> ShowUserBillingDetails(UserDetails userDetails);
    }
}
