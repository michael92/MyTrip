using JF.AspNet.Identity.DocumentDB;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Microsoft.Azure;
using MyTrip.MyTripLogic.Models;
using System.Threading.Tasks;

namespace MyTrip.MyTripLogic.Services
{
    public class MandrillEmailService
    {
        private readonly MandrillApi _api;

        public MandrillEmailService()
        {
            var apiKey = CloudConfigurationManager.GetSetting("MandrillApiKey");
            _api = new MandrillApi(apiKey);
        }

        public async Task SendPasswordResetEmail(IdentityUser user, string token)
        {
            var email = new EmailAddress(user.Email);

            var message = new EmailMessage
            {
                Tags = new[] { "passreset" },
                To = new[] { email }
            };

            message.AddGlobalVariable("username", user.UserName);
            message.AddGlobalVariable("token", token);

            var result = await _api.SendMessage(new SendMessageRequest(message));
        }
    }
}