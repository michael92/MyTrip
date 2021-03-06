﻿using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using MyTrip.MyTripLogic.Models;
using MyTrip.MyTripLogic.Repositories;
using MyTrip.MyTripLogic.Services;

namespace MyTrip.MyTripLogic.Controllers
{
    [RoutePrefix("api/Account")]
    [Authorize]
    public class AccountController : ApiController
    {
        private readonly AuthRepository _repo;

        public AccountController()
        {
            _repo = new AuthRepository();
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserRegistrationModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await _repo.RegisterUserAsync(user);

            IHttpActionResult errorResult = GetErrorResult(result);

            if (errorResult != null)
            {
                return errorResult;
            }

            await new MandrillEmailService().SendWelcomeEmail(user.Email, user.UserName);
            return Ok();
        }

        [AllowAnonymous]
        [Route("PasswordResetEmail")]
        public async Task<IHttpActionResult> GetPasswordResetEmail(string email)
        {
            var user = email.Contains("@") ? 
                await _repo.FindByEmail(email) :
                await _repo.FindUser(email);

            if (user == null)
                return NotFound();

            var token = await _repo.GetPasswordResetToken(user.Id);
            var emailService = new MandrillEmailService();

            var result = await emailService.SendPasswordResetEmail(user, token);
            if (!result)
                return InternalServerError();

            return Ok();
        }

        [AllowAnonymous]
        [Route("PasswordReset")]
        public async Task<IHttpActionResult> PostPasswordReset(PasswordResetModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _repo.ResetPassword(model.UserId, model.Password, model.Token);
            if (!result)
                return BadRequest("Incorrect userid or token");

            return Ok();
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }
    }
}
