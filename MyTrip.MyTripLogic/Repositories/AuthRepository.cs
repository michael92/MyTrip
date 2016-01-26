using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using JF.AspNet.Identity.DocumentDB;
using Microsoft.AspNet.Identity;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using MyTrip.MyTripLogic.DB;
using MyTrip.MyTripLogic.Models;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;

namespace MyTrip.MyTripLogic.Repositories
{
    public class AuthRepository
    {
        private readonly DocumentDBUserStore<MyTripUser> userStore;
        private readonly UserManager<MyTripUser> manager;
        private DocumentDb document;

        public AuthRepository() 
        {
            document = new DocumentDb("MyTripDb", null);
            var identityCollectionManager= new IdentityCollectionManager<MyTripUser>(document.Client, "MyTripDb", true);
            var identityRoleStore = new IdentityRoleStore();
            this.userStore= new DocumentDBUserStore<MyTripUser>(identityCollectionManager, identityRoleStore);

            manager = new UserManager<MyTripUser>(userStore);
            var provider = Startup.DataProtectionProvider;
            manager.UserTokenProvider = new DataProtectorTokenProvider<MyTripUser, string>(provider.Create("EmailConfirmation"));
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationModel registrationModel)
        {
            var existingUser = await FindUser(registrationModel.UserName);
            if (existingUser != null)
                return IdentityResult.Failed("User with this username already exists");

            existingUser = await FindByEmail(registrationModel.Email);
            if(existingUser != null)
                return IdentityResult.Failed("User with this email already exists");


            var user = new MyTripUser()
            {
                UserName = registrationModel.UserName,
                Email = registrationModel.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            await manager.CreateAsync(user, registrationModel.Password);
            return IdentityResult.Success;
        }


        public async Task<IdentityUser> FindUser(string userName)
        {
            var user = await manager.FindByNameAsync(userName);
            return user;
        }

        public async Task<IdentityUser> FindByEmail(string email)
        {
            var user = await manager.FindByEmailAsync(email);
            return user;
        }

        public async Task<string> GetPasswordResetToken(string userId)
        {
            return await manager.GeneratePasswordResetTokenAsync(userId);
        }

        public async Task<IdentityResult> ValidateUserAsync(string userName, string password)
        {
            var user = await FindUser(userName);
            if (user == null)
                return IdentityResult.Failed();
            UserManager<MyTripUser> manager = new UserManager<MyTripUser>(userStore);
            var verificationResult = manager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if (verificationResult == PasswordVerificationResult.Failed)
                return IdentityResult.Failed();
            return IdentityResult.Success;
        }

        public async Task<bool> ResetPassword(string userId, string password, string token)
        {
            var result = await manager.ResetPasswordAsync(userId, token, password);
            return result.Succeeded;
        }
    }
}