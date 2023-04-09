using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Services;

namespace Uninet.APP.Interfaces
{
    public  interface IMailassist 
    {
        Task<bool> sendsmtpmail(string subject, string body, string From, string To);
       

    }
}
