using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace Company.Function
{
    public static class SendEmailNotification
    {
        [FunctionName("SendEmailNotification")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            log.LogInformation($"Received JSON: {requestBody}");
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string toEmail =  data?.toEmail;
            string subject =  data?.subject;
            string text =  data?.text;
            string email =  data?.email;
            string phonenumber =  data?.phoneNumber;
            string name =  data?.name;

            // string responseMessage = string.IsNullOrEmpty(toEmail)
            //     ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //     : $"Hello, {toEmail} , {subject} , {text}. This HTTP triggered function executed successfully.";

            try
            {
           // Construct the HTML email body
            string htmlBody = $@"
            <html>
            <body>
                <p>Name : {name},</p>
                <p>Email: {email}</p>
                <p>Phone Number : {phonenumber}</p>
                <p>Text : {text}</p>
            </body>
            </html>";

            // SMTP configuration
            var smtpClient = new System.Net.Mail.SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential("....@hotmail.com", "...."),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(".....@hotmail.com"),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);

            return new OkObjectResult("Email sent successfully!");
        }
        catch (SmtpException ex)
        {
            log.LogInformation($"Error sending email: {ex.Message}");
            return new OkObjectResult("Error sending email");
        }
      }
    }
}
