using System;
using System.Net.Mime;
using System.IO;
using System.Net.Mail;
using System.Net;
using System.Net.Http.Headers;

namespace Wrd2Pdf.Consumer
{
    public class Program
    {
        private const string MailFrom = "gicirbeymail@gmail.com";
        private const string pass = "192837eyrt";
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        public static bool SendEmail(string email, MemoryStream memoryStream, string fileName)
        {
            try
            {
                //memory başlangıç pozisyonu
                memoryStream.Position = 0;
                //içerik tipi pdf olarak seçildi
                ContentType contentType = new ContentType(MediaTypeNames.Application.Pdf);

                //attachment a streamden dosya okunacak
                Attachment attachment = new Attachment(contentStream: memoryStream, contentType: contentType);
                attachment.ContentDisposition.FileName = $"{fileName}.pdf";
                memoryStream.Close();

                SmtpClient smtpClient = new SmtpClient()
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    Credentials = new NetworkCredential("Halo", pass)
                };
                MailMessage mailMessage = new MailMessage()
                {
                    From = new MailAddress(MailFrom),
                    Subject = "PDF File convert",
                    Body = "Look attachment",
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(new MailAddress(email));
                mailMessage.Attachments.Add(attachment);

                smtpClient.Send(mailMessage);

                Console.WriteLine("Mail Send");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.InnerException.Message);
                return false;
            }

        }
    }
}
