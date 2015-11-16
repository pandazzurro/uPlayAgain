using uPlayAgain.Entities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uPlayAgain.Models;
using System.Web.Mvc;
using uPlayAgain.Utilities;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity.Validation;
using System.Text;
using System.Web;
using System.IO;

namespace uPlayAgain
{

    public class AuthRepository : IDisposable
    {
        private uPlayAgainContext _ctx;
        private UserManager<User> _userManager;
        private NLog.Logger _log = NLog.LogManager.GetLogger("uPlayAgain");

        public AuthRepository()
        {
            _ctx = new uPlayAgainContext();
            _userManager = new ApplicationUserManager(new UserStore<User>(_ctx));
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IdentityResult> RegisterUser(UserLogin login)
        {
            IdentityResult result = null;
            User user = new User()
            {
                UserName = login.Username,
                Email = login.Email,
                Image = login.Image,
                PositionUser = login.PositionUser,
                LastLogin = DateTimeOffset.Now              
            };
            
            try
            {
                result = await _userManager.CreateAsync(user, login.Password);                
                if (result.Succeeded)
                {
                    string token = await _userManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    string encode = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(token));
                    // Settare il callback url che abilita l'utente.
                    Uri callbackUrl = new Uri(String.Format("{0}/mail-activation.html?userId={1}&token={2}", HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath.ToString(), ""), user.Id, encode));

                    // Caricare il testo della mail e riempire i dati
                    string mailText = File.ReadAllText(HttpContext.Current.Server.MapPath("~/MailTemplate/template.html"));
                    mailText = mailText.Replace("{URL}", callbackUrl.ToString());
                    mailText = mailText.Replace("{USER}", user.UserName);
                    mailText = mailText.Replace("{USERPIC}", Convert.ToBase64String(user.Image));                    

                    await _userManager.SendEmailAsync(user.Id,"UplayAgain ti da il benvenuto. Conferma la tua password!", mailText);
                }
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();
                ex.EntityValidationErrors.ToList().ForEach(entityValidation => { entityValidation.ValidationErrors.ToList().ForEach(validation => sb.Append(string.Concat(validation.PropertyName, " - ", validation.ErrorMessage))); });

                _log.Error("{0}{1}Validation errors:{1}{2}", ex, Environment.NewLine, sb.ToString());
                throw;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
            return result;
        }

        public async Task<bool> ValidateMailTokenAndConfirm(string userId, string token)
        {
            try
            {
                IdentityResult result =  await _userManager.ConfirmEmailAsync(userId, token);
                if(result.Errors.Any())
                {
                    _log.Error(string.Concat(result.Errors));
                    return false;
                }
                return result.Succeeded;
            }
            catch(Exception ex)
            {
                _log.Error(ex);
                return false;
            }            
        }
        public async Task<IList<string>> ValidateMailTokenMessages(string userId, string token)
        {
            IdentityResult result = await _userManager.ConfirmEmailAsync(userId, token);
            return result.Errors.ToList();
        }

        [AllowAnonymous]
        public async Task GeneratePasswordResetTokenAsync(string email)
        {
            User user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                string token = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
                string encode = HttpServerUtility.UrlTokenEncode(Encoding.UTF8.GetBytes(token));
                // Settare il callback url che abilita l'utente.
                Uri callbackUrl = new Uri(String.Format("{0}/mail-reset-password.html?userId={1}&token={2}", HttpContext.Current.Request.Url.AbsoluteUri.Replace(HttpContext.Current.Request.Url.AbsolutePath.ToString(), ""), user.Id, encode));

                // Caricare il testo della mail e riempire i dati
                string mailText = File.ReadAllText(HttpContext.Current.Server.MapPath("~/MailTemplate/resetPassword.html"));
                mailText = mailText.Replace("{URL}", callbackUrl.ToString());
                mailText = mailText.Replace("{USER}", user.UserName);
                mailText = mailText.Replace("{USERPIC}", Convert.ToBase64String(user.Image));

                await _userManager.SendEmailAsync(user.Id, "UplayAgain ti da il benvenuto. Conferma la tua password!", mailText);
            }
        }

        [AllowAnonymous]        
        public async Task ResetPasswordByToken(string userId, string token, string newPassword)
        {
            try
            {
                User u = await _userManager.FindByIdAsync(userId);
                if(u != null)
                {
                    await _userManager.ResetPasswordAsync(u.Id, token, newPassword);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }            
        }
        public async Task<User> FindUser(string userName, string password)
        {
            User user = await _userManager.FindAsync(userName, password);
            return null;
        }

        public async Task<bool> IsEmailConfirmedAsync(string userId)
        {
            return await _userManager.IsEmailConfirmedAsync(userId);
        }

        public async Task<User> FindByIdAsync(string userId)
        {
            User user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        public async Task<User> RefreshUser(string userName, string password)
        {
            User user = await _userManager.FindAsync(userName, password);
            if (user != null)
            {
                DateTimeOffset now = DateTimeOffset.Now;
                // impongo un orario di aggiornamento se dal Json non arriva.
                if (user.LastLogin == DateTimeOffset.MinValue || user.LastLogin < now)
                    user.LastLogin = now;

                await _userManager.UpdateAsync(user);
            }

            return user;
        }

        public Client FindClient(string clientId)
        {
            var client = _ctx.Clients.Find(clientId);

            return client;
        }

        public async Task<bool> AddRefreshToken(RefreshToken token)
        {

           var existingToken = _ctx.RefreshTokens.Where(r => r.Subject == token.Subject && r.ClientId == token.ClientId).SingleOrDefault();

           if (existingToken != null)
           {
             var result = await RemoveRefreshToken(existingToken);
           }
          
            _ctx.RefreshTokens.Add(token);

            return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveRefreshToken(string refreshTokenId)
        {
           var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

           if (refreshToken != null) {
               _ctx.RefreshTokens.Remove(refreshToken);
               return await _ctx.SaveChangesAsync() > 0;
           }

           return false;
        }

        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
             return await _ctx.SaveChangesAsync() > 0;
        }

        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            var refreshToken = await _ctx.RefreshTokens.FindAsync(refreshTokenId);

            return refreshToken;
        }

        public List<RefreshToken> GetAllRefreshTokens()
        {
             return  _ctx.RefreshTokens.ToList();
        }

        public async Task<User> FindAsync(UserLoginInfo loginInfo)
        {
            User user = await _userManager.FindAsync(loginInfo);

            return user;
        }

        public async Task<IdentityResult> CreateAsync(User user)
        {
            var result = await _userManager.CreateAsync(user);

            return result;
        }

        public async Task<IdentityResult> AddLoginAsync(string userId, UserLoginInfo login)
        {
            var result = await _userManager.AddLoginAsync(userId, login);

            return result;
        }

        public void Dispose()
        {
            _ctx.Dispose();
            _userManager.Dispose();

        }
    }
}