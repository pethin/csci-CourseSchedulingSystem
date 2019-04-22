using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CourseSchedulingSystem.Services
{
    /// <summary>
    /// Does not send emails. Only needed for ASP.NET Core Identity.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}