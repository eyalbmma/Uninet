using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Services;
using Uninet.Domain.Models;

namespace Uninet.APP.Interfaces
{
    public  interface IMailassist 
    {
        Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To, int TemplateId, int Lang,string encryptedUserId = "", string Name = null);



    }
}
