using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using uPlayAgain.Models;

namespace uPlayAgain.Utilities
{
    public class ApplicationUserManager : UserManager<User>
    {

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<User>(context.Get<uPlayAgainContext>()));
            return manager;
        }

        public ApplicationUserManager(IUserStore<User> store) : base(store)
        {   
            // Configure validation logic for usernames
            UserValidator = new UserValidator<User>(this) { AllowOnlyAlphanumericUserNames = false, RequireUniqueEmail = true };

            // Configure validation logic for passwords
            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6
            };

            // Configure user lockout defaults
            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            MaxFailedAccessAttemptsBeforeLockout = 5;

            // Register two factor authentication providers.
            //manager.RegisterTwoFactorProvider("PhoneCode",
            //    new PhoneNumberTokenProvider<User>
            //    {
            //        MessageFormat = "Your security code is: {0}"
            //    });

            //manager.RegisterTwoFactorProvider("EmailCode",
            //    new EmailTokenProvider<User>
            //    {
            //        Subject = "SecurityCode",
            //        BodyFormat = "Your security code is {0}"
            //    });
            PasswordHasher = new PasswordHasher();
            EmailService = new EmailService();
            UserTokenProvider = new DataProtectorTokenProvider<User>(Startup.DataProtectionProvider.Create("Confirmation"));
        }       

        public virtual async Task<IdentityResult> AddUserToRolesAsync(string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<User, string>)Store;
            var user = await FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);

            // Add user to each role using UserRoleStore
            foreach (var role in roles.Where(role => !userRoles.Contains(role)))
            {
                await userRoleStore.AddToRoleAsync(user, role).ConfigureAwait(false);
            }

            // Call update once when all roles are added
            return await UpdateAsync(user).ConfigureAwait(false);
        }

        public virtual async Task<IdentityResult> RemoveUserFromRolesAsync(string userId, IList<string> roles)
        {
            var userRoleStore = (IUserRoleStore<User, string>)Store;
            var user = await FindByIdAsync(userId).ConfigureAwait(false);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid user Id");
            }

            var userRoles = await userRoleStore.GetRolesAsync(user).ConfigureAwait(false);

            // Remove user to each role using UserRoleStore
            foreach (var role in roles.Where(userRoles.Contains))
            {
                await userRoleStore
                    .RemoveFromRoleAsync(user, role)
                    .ConfigureAwait(false);
            }

            // Call update once when all roles are removed
            return await UpdateAsync(user).ConfigureAwait(false);
        }
    }
}