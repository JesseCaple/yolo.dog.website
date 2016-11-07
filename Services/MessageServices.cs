namespace Yolo.Dog.Website.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using MailKit.Net.Smtp;
    using Microsoft.AspNetCore.Hosting;
    using MimeKit;

    public class MessageServices : IEmailSender, IEmailValidator
    {
        private static Regex regex;
        private static HashSet<string> bannedDomains;
        private readonly IHostingEnvironment env;

        static MessageServices()
        {
            regex = new Regex(
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
            bannedDomains = new HashSet<string>();
            using (var stream = Assembly.GetEntryAssembly().GetManifestResourceStream("yolo.dog.website.Data.BannedDomains.txt"))
            using (var reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    bannedDomains.Add(reader.ReadLine().Trim());
                }
            }
        }

        public MessageServices(IHostingEnvironment env)
        {
            this.env = env;
        }

        public bool IsValidEmail(string email)
        {
            return regex.IsMatch(email);
        }

        public bool IsBannedEmailDomain(string email)
        {
            string domain = email.Substring(email.IndexOf('@') + 1);
            return bannedDomains.Contains(domain);
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Tubes", "noreply@tubes-project.com"));
            emailMessage.To.Add(new MailboxAddress(string.Empty, email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            if (this.env.IsDevelopment())
            {
                using (StreamWriter data = System.IO.File.CreateText(@"C:\Personal\Code\tubes_email\email.txt"))
                {
                    emailMessage.WriteTo(data.BaseStream);
                }
            }
            else
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync("smtp.example.com", 465, true);
                    await client.AuthenticateAsync("username", "password");
                    await client.SendAsync(emailMessage);
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
