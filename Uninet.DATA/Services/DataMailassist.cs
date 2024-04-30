using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.AspNetCore.Mvc;
using static System.Net.WebRequestMethods;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using Uninet.Domain.Models;
using Uninet.DATA.Interfaces;
using System.Reflection;
using Amazon.Runtime.Internal.Transform;
using Uninet.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Web;

namespace Uninet.DATA.Services
{
    public class DataMailassist: IDataMailassist
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _IcountCompaniesInfoCollection;
        public DataMailassist(IRepository<UninetContext> repository, IMongoClient client)//, IloginRepository loginRepository
        {
           
            _repository = repository;


            var database = client.GetDatabase("Uninet");
            _IcountCompaniesInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");

        }

        public string ReplacePlaceholders(string html,  string otpCode, string companyName)
        {


            // Replace [OTP code] with actual OTP code
            html = html.Replace("[OTP code]", otpCode);

            // Replace [company name] with actual company name
            html = html.Replace("[company name]", companyName);

            // Replace <body> tag with <div> tag
            html = html.Replace("<body>", "<div>");

            // Replace </body> tag with </div> tag
            html = html.Replace("</body>", "</div>");

            return html;
        }
        protected string Generate_otp()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            for (int i = 0; i < 4; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString())) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }

        //public static string ReplaceDynamicPlaceholders(string html, Dictionary<string, string> placeholderValues)
        //{
        //    foreach (var kvp in placeholderValues)
        //    {
        //        string placeholder = $"{{{kvp.Key}}}";
        //        string value = kvp.Value;
        //        html = html.Replace(placeholder, value);
        //    }

        //    return html;
        //}
        private string ReplaceDynamicPlaceholders(string htmlBody, Dictionary<string, string> values)
        {
            // Loop through the dictionary of values and replace placeholders in the HTML
            foreach (var entry in values)
            {
                // To replace the placeholders in the HTML, we will use {key} as the placeholder
                // For example, to replace {RESET_URL} with the actual URL, we will search for {RESET_URL} in the HTML and replace it with the URL value.
                htmlBody = htmlBody.Replace("{" + entry.Key + "}", entry.Value);
            }

            return htmlBody;
        }

        public async Task<SendOtpViaMailResponse> BusinessPartnerSendEmail(SendEmailRequest sendEmailRequest, string userId, int Lang)
        {
            SendOtpViaMailResponse resmail = null; // Initialize resmail to null

            try
            {
                // Attempt to send the email
                resmail = await sendsmtpmail("הזמנה להצטרף ליונינט", "eyalbmma@gmail.com", sendEmailRequest.Email, 3, Lang, null, "", userId.ToString(), "");

                // If sendsmtpmail returns null, initialize resmail with a new SendOtpViaMailResponse object
                if (resmail == null)
                {
                    resmail = new SendOtpViaMailResponse { result = false };
                }
                else
                {
                    // If resmail.result is false, keep it false; otherwise, set it to true
                    resmail.result = resmail.result ? true : false;
                }

                string businessName = "";
                int organizationId = 0;
                int subCompanyId = 0;

                var ObjMainSubCopmaniesMasters = await _repository.GetFirstObjectAsync<MainSubCopmaniesMasters>(x => x.SubCopmanyId == sendEmailRequest.SubCompanyId);
                organizationId = ObjMainSubCopmaniesMasters.MainCompanyId;
                subCompanyId = sendEmailRequest.SubCompanyId;

                BusinessPartnersEmails existingEntry = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(e =>
                    e.VatId == Convert.ToInt32(sendEmailRequest.Vatid) &&
                    e.OrganizationId == organizationId &&
                    e.UserId == Convert.ToInt32(userId) &&
                    e.SubCompanyId == subCompanyId);

                if (existingEntry != null)
                {
                    // Update last sent date and email sent status on the existing entity
                    existingEntry.LastDateSent = DateTime.UtcNow;
                    existingEntry.EmailSent = resmail?.result ?? false;
                    await _repository.UpdateAsync(existingEntry);
                }
                else
                {
                    BusinessPartnersEmails emailEntry = new BusinessPartnersEmails
                    {
                        VatId = Convert.ToInt32(sendEmailRequest.Vatid),
                        EntityType = "SomeEntityType",
                        OrganizationId = organizationId,
                        UserId = Convert.ToInt32(userId),
                        SubCompanyId = subCompanyId,
                        EmailSent = resmail?.result ?? false,
                        LastDateSent = DateTime.UtcNow
                    };
                    // Create new record if it does not exist
                    await _repository.CreateAsync(emailEntry);
                }
            }
            catch (Exception ex)
            {
                // Handle or log the exception
                resmail = new SendOtpViaMailResponse { result = false };
            }

            return resmail;
        }


        public async Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To,int Templateid,int lang, RequestedMailObject InputMailDetails= null,string username=null,string encryptedUserId="",string Name=null)
        {
            try
            {
                SendOtpViaMailResponse sendsmtpmailres=new SendOtpViaMailResponse();
                var fromAddress = new MailAddress(From);
                var toAddress = new MailAddress(To);
                MailMessage message = new MailMessage(fromAddress, toAddress);
                string userOtp = Generate_otp();
                message.Subject = subject;
                var ObjTemplateparam = new { TemplateId = Templateid, Lang = lang };
                var res = await _repository.ExecuteGetSPAsync<OTPHtmlBody>(ConstUninetStoredprocedure.SP_GetHtmlBody, ObjTemplateparam);
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                switch (ObjTemplateparam.TemplateId)
                {
                    case 1:
                        //string OtpRedirectUrl = "https://uninet-app.netlify.app/verify-email?Otp=" + userOtp + "&encrypteduserid=" + encryptedUserId;

                        string OtpRedirectUrl = "https://panel.uninet-io.com/verify-email?Otp=" + userOtp + "&encrypteduserid=" + encryptedUserId;


                      

                        //recipient name
                        string encodedUrl = HttpUtility.HtmlAttributeEncode(OtpRedirectUrl);
                        Dictionary<string, string> values1 = new Dictionary<string, string>
                        {
                            
                            { "OTP", userOtp },
                            {"OTPREDIRECTURL",OtpRedirectUrl }
                        };

                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values1);
                        break;
                    case 2:
                        
                        Dictionary<string, string> values2 = new Dictionary<string, string>
                        {
                            { "Username", username }
                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values2);
                       


                        break;
                    case 3:
                        Dictionary<string, string> values3 = new Dictionary<string, string>
                        {
                            { "recipientName", Name },
                           
                            
                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values3);
                        
                        break;
                    case 4:
                        Dictionary<string, string> values4 = new Dictionary<string, string>
                        {
                            { "recipientName", InputMailDetails.RecipientName },
                            { "senderName",InputMailDetails.Sendername },
                             { "docType", InputMailDetails.DocType },
                              { "DocLink", InputMailDetails.DocLink }
                            
                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values4);
                        break;
                    case 5:
                        Dictionary<string, string> values5= new Dictionary<string, string>
                        {
                            { "recipientName", "eyal berda" },
                            { "senderName", "yosi mualem " },
                             { "docType", "pdf " },
                             { "docID", "111 " }

                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values5);
                        break;
                    case 6:
                        Dictionary<string, string> values6 = new Dictionary<string, string>
                        {
                            { "username", "yona" },
                            { "docType", "pdf " },
                            { "createdDate", israelNow.ToString() },
                            { "RecipientName", "moshe  RecipientName " },
                            {"doc status","opened" }

                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values6);
                        break;
                    case 9:

                        var adminuserobject =await _repository.GetFirstObjectAsync<AdminUsers>(x => x.Email == To);
                        if (adminuserobject != null)
                        {
                            string ResetPasswordLandingPage = "https://panel.uninet-io.com/reset-password?UserResetToken=" + adminuserobject.ResetPasswordToken + "&clicktracking=false";
                            Dictionary<string, string> values7 = new Dictionary<string, string>
                            {
                               { "RESET_URL", ResetPasswordLandingPage },
                           };

                            message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values7);
                        }


                        break;
                    default:
                        // Code to handle cases other than 1 to 7
                        break;
                }

                



                /*
                 <add key="Username" value="apikey"/>
                <!--""/-->
                <add key="Password" value="YOUR_EMAIL_API_KEY"/>
                */
                try
                {
                    message.Headers.Add("Content-Type", "text/html");
                    message.BodyEncoding = Encoding.UTF8;
                    message.IsBodyHtml = true;

                   


                    SmtpClient smtp = new SmtpClient("smtp.sendgrid.net", 587);//smtpout.secureserver.net //smtp-relay.sendinblue.com
                    System.Net.NetworkCredential credential = new NetworkCredential("apikey", "YOUR_EMAIL_API_KEY");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = credential;
                    smtp.Send(message);

                    //SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                    //smtp.EnableSsl = true;
                    //System.Net.NetworkCredential credential = new NetworkCredential("eyalberda@gmail.com", "Ilayshaked1!");
                    //smtp.UseDefaultCredentials = false;
                    //smtp.Credentials = credential;
                    //smtp.Send(message);
                }
                catch (Exception ex)
                {
                    sendsmtpmailres = new SendOtpViaMailResponse
                    {
                        result = false,
                        OTP = userOtp
                    };
                    return sendsmtpmailres;

                }


                // return sendMail(message.Body, message.Subject, toAddress.Address);
                sendsmtpmailres = new SendOtpViaMailResponse
                {
                    result = true,
                    OTP = userOtp
                };
                return sendsmtpmailres;

            }
            catch (Exception ex)
            {
                return null;
            }


        }

    }
}
