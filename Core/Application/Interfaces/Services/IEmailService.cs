using Linnked.Core.Domain.Entities;
using Linnked.Models;
using Linnked.Models;


public interface IEmailService
{
    Task<BaseResponse> SendWelcomeEmailAsync(Message message);
    Task<BaseResponse> SendRejectionEmailAsync(Message message);
    Task<BaseResponse> SendAcceptanceEmailAsync(Message message);

    Task<BaseResponse> SendEmailAsync(User user);
    public Task<bool> SendEmailAsync(MailRecieverDto model, MailRequests request);
    public Task<string> SendEmailClient(string msg, string title, string email);
}
