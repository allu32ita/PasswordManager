using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace PasswordManager.Models.Services.Infrastructure
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly IConfiguration par_Configuration;
        public MailKitEmailSender(IConfiguration par_Configuration)
        {
            this.par_Configuration = par_Configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var var_Host        = par_Configuration.GetSection("ClientEmail").GetValue<string>("Host");
                var var_Port        = par_Configuration.GetSection("ClientEmail").GetValue<int>("Port");
                var var_Security    = par_Configuration.GetSection("ClientEmail").GetValue<SecureSocketOptions>("Security");
                var var_Username    = par_Configuration.GetSection("ClientEmail").GetValue<string>("Username");
                var var_Password    = par_Configuration.GetSection("ClientEmail").GetValue<string>("Password");
                var var_Sender      = par_Configuration.GetSection("ClientEmail").GetValue<string>("Sender");

                using var var_Client = new SmtpClient();
                await var_Client.ConnectAsync(var_Host, var_Port, var_Security);

                if (string.IsNullOrEmpty(var_Username) == false)
                {
                    await var_Client.AuthenticateAsync(var_Username, var_Password);
                }
        
                var var_message = new MimeMessage();
                var_message.From.Add(MailboxAddress.Parse(var_Sender));
                var_message.To.Add(MailboxAddress.Parse(email));
                var_message.Subject = subject;
                var_message.Body = new TextPart("html") { Text = htmlMessage };
                await var_Client.SendAsync(var_message);
                await var_Client.DisconnectAsync(true);
            }
            catch (Exception)
            {
                throw new Exception("Errore nel invio email di conferma.");
            }
        }
    }
}