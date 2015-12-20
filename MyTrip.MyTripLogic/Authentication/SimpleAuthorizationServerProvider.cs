using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using JF.AspNet.Identity.DocumentDB;
using Microsoft.Owin.Security.OAuth;
using MyTrip.MyTripLogic.Repositories;

namespace MyTrip.MyTripLogic.Authentication
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            context.Validated();
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            AuthRepository authRepository = new AuthRepository();

            var result = await authRepository.ValidateUserAsync(context.UserName, context.Password);

            if (!result.Succeeded)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            var user = await authRepository.FindUser(context.UserName);
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim(ClaimTypes.Role, "user"));

            context.Validated(identity);
        }
    }
}