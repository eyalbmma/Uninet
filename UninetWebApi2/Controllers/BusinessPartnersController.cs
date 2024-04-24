using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessPartnersController : ControllerBase
    {
        
        private readonly EnumRepository _enumRepository;
        private readonly IMailassist _mailasist;
        //private readonly IRepository<UninetContext> _repository;
        private IUninetOutPutAppService _uninetOutPutAppService;
        public BusinessPartnersController(IMailassist mailassist, EnumRepository enumRepository, IUninetOutPutAppService uninetOutPutAppService)
        {
            

             _enumRepository = enumRepository;
            _mailasist = mailassist;
            _uninetOutPutAppService = uninetOutPutAppService;   
        }



        [Authorize]
        [HttpPost("SendEmail")]
       
        public async Task<ActionResult> SendEmail([FromBody] SendEmailRequest sendEmailRequest)
        {
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var ObjTemplateparam = new { TemplateId = 3, Lang = 2 };
            //var res = await _repository.ExecuteGetSPAsync<OTPHtmlBody>(ConstUninetStoredprocedure.SP_GetHtmlBody, ObjTemplateparam);


            var res=await _mailasist.BusinessPartnerSendEmail( sendEmailRequest, userId, sendEmailRequest.Lang);

            
           
            return Ok(res.result);
        }

        // Endpoint to get business partners by filter type
        [Authorize]
       
        [HttpGet("GetBusinessPartnersByFilter")]
        public async Task<ActionResult> GetBusinessPartnersByFilter([FromQuery] int filterType,int subCopmanyId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Check if the provided filterType is valid
            if (!Enum.IsDefined(typeof(FilterType), filterType))
            {
                return BadRequest("Invalid filter type.");
            }

            // Convert the integer value to the corresponding FilterType enum value
            FilterType selectedFilter = (FilterType)filterType;

            switch (selectedFilter)
            {
                case FilterType.Supplier:
                    // Logic for Supplier filter type
                    var res = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType,Convert.ToInt32(userId), subCopmanyId);
                    return Ok(res);

                case FilterType.Client:
                    var res2 = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId);
                    return Ok(res2);

                case FilterType.Both:
                    var res3 = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId);
                    return Ok(res3);

                default:
                    // Invalid filter type
                    return BadRequest("Filter type does not match.");
            }

        }
    }
}
