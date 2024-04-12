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
using Uninet.Domain.Models;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessPartnersController : ControllerBase
    {
        private readonly IUninetOutPutAppService _uninetOutPutAppService;
        private readonly EnumRepository _enumRepository;
       
        
        public BusinessPartnersController(IUninetOutPutAppService uninetOutPutAppService, EnumRepository enumRepository, IDataMailassist dataMailassist, IMongoClient client)
        {
           
            _uninetOutPutAppService = uninetOutPutAppService;
            _enumRepository = enumRepository;
           
        }



        [Authorize]
        [HttpPost("SendEmail")]
       
        public async Task<ActionResult> SendEmail([FromBody] SendEmailRequest sendEmailRequest)
        {
            /*
            public class BusinessPartnerProp
                {
                    public string BusinesspartnerName { get; set; }
                    public string VatId { get; set; }
                    public int DocAmount { get; set; }
                    public string Status { get; set; }
                    public DateTime LastInvitationDate { get; set; }

                    public ActionItem Actions { get; set; }
                }
            
            */
           
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            _uninetOutPutAppService.SendEmail(sendEmailRequest.VatId, userId, sendEmailRequest.Lang);

            
           
            return Ok();
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
