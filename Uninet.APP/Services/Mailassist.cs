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

namespace Uninet.APP.Services
{
    public class Mailassist: IMailassist
    {

        readonly IDataMailassist _dataMailassist = null;
       
        public Mailassist(IDataMailassist dataMailassist)
        {
            // _logger = logger;
            _dataMailassist = dataMailassist;
        }
       
        //public string PopulateResearchBody()//string userName, string title, string url, string description
        //{
        //    try
        //    {
        //        var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/researchregister.html");

        //        string body = string.Empty;
        //        using (StreamReader reader = new StreamReader(fullPath))
        //        {
        //            body = reader.ReadToEnd();
        //        }
        //        //body = body.Replace("{UserName}", userName);
        //        //body = body.Replace("{Title}", title);
        //        //body = body.Replace("{Url}", url);
        //        //body = body.Replace("{Description}", description);
        //        return body;
        //    }
        //    catch (Exception ex)
        //    {
        //        Loger.Writetolog("current function PopulateBody" + ex.Message + ex.InnerException);
        //        return "";
        //    }
        //}
        //public string PopulateBody()//string userName, string title, string url, string description
        //{
        //    try
        //    {
        //        var fullPath = System.Web.Hosting.HostingEnvironment.MapPath(@"~/App_Data/panelistregister.html");
        //        //string filePath = System.IO.Path.GetFullPath(@"\Template\panelistregister.html");
        //        //StreamReader sr = new StreamReader(filePath);
        //        // string dir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);


        //        // string file = dir + @"\Template\panelistregister.html";
        //        string body = string.Empty;
        //        using (StreamReader reader = new StreamReader(fullPath))
        //        {
        //            body = reader.ReadToEnd();
        //        }
        //        //body = body.Replace("{UserName}", userName);
        //        //body = body.Replace("{Title}", title);
        //        //body = body.Replace("{Url}", url);
        //        //body = body.Replace("{Description}", description);
        //        return body;
        //    }
        //    catch (Exception ex)
        //    {
        //        Loger.Writetolog("current function PopulateBody" + ex.Message + ex.InnerException);
        //        return "";
        //    }
        //}


       


        public async Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From ,string To,int TemplateId,int Lang, string encryptedUserId = null)
        {
            try
            {
                return await _dataMailassist.sendsmtpmail(subject, From, To, TemplateId, Lang,null,null, encryptedUserId);
               // Task<SendOtpViaMailResponse> sendsmtpmail(string subject, string From, string To, int Templateid, int lang, RequestedMailObject InputMailDetails = null, string username = null, string encryptedUserId = "");
            }
            catch (Exception ex)
            {
               // Loger.Writetolog("current function sendsmtpmail" + ex.Message + ex.InnerException);
                return null;
            }


        }

        //public bool SendPushNotificationapp(string Token, string surveylink)
        //{
        //    try
        //    {
        //        string title = "";
        //        string body = "";
        //        var data = new { type = "link", message = surveylink };
        //        title = "שלום רב,הגיע אליך הסקר";
        //        body = "על מנת למלא את הסקר ולקבל ניקוד בעבורו עליך ללחוץ על ההודעה ותועבר לדף הסקר";
        //        var tokens = new string[1];
        //        tokens[0] = Token; //"eEwMnO_RTKi9oxph4WZEcz:APA91bGXwGYP-yMtI1qxxcOv72yy7xNFsXKQ72A3ft1fWxKT-qHCLEK7UOm-jX6L8rvNQndoDlsnjgqEXj0-XF8S7ZujaZvw6O-VFk1iVgCOCMigSUGKD3zl2kX-Dq6hg53NEJ0rYNJH";
        //        Loger.Writetolog("Token" + Token);
        //        var pushSent = PushNotificationLogic.SendPushNotification(tokens, title, body, data);
        //        Loger.Writetolog("firebase statuses" + "pushSent.Status" + pushSent.Status.ToString());
        //        return Convert.ToBoolean(pushSent);
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}


        //public bool sendpartialmailsubgroup(List<getFitpanelistim_Result> Templistbypopulationid, int Calculated_multnumber, string query_getemailtemplate_content, int surveyid)
        //{
        //    try
        //    {
        //        Random rnd = new Random();
        //        for (int i = 0; i < Calculated_multnumber; i++)
        //        {


        //            int index = rnd.Next(0, Templistbypopulationid.Count - 1);

        //            string email = Templistbypopulationid[index].email;
        //            string femilyname = Templistbypopulationid[index].femilyname;
        //            string privatename = Templistbypopulationid[index].privatename;
        //            string body = query_getemailtemplate_content.Replace("@@FirstName", privatename).Replace("@@LastName", femilyname).Replace("@@surveylink", Templistbypopulationid[index].GizmoLink);
        //            string Token = Templistbypopulationid[index].token;

        //            string Gizmolink = Templistbypopulationid[index].GizmoLink;
        //            //var fromAddress = new MailAddress("info@goldpanel.co.il", "GOLD PANEL");
        //            //var toAddress = new MailAddress(email);//boaz.zur@gmail.com

        //            Mailassist mailassistobj = new Mailassist("info@goldpanel.co.il", email);
        //            //const string fromPassword = "as200880";
        //            string subject = "גולד  פאנל מזמינים אותך ";
        //            // string body = "אוכלוסייה היא    " + Templistbypopulationid[0].populationid + "המייל שנישלח אליו הוא " + email;
        //            //if (email == "eyalberda@gmail.com")
        //            //{

        //            //    SendPushNotificationapp(Token, Gizmolink);
        //            //}
        //            //var sendmailsmtp_flag = mailassistobj.sendsmtpmail(subject, body,null,string.Empty,true, Token, Gizmolink);
        //            //if (sendmailsmtp_flag)
        //            //{ 
        //            bool flag = SaveMembersurvay(Templistbypopulationid[index], surveyid);
        //            Templistbypopulationid.RemoveAt(index);
        //            // }
        //        }

        //        return true;


        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }


        //}



    


    }
}
