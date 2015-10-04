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
                client = new SmtpClient(ConfigurationManager.AppSettings["smtpClient"])
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = false,
                    Credentials = new NetworkCredential(credentialUserName, pwd)
                };

                // Creazione logo
                AlternateView av = AlternateView.CreateAlternateViewFromString(message.Body, null, MediaTypeNames.Text.Html);
                //av.LinkedResources.Add(await CreateEmbeddedImageAsync(HttpContext.Current.Server.MapPath("~/css/images/logo.png"), "logo"));

                // Create the message:
                MailMessage mail = new MailMessage(sentFrom, message.Destination)
                {
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                    From = new MailAddress(ConfigurationManager.AppSettings["emailFrom"], "UPlayAgain"),
                    Subject = message.Subject
                };
                mail.AlternateViews.Add(av);
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
        
        private async Task<LinkedResource> CreateEmbeddedImageAsync(string path, string CID)
        {
            return await Task.Run(() => {
                LinkedResource image = new LinkedResource(path);
                image.ContentId = CID;
                image.ContentType = new ContentType("image/jpg");
                return image;
            });
        }
    }
}