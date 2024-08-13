using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services;
using Uninet.Domain.Models;
using Uninet.Domain.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.StoredProcedures.Responses;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.Entities;
using Twilio.Rest.Api.V2010.Account.Usage.Record;

namespace Uninet.APP.Services
{
    public class Mailassist: IMailassist
    {

        readonly IDataMailassist _dataMailassist = null;
       
        private readonly IRepository<UninetContext> _repository;
        public Mailassist(IDataMailassist dataMailassist, IRepository<UninetContext> repository)
        {
            // _logger = logger;
            _dataMailassist = dataMailassist;
            _repository = repository;
            
        }

  
        public async Task<List<SendEmailResponse>> BusinessPartnerSendEmail(List<SendEmailRequest> sendEmailRequest, string userId)
        {
            try
            {
                //var MainCompanyIdObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == 627);
                //var ObjTemplateparam = new { TemplateId = 3, Lang = 2 };
                //var res = await _repository.ExecuteGetSPAsync<OTPHtmlBody>(ConstUninetStoredprocedure.SP_GetHtmlBody, ObjTemplateparam);
                return await _dataMailassist.BusinessPartnerSendEmail(sendEmailRequest, userId);
            }
            catch (Exception ex) { return null; }

        }


        public async Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From ,string To,int TemplateId,int Lang, string encryptedUserId = null, string Name = null)
        {
            try
            {
                return await _dataMailassist.sendsmtpmail(subject, From, To, TemplateId, Lang,null,null, encryptedUserId, Name);
               // Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To, int Templateid, int lang, RequestedMailObject InputMailDetails = null, string username = null, string encryptedUserId = "");
            }
            catch (Exception ex)
            {
               // Loger.Writetolog("current function sendsmtpmail" + ex.Message + ex.InnerException);
                return null;
            }


        }

       



    


    }
}
