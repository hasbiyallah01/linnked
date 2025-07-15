using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Linnked.Models;
using Linnked.Core.Domain.Entities;

namespace Linnked.Core.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IWebHostEnvironment _hostenv;
        private readonly EmailConfiguration _emailConfiguration;
        public EmailService(IWebHostEnvironment hostenv, IOptions<EmailConfiguration> emailConfiguration)
        {
            _hostenv = hostenv;
            _emailConfiguration = emailConfiguration.Value;
        }


        public async Task<BaseResponse> SendWelcomeEmailAsync(Message message)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = message.SenderEmail,
                Name = message.SenderFirstName
            };

            string imageUrl = "https://drive.google.com/uc?export=view&id=1s-p-LPh2ZZqQc_N39LVTDEpZpmk1V9jE";

            string emailBody = $@"
                        <div style='text-align: center;'>
                            <img src='{imageUrl}' alt='Welcome to Linnked' style='max-width: 100%; height: auto;' />
                        </div>
                        <p>You're now part of a world where sending cute, thoughtful messages is effortless.</p>
                        <p>Whether it’s love, friendship, or just a little appreciation, Linnked makes every message memorable. And guess what? We’re starting with Linnked’s! ❤️</p>

                        <h3>What's Next?</h3>
                        <ul>
                            <li>✅ Create a message - Pick your words or let AI help you!</li>
                            <li>✅ Share your link - Send it in seconds.</li>
                            <li>✅ Get a response - A sweet “Yes” or... well, maybe next time.</li>
                        </ul>

                        <h3>Spread the Love!</h3>
                        <p>Share your Linnked experience and tag us on:
                            <a href='https://www.linkedin.com/company/linnked/posts/?feedView=all' target='_blank'>LinkedIn</a> and 
                            <a href='https://x.com/hellolinnked' target='_blank'>X</a>.
                        </p>

                        <h3>💕 Start Linnking Now</h3>
                        <a href='https://linnked.vercel.app' style='padding: 10px 15px; background-color: #ff5c5c; color: white; text-decoration: none; border-radius: 5px;'>Send a Message</a>";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "🎉 Welcome to Linnked! ❤️",
                HtmlContent = emailBody
            };

            await SendEmailAsync(mailRecieverRequestDto, mailRequest);

            return new BaseResponse
            {
                Message = "Welcome email sent successfully",
                IsSuccessful = true,
            };
        }

        public async Task<BaseResponse> SendAcceptanceEmailAsync(Message message)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = message.SenderEmail,
                Name = message.SenderFirstName
            };

            string imageUrl = "https://drive.google.com/uc?export=view&id=1rxU2RCSH2GPN_0QKgcd8ZfB_DH1TZSf8";

            string emailBody = $@"
                                <div style='text-align: center;'>
                                    <img src='{imageUrl}' alt='Message Accepted' style='max-width: 100%; height: auto;' />
                                </div>
                                <p>Hey {message.SenderFirstName},</p>
                                <p>Great news! {message.RecipientFirstName} has accepted your message—looks like you’ve got a Linnked (or just made their day)! 💖</p>

                                <p>Now, let’s make it official—share your Linnked moment on social media & tag us! The world deserves to see this!</p>
                                <ul>
                                    <li>Tag us on X: <a href='https://x.com/hellolinnked' target='_blank'>@hellolinnked</a></li>
                                </ul>

                                <p>Enjoy the moment! More cute messages, more love, and more Linnks ahead. ❤️</p>

                                <p>—💕The Linnked Team</p>

                                <h3>Share this Moment</h3>
                                <a href='https://linnked.vercel.app' style='padding: 10px 15px; background-color: #ff5c5c; color: white; text-decoration: none; border-radius: 5px;'>Share this Moment</a>
                            ";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "Congratulations! Your message has been accepted ❤️",
                HtmlContent = emailBody
            };

            await SendEmailAsync(mailRecieverRequestDto, mailRequest);

            return new BaseResponse
            {
                Message = "Acceptance email sent successfully",
                IsSuccessful = true,
            };
        }





        public async Task<BaseResponse> SendRejectionEmailAsync(Message message)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = message.SenderEmail,
                Name = message.SenderFirstName
            };

            string imageUrl = "https://drive.google.com/uc?export=view&id=1rxjVhaYwNw2IUivguLUwkcyfBeFAgIhK";

            string emailBody = $@"
                        <div style='text-align: center;'>
                            <img src='{imageUrl}' alt='Linnked' style='max-width: 100%; height: auto;' />
                        </div>

                        <p>Hey {message.SenderFirstName},</p>
                        <p>We know this isn’t the news you were hoping for... {message.RecipientFirstName} didn’t accept your message this time 💔. But hey, love (and friendship) is all about taking chances!</p> 

                        <p>Why not Linnk someone else? There’s always someone out there who’d love to hear from you. Create another message & try again!</p>

                        <p>And if you're feeling bold, share your Linnked experience & tag us! 
                            <a href='https://x.com/hellolinnked' target='_blank'>@hellolinnked</a>
                        </p>

                        <p>Keep spreading the love! ❤️<br>— The Linnked Team 💕</p> 

                        <h3>Send Another Message</h3>
                        <a href='https://linnked.vercel.app' style='padding: 10px 15px; background-color: #ff5c5c; color: white; text-decoration: none; border-radius: 5px;'>Send Another Message</a>
                        ";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "Your Message was Rejected 💔",
                HtmlContent = emailBody
            };

            await SendEmailAsync(mailRecieverRequestDto, mailRequest);

            return new BaseResponse
            {
                Message = "Rejection email sent successfully",
                IsSuccessful = true,
            };
        }

        public Task<string> SendEmailClient(string msg, string title, string email)
        {
            if (string.IsNullOrEmpty(msg))
            {
                Console.WriteLine("Error: Email message content is null or empty.");
                throw new ArgumentNullException(nameof(msg), "Email message content cannot be null or empty");
            }

            try
            {
                var emailAddress = MailboxAddress.Parse(email);
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Email Address.");
                return Task.FromResult("Invalid Email");
            }

            var message = new MimeMessage();
            message.To.Add(MailboxAddress.Parse(email));
            message.From.Add(new MailboxAddress(_emailConfiguration.EmailSenderName, _emailConfiguration.EmailSenderAddress));
            message.Subject = title;

            message.Body = new TextPart("html")
            {
                Text = msg
            };

            using (var client = new SmtpClient())
            {
                try
                {
                    Console.WriteLine("Inside email client");
                    client.Connect(_emailConfiguration.SMTPServerAddress, _emailConfiguration.SMTPServerPort, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(_emailConfiguration.EmailSenderAddress, _emailConfiguration.EmailSenderPassword);
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred in email client: {ex.Message}", DateTime.UtcNow.ToLongTimeString());
                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }

            return Task.FromResult("Email Sent");
        }


        public async Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request)
        {
            try
            {
                Console.WriteLine("Calling email client");
                string buildContent = $"Dear {model.Name}," +
                                            $"<p>{request.Body}</p>";

                if (string.IsNullOrWhiteSpace(request.HtmlContent))
                {
                    throw new ArgumentNullException(nameof(request.HtmlContent), "Email content cannot be null or empty");
                }

                await SendEmailClient(request.HtmlContent, request.Title, model.Email);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("There was an error while sending email");
            }
        }

        public async Task<BaseResponse> SendEmailAsync(User user)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = user.Email,
                Name = user.FirstName
            };

            string imageUrl = "https://drive.google.com/uc?export=view&id=1s-p-LPh2ZZqQc_N39LVTDEpZpmk1V9jE";

            string emailBody = $@"
                        <div style='text-align: center;'>
                            <img src='{imageUrl}' alt='Welcome to Linnked' style='max-width: 100%; height: auto;' />
                        </div>
                        <p>You're now part of a world where sending cute, thoughtful messages is effortless.</p>
                        <p>Whether it’s love, friendship, or just a little appreciation, Linnked makes every message memorable. And guess what? We’re starting with Linnked’s! ❤️</p>

                        <h3>What's Next?</h3>
                        <ul>
                            <li>✅ Create a message - Pick your words or let AI help you!</li>
                            <li>✅ Share your link - Send it in seconds.</li>
                            <li>✅ Get a response - A sweet “Yes” or... well, maybe next time.</li>
                        </ul>

                        <h3>Spread the Love!</h3>
                        <p>Share your Linnked experience and tag us on:
                            <a href='https://www.linkedin.com/company/linnked/posts/?feedView=all' target='_blank'>LinkedIn</a> and 
                            <a href='https://x.com/hellolinnked' target='_blank'>X</a>.
                        </p>

                        <h3>💕 Start Linnking Now</h3>
                        <a href='https://linnked.vercel.app' style='padding: 10px 15px; background-color: #ff5c5c; color: white; text-decoration: none; border-radius: 5px;'>Send a Message</a>";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "🎉 Welcome to Linnked! ❤️",
                HtmlContent = emailBody
            };

            await SendEmailAsync(mailRecieverRequestDto, mailRequest);

            return new BaseResponse
            {
                Message = "Welcome email sent successfully",
                IsSuccessful = true,
            };
        }



    }


}
/*public async Task<BaseResponse> SendAcceptanceEmailAsync(Message message)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = message.SenderEmail,
                Name = message.SenderFirstName
            };

            string imageUrl = "https://drive.google.com/file/d/1rxU2RCSH2GPN_0QKgcd8ZfB_DH1TZSf8/view?usp=sharing"; 

            string emailBody = $@"
                                <div style='text-align: center;'>
                                    <img src='{imageUrl}' alt='Message Accepted' style='max-width: 100%; height: auto;' />
                                </div>
                                <p>Hey {message.SenderFirstName},</p>
                                <p>Great news! {message.RecipientFirstName} has accepted your message—looks like you’ve got a Linnked (or just made their day)! 💖</p>
        
                                <p>Now, let’s make it official—share your Linnked moment on social media & tag us! The world deserves to see this!</p>
                                <ul>
                                    <li>Tag us on X: <a href='https://x.com/hellolinnked' target='_blank'>@hellolinnked</a></li>
                                </ul>
        
                                <p>Enjoy the moment! More cute messages, more love, and more Linnks ahead. ❤️</p>
        
                                <p>— The Linnked Team</p>
        
                                <h3>Share this Moment</h3>
                                <a href='https://linnked.vercel.app' style='padding: 10px 15px; background-color: #ff5c5c; color: white; text-decoration: none; border-radius: 5px;'>Share this Moment</a>
                            ";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "Congratulations! Your message has been accepted 🎉",
                HtmlContent = emailBody
            };

            await SendEmailAsync(mailRecieverRequestDto, mailRequest);

            return new BaseResponse
            {
                Message = "Acceptance email sent successfully",
                IsSuccessful = true,
            };
        }*/


/*public async Task<BaseResponse> SendWelcomeEmailAsync(Message message)
        {
            var mailRecieverRequestDto = new MailRecieverDto
            {
                Email = message.SenderEmail,
                Name = message.SenderFirstName
            };

            string imageUrl = "https://drive.google.com/file/d/1s-p-LPh2ZZqQc_N39LVTDEpZpmk1V9jE/view?usp=drive_link"; 

            string emailBody = $@"
                                <div style='text-align: center;'>
                                    <img src='{imageUrl}' alt='Welcome to Linnked' style='max-width: 100%; height: auto;' />
                                </div>
                                <p>You're now part of a world where sending cute, thoughtful messages is effortless.</p>
                                <p>Whether it’s love, friendship, or just a little appreciation, Linnked makes every message memorable. And guess what? We’re starting with Linnked’s! ❤️</p>
        
                                <h3>What's Next?</h3>
                                <ul>
                                    <li>Create a message - Pick your words or let AI help you!</li>
                                    <li>Share your link - Send it in seconds.</li>
                                    <li>Get a response - A sweet “Yes” or... well, maybe next time.</li>
                                </ul>
        
                                <h3>Spread the Love!</h3>
                                <p>Share your Linnked experience and tag us on:
                                    <a href='https://www.linkedin.com/company/linnked/posts/?feedView=all' target='_blank'>LinkedIn</a> and 
                                    <a href='https://x.com/hellolinnked' target='_blank'>X</a>.
                                </p>
        
                                <h3>Start Linnking Now</h3>
                                <a href='https://linnked.vercel.app' style='padding: 10px 15px; background-color: #ff5c5c; color: white; text-decoration: none; border-radius: 5px;'>Send a Message</a>";

            var mailRequest = new MailRequests
            {
                Body = emailBody,
                Title = "🎉 Welcome to Linnked! ❤️",
                HtmlContent = emailBody
            };

            await SendEmailAsync(mailRecieverRequestDto, mailRequest);

            return new BaseResponse
            {
                Message = "Welcome email sent successfully",
                IsSuccessful = true,
            };
        }*/