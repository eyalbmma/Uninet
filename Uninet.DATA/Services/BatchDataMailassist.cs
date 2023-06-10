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

namespace Uninet.DATA.Services
{
    public  class BatchDataMailassist: IBatchDataMailassist
    {
        
        private readonly IBatchRepository<UninetBatchContext> _batchrepository;

        public BatchDataMailassist(IBatchRepository<UninetBatchContext> batchrepository)//, IloginRepository loginRepository
        {

            _batchrepository = batchrepository;

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

        public async Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To, int Templateid, int lang, RequestedMailObject InputMailDetails = null)
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
                    case 4:
                        Dictionary<string, string> values4 = new Dictionary<string, string>
                        {
                            { "recipient name", InputMailDetails.RecipientName },
                            { "Sender name",InputMailDetails.Sendername },
                             { "doc type", InputMailDetails.DocType },
                              { "DocLink", InputMailDetails.DocLink }

                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values4);
                        break;
                    case 5:
                        Dictionary<string, string> values5 = new Dictionary<string, string>
                        {
                            { "recipient name", "eyal berda" },
                            { "Sender name", "yosi mualem " },
                             { "doc type", "pdf " },
                             { "docID", "111 " }

                        };
                        message.Body = ReplaceDynamicPlaceholders(res[0].HtmlBody, values5);
                        break;
                    case 6:
                        Dictionary<string, string> values6 = new Dictionary<string, string>
                        {
                            { "username", "yona" },
                            { "docType", "pdf " },
                            { "createdDate", DateTime.Now.ToString() },
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

