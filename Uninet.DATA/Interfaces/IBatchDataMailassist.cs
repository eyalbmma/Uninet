using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Models;

namespace Uninet.DATA.Interfaces
{
    public interface IBatchDataMailassist
    {
        Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To, int Templateid, int lang, RequestedMailObject InputMailDetails = null,string Userid="",string JsonDocumentid="");
    }
}
