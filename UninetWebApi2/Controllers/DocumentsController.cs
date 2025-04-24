using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UninetWebApi2.Controllers
{
    [Authorize(Policy = "ExternalPolicy")]
    [ApiController]
    [Route("api/[controller]")]
    public class DocumentsController : ControllerBase
    {
        [HttpPost("send_docs")]
        public async Task<IActionResult> SendDocs([FromBody] SendDocsRequest request)
        {
            // Validate request
            if (request == null || request.FinanceSystem == null || request.Entities == null)
            {
                return BadRequest("Invalid request format.");
            }

            // Check for duplicate documents (pseudo logic)
            foreach (var entity in request.Entities)
            {
                foreach (var doc in entity.Documents)
                {
                    bool exists = await DocumentExistsAsync(doc.Id);
                    if (exists)
                    {
                        // Skip duplicates
                        continue;
                    }

                    // Save document (pseudo logic)
                    await SaveDocumentAsync(entity, doc);
                }
            }

            // Return success response
            return Ok(new SendDocsResponse
            {
                Success = true,
                Message = "All documents processed successfully."
            });
        }

        private Task<bool> DocumentExistsAsync(string documentId)
        {
            // TODO: Implement actual logic to check if document exists in DB
            return Task.FromResult(false);
        }

        private Task SaveDocumentAsync(EntityInfo entity, DocumentInfo doc)
        {
            // TODO: Implement actual logic to save document
            return Task.CompletedTask;
        }
    }

    // Request DTO
    public class SendDocsRequest
    {
        public FinanceSystemInfo FinanceSystem { get; set; }
        public string RequestId { get; set; }
        public DateTime PreviousRequestDate { get; set; }
        public DateTime CurrentRequestDate { get; set; }
        public List<EntityInfo> Entities { get; set; }
    }

    public class FinanceSystemInfo
    {
        public Guid ExternalSystemGuid { get; set; }
        public string FinanceSystemName { get; set; }
    }

    public class EntityInfo
    {
        public int InternalEntityId { get; set; }
        public string EntityName { get; set; }
        public string TaxId { get; set; }
        public string VatId { get; set; }
        public List<DocumentInfo> Documents { get; set; }
    }

    public class DocumentInfo
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public int Number { get; set; }
        public string DocumentDate { get; set; }
        public long CreationDate { get; set; }
        public int Status { get; set; }
        public string Lang { get; set; }
        public float AmountDueVat { get; set; }
        public float AmountExemptVat { get; set; }
        public float AmountExcludedVat { get; set; }
        public float AmountLocal { get; set; }
        public float AmountOpened { get; set; }
        public float Vat { get; set; }
        public float Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencyName { get; set; }
        public float CurrencyRate { get; set; }
        // Additional fields based on detailed documentation can be added here
    }

    // Response DTO
    public class SendDocsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<string> Errors { get; set; }
    }
}
