using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using Uninet.Domain.Classes;
using Uninet.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Amazon.Runtime.Internal.Transform;

namespace Uninet.DATA.Services
{
    public  class BatchDataMailassist: IBatchDataMailassist
    {
        
        private readonly IBatchRepository<UninetBatchContext> _batchrepository;
        public IConfiguration Configuration { get; }
        public BatchDataMailassist(IBatchRepository<UninetBatchContext> batchrepository, IConfiguration configuration)//, IloginRepository loginRepository
        {

            _batchrepository = batchrepository;
            Configuration= configuration;

        }

        public string ReplacePlaceholders(string html, string otpCode, string companyName)
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

        public static string ReplaceDynamicPlaceholders(string html, Dictionary<string, string> placeholderValues)
        {
            foreach (var kvp in placeholderValues)
            {
                string placeholder = $"{{{kvp.Key}}}";
                string value = kvp.Value;
                html = html.Replace(placeholder, value);
            }

            return html;
        }

        private async Task WriteToTableAsync(int Taskid, string TaskDesc, string text)
        {
            // Define the time zone ID for Israel
            string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

            // Get the Israel time zone
            TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

            // Convert server's DateTime.Now to Israel local time
            DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
            var entity = new Jobbatchlog // Replace YourTableName with the appropriate class name
            {
                TaskId = Taskid,
                TaskDesc = TaskDesc,
                date = israelNow,
                text = text
            };
            await _batchrepository.CreateAsync(entity);
        }
        public async Task<string> GenerateGuidForEmailVerification(string Userid)
        {
            try
            {
                var Adminuserres = _batchrepository.GetFirstObject<AdminUsers>(x => x.AdminUserid == Convert.ToInt32(Userid));
                if (Adminuserres != null)
                {


                   if (Adminuserres.EmailGuidVerification == null)
                    {
                        string emailGuidVerification = Guid.NewGuid().ToString();
                        Adminuserres.EmailGuidVerification = emailGuidVerification;
                        await _batchrepository.UpdateAsync(Adminuserres);
                        await WriteToTableAsync(22, "--adminusertableupdated ", Userid + " " + emailGuidVerification);
                        return emailGuidVerification;
                    }
                    else 
                    {
                        return Adminuserres.EmailGuidVerification;
                    }
                }
                else
                {
                    return "";
                }
               
                
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        public async Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To, int Templateid, int lang, RequestedMailObject InputMailDetails = null,string Userid = "", string JsonDocumentid = "")
        {
            try
            {
                SendOtpViaMailResponse sendsmtpmailres = new SendOtpViaMailResponse();
                var fromAddress = new MailAddress(From);
                var toAddress = new MailAddress(To);
                MailMessage message = new MailMessage(fromAddress, toAddress);
                message.Subject = subject;
                var ObjTemplateparam = new { TemplateId = Templateid, Lang = lang };
                string userOtp = Generate_otp();

                var res = _batchrepository.ExecuteGetSP<OTPHtmlBody>(ConstUninetStoredprocedure.SP_GetHtmlBody, ObjTemplateparam).ToList();
                // Define the time zone ID for Israel
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                switch (ObjTemplateparam.TemplateId)
                {
                    case 1:
                        
                        //recipient name
                        Dictionary<string, string> values1 = new Dictionary<string, string>
                        {

                            { "OTP_CODE", userOtp },
                        };

                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values1);
                        break;
                    case 2:

                        Dictionary<string, string> values2 = new Dictionary<string, string>
                        {
                            { "username", "John Doe" }
                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values2);



                        break;
                    case 3:
                        Dictionary<string, string> values3 = new Dictionary<string, string>
                        {
                            { "recipient name", "eyal berda" },
                            { "Sender name", "yosi mualem " }

                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values3);

                        break;
                    case 7:

                        //string EmailGuidVerification = await GenerateGuidForEmailVerification(Userid);
                       
                            var RedirectUrl = Configuration.GetValue<string>("UrlRedirect:Console");
                            // string emailLink = $"{RedirectUrl}?activeKey=Entered&jsonDocumentid={JsonDocumentid}"&EmailGuidVerification="{EmailGuidVerification}";
                            var jsonObj = _batchrepository.GetFirstObject<BusinessData>(x => x.JsonDocumentid == JsonDocumentid);
                            if (jsonObj != null)
                            {
                                string ActiveKy = "";
                                switch(jsonObj.DocumentApprovedtoUninet)
                                {
                                    case null:
                                        ActiveKy = "Inbox";
                                        break;
                                    case false:
                                        ActiveKy = "Rejected";
                                        break;
                                    case true:
                                        ActiveKy = "Entered";
                                        break;
                                }

                                string emailLink = $"{RedirectUrl}?activeKey={ActiveKy}&jsonDocumentid={JsonDocumentid}";

                                Dictionary<string, string> values4 = new Dictionary<string, string>
                                {
                                         { "recipient name", InputMailDetails.RecipientName },
                                         { "Sender name",InputMailDetails.Sendername },
                                         { "doc type", InputMailDetails.DocType },
                                         { "DocLink", emailLink },

                                };
                                message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values4);
                            }
                      
                        break;
                    case 5:
                        //string EmailGuidVerification = await GenerateGuidForEmailVerification(Userid);

                        var RedirectUrlUnsigned = Configuration.GetValue<string>("UrlRedirect:SignUp");
                        // string emailLink = $"{RedirectUrl}?activeKey=Entered&jsonDocumentid={JsonDocumentid}"&EmailGuidVerification="{EmailGuidVerification}";
                        
                        var HomepageUnsigned = Configuration.GetValue<string>("UrlRedirect:Homepage");
                        var jsonObjunsigned = _batchrepository.GetFirstObject<BusinessData>(x => x.JsonDocumentid == JsonDocumentid);
                        if (jsonObjunsigned != null)
                        {
                            string ActiveKy = "";
                            switch (jsonObjunsigned.DocumentApprovedtoUninet)
                            {
                                case null:
                                    ActiveKy = "Inbox";
                                    break;
                                case false:
                                    ActiveKy = "Rejected";
                                    break;
                                case true:
                                    ActiveKy = "Entered";
                                    break;
                            }

                            string Signup = $"{RedirectUrlUnsigned}";
                            string HomepageLink= $"{HomepageUnsigned}";
                            Dictionary<string, string> values4 = new Dictionary<string, string>
                                {
                                         { "recipient name", InputMailDetails.RecipientName },
                                         { "Sender name",InputMailDetails.Sendername },
                                         { "doc type", InputMailDetails.DocType },
                                         { "Signup", Signup },
                                         {"Homepage",HomepageLink }

                                };
                            message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values4);
                        }
                        break;
                    case 6:
                        Dictionary<string, string> values6 = new Dictionary<string, string>
                        {
                            { "username", "yona" },
                            { "docType", "pdf " },
                            { "createdDate",israelNow.ToString() },
                            { "RecipientName", "moshe  RecipientName " },
                            {"doc status","opened" }

                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values6);
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
                       
                    };
                    return sendsmtpmailres;

                }


                // return sendMail(message.Body, message.Subject, toAddress.Address);
                sendsmtpmailres = new SendOtpViaMailResponse
                {
                    result = true,
                   
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

