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

namespace MyTrip.MyTripLogic.Repositories
{
    public class AuthRepository
    {
        private readonly DocumentDBUserStore<MyTripUser> userStore;

        public AuthRepository() 
        {
            var identityCollectionManager= new IdentityCollectionManager<MyTripUser>(DocumentDb.Client, DocumentDb.Database, true);
            var identityRoleStore = new IdentityRoleStore();
            this.userStore= new DocumentDBUserStore<MyTripUser>(identityCollectionManager, identityRoleStore); 
        }

        public async Task<IdentityResult> RegisterUserAsync(UserRegistrationModel registrationModel)
        {
            var existingUser = await FindUser(registrationModel.UserName);
            if (existingUser != null)
                return IdentityResult.Failed("User with this username already exists");
            UserManager<MyTripUser> manager=new UserManager<MyTripUser>(userStore);
            var user = new MyTripUser()
            {
                UserName = registrationModel.UserName,
                PasswordHash = manager.PasswordHasher.HashPassword(registrationModel.Password),
                Email = registrationModel.Email
            };
            await userStore.CreateAsync(user);
            return IdentityResult.Success;
        }


        public async Task<IdentityUser> FindUser(string userName)
        {
            var user = await userStore.FindByNameAsync(userName);
            return user;
        }

        public async Task<IdentityResult> ValidateUserAsync(string userName, string password)
        {
            var user = await FindUser(userName);
            if (user == null)
                return IdentityResult.Failed();
            UserManager<MyTripUser> manager = new UserManager<MyTripUser>(userStore);
            var verificationResult = manager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, password);
            if(verificationResult == PasswordVerificationResult.Failed)
                return IdentityResult.Failed();
            return IdentityResult.Success;
        }
    }
}