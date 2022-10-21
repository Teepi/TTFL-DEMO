using System.Net;
using System.Net.Mail;

namespace TTFL.COMMON.Helpers.MailHelper
{
    public class MailHelper
    {
        /// <summary>
        /// Send Mail
        /// </summary>
        /// <param name="messsage"></param>
        /// <returns></returns>
        public static async Task<bool> SendMailAsync(string messsage)
        {
            try
            {
                using (MailMessage message = new())
                {
                    message.From = new MailAddress("thomas.peyrot@hotmail.com");
                    message.To.Add(new MailAddress("thomas.peyrot@hotmail.com"));
                    message.Subject = $"TTFL -Resultat scrapping du {DateTime.Now:dd-MM-yyyy}";
                    message.IsBodyHtml = true; //to make message body as html  
                    message.Body = messsage;

                    using SmtpClient smtp = new();
                    smtp.Port = 587;
                    smtp.Host = "smtp-mail.outlook.com"; //for outlook host  
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("thomas.peyrot@hotmail.com", "Doline.949621");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    await smtp.SendMailAsync(message);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
