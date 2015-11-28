using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web.Http;
using config = System.Configuration;

namespace uPlayAgain.Controllers
{
    public class AlertAdminController : BaseController
    {
        // POST: api/AlertAdmin
        public async Task Post([FromBody]string message)
        {
            _log.Info(string.Concat("Segnalazione all'amministratore del sito: ", message));

            SmtpClient client = null;
            try
            {
                // Credentials:
                var credentialUserName = config.ConfigurationManager.AppSettings["emailFrom"];
                var sentFrom = config.ConfigurationManager.AppSettings["emailFrom"];
                var pwd = config.ConfigurationManager.AppSettings["emailPassword"];

                // Configure the client:
                client = new SmtpClient(config.ConfigurationManager.AppSettings["smtpClient"])
                {
                    Port = 25,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    EnableSsl = false,
                    Credentials = new NetworkCredential(credentialUserName, pwd)
                };

                // Create the message:
                MailMessage mail = new MailMessage(sentFrom, config.ConfigurationManager.AppSettings["adminMail"])
                {
                    IsBodyHtml = true,
                    Priority = MailPriority.High,
                    From = new MailAddress(config.ConfigurationManager.AppSettings["emailFrom"], "UPlayAgain"),
                    Subject = message
                };
                // Send:
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
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
