using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Net.Mime;

namespace uPlayAgain.Utilities
{
    public class EmailService : IIdentityMessageService
    {
        private NLog.Logger _log = NLog.LogManager.GetLogger("uPlayAgain");
        public async Task SendAsync(IdentityMessage message)
        {
            SmtpClient client = null;
            try
            {
                // Credentials:
                var credentialUserName = ConfigurationManager.AppSettings["emailFrom"];
                var sentFrom = ConfigurationManager.AppSettings["emailFrom"];
                var pwd = ConfigurationManager.AppSettings["emailPassword"];

                // Configure the client:
                client = new SmtpClient(ConfigurationManager.AppSettings["smtpClient"]);
                //client.Port = 587;
                client.Port = 25;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;

                // Create the credentials:
                NetworkCredential credentials = new NetworkCredential(credentialUserName, pwd);
                //client.EnableSsl = true;
                client.EnableSsl = false;
                client.Credentials = credentials;

                // Creazione logo
                LinkedResource logo = new LinkedResource(HttpContext.Current.Server.MapPath("~/css/images/logo.png"));
                logo.ContentId = "logo";
                logo.ContentType = new ContentType("image/jpg");
                AlternateView av = AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html);
                av.LinkedResources.Add(logo);

                // Create the message:
                MailMessage mail = new MailMessage(sentFrom, message.Destination);
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;
                mail.AlternateViews.Add(av);
                mail.From = new MailAddress(ConfigurationManager.AppSettings["emailFrom"]);
                mail.To.Add(new MailAddress(message.Destination));
                mail.Subject = message.Subject;

                // Send:
                await client.SendMailAsync(mail);
            }
            catch(Exception ex)
            {
                _log.Error(ex);
            }
            finally
            {
                client.Dispose();
            }
        }        
    }
}