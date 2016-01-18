using JF.AspNet.Identity.DocumentDB;
using Mandrill;
using Mandrill.Models;
using Mandrill.Requests.Messages;
using Microsoft.Azure;
using MyTrip.MyTripLogic.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyTrip.MyTripLogic.Services
{
    public class MandrillEmailService
    {
        private readonly MandrillApi _api;
        private string passwordResetUrl;

        public MandrillEmailService()
        {
            passwordResetUrl = CloudConfigurationManager.GetSetting("PassResetFormUri");
            var apiKey = CloudConfigurationManager.GetSetting("MandrillApiKey");
            _api = new MandrillApi(apiKey);
        }

        public async Task<bool> SendPasswordResetEmail(IdentityUser user, string token)
        {
            var email = new EmailAddress(user.Email);

            var url = GetUrl(user.Id, token);

            var message = new EmailMessage
            {
                FromEmail = "noreplay@mytrip.com",
                Tags = new[] { "passreset" },
                To = new[] { email }
            };

            message.AddGlobalVariable("username", user.UserName);
            message.AddGlobalVariable("token", url);

            var request = new SendMessageTemplateRequest(message, "passreset", Enumerable.Empty<TemplateContent>());
            var result = await _api.SendMessageTemplate(request);
            if (!result.Any())
                return false;

            return result.FirstOrDefault().Status == EmailResultStatus.Sent;

        }

        private string GetUrl(string userId, string token)
        {
            var queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryString["userid"] = userId;
            queryString["token"] = token;

            var ub = new UriBuilder(passwordResetUrl);
            ub.Query = queryString.ToString();

            return ub.ToString();
        }
    }
}