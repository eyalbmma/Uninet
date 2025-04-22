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
using UninetWebApi2.Helpers;

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
        public async Task<ActionResult> SendEmail([FromBody] List<SendEmailRequest> sendEmailRequests, int Lang,int MainCompanyid)
        {
            var context = UserContextHelper.GetUserContext(User);

            int? userId = null;
            Guid? systemGuid = null;

            if (context.systemType == "adminUser")
            {
                userId = context.userId.Value; // שמירת userId
            }
            else if (context.systemType == "externalSystem")
            {
                return Unauthorized("This endpoint is for UI users only. Please login via the user interface.");
            }
            else
            {
                return Unauthorized("Invalid user context.");
            }



            // Call the modified BusinessPartnerSendEmail with a list
            var results = await _mailasist.BusinessPartnerSendEmail(sendEmailRequests, userId.ToString(), Lang, MainCompanyid);

            return Ok(results);
        }



        [Authorize]

        [HttpGet("ShortVersionGetBusinessPartnersByFilter")]
        public async Task<ActionResult> ShortVersionGetBusinessPartnersByFilter([FromQuery] int filterType, int subCopmanyId, int pageNumber, int pageSize,int Lang)
        {
            var context = UserContextHelper.GetUserContext(User);

            int? userId = null;
            Guid? systemGuid = null;

            if (context.systemType == "adminUser")
            {
                userId = context.userId.Value; // שמירת userId
            }
            else if (context.systemType == "externalSystem")
            {
                return Unauthorized("This endpoint is for UI users only. Please login via the user interface.");
            }
            else
            {
                return Unauthorized("Invalid user context.");
            }

            // Check if the provided filterType is valid
            if (!Enum.IsDefined(typeof(FilterType), filterType))
            {
                return BadRequest("Invalid filter type.");
            }

            // Convert the integer value to the corresponding FilterType enum value
            FilterType selectedFilter = (FilterType)filterType;

            switch (selectedFilter)
            {
                case FilterType.ShortversionSuplier:
                    // Logic for Supplier filter type
                    var res = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId, pageNumber, pageSize, Lang);
                    return Ok(res);

                //case FilterType.Client:
                //    var res2 = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId, pageNumber, pageSize);
                //    return Ok(res2);

                //case FilterType.Both:
                //    var res3 = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId, pageNumber, pageSize);
                //    return Ok(res3);

                default:
                    // Invalid filter type
                    return BadRequest("Filter type does not match.");
            }
        }

            // Endpoint to get business partners by filter type
            [Authorize]
       
        [HttpGet("GetBusinessPartnersByFilter")]
        public async Task<ActionResult> GetBusinessPartnersByFilter([FromQuery] int filterType,int subCopmanyId, int pageNumber, int pageSize,int Lang)
        {

            var context = UserContextHelper.GetUserContext(User);

            int? userId = null;
            Guid? systemGuid = null;

            if (context.systemType == "adminUser")
            {
                userId = context.userId.Value; // שמירת userId
            }
            else if (context.systemType == "externalSystem")
            {
                return Unauthorized("This endpoint is for UI users only. Please login via the user interface.");
            }
            else
            {
                return Unauthorized("Invalid user context.");
            }

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
                    var res = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType,Convert.ToInt32(userId), subCopmanyId, pageNumber, pageSize, Lang);
                    return Ok(res);

                case FilterType.Client:
                    var res2 = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId, pageNumber, pageSize, Lang);
                    return Ok(res2);

                case FilterType.Both:
                    var res3 = await _uninetOutPutAppService.GetBusinessPartnersByFilter(filterType, Convert.ToInt32(userId), subCopmanyId, pageNumber, pageSize, Lang);
                    return Ok(res3);

                default:
                    // Invalid filter type
                    return BadRequest("Filter type does not match.");
            }

        }
    }
}
