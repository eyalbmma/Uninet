using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Uninet.APP.Interfaces;
using Uninet.Domain.Models;
namespace UninetWebApi2.Controllers
{
    [Authorize(Policy = "ExternalPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class SendDocsController : ControllerBase
    {
        private readonly ISendDocsService _sendDocsService;

        public SendDocsController(ISendDocsService sendDocsService)
        {
            _sendDocsService = sendDocsService;
        }

        [HttpPost("send_docs")]
        public async Task<IActionResult> SendDocs([FromBody] SendDoc request)
        {
            if (request == null || string.IsNullOrEmpty(request.documentId))
            {
                return BadRequest("Invalid document data.");
            }
            if (string.IsNullOrEmpty(request.fis_id) || string.IsNullOrEmpty(request.entity_id_internal) || string.IsNullOrEmpty(request.entity_vat_number))
            {
                return BadRequest("Missing entity identification fields (fis_id, entity_id_internal, entity_vat_number)");
            }
            bool isSaved = await _sendDocsService.SaveDocumentAsync(request);

            if (!isSaved)
            {
                return Conflict("Document already exists.");
            }

            return Ok(new { success = true, message = "Document saved successfully." });
        }
    }
    

  
}
