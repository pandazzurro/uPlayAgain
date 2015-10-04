using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using uPlayAgain.Models;
using uPlayAgain.Results;
using System.Web;
using System.Text;
using uPlayAgain.Utilities;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Validation;

namespace uPlayAgain.Controllers
{
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;
        private uPlayAgainContext db;
        private UserManager<User> _userManager;
        private SignInManager<User, string> _signInManager;
        private IAuthenticationManager Authentication
        {
            get
            {
                if (Request != null)
                    return Request.GetOwinContext().Authentication;
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }
        private NLog.Logger _log = NLog.LogManager.GetLogger("uPlayAgain");

        public AccountController()
        {
            _repo = new AuthRepository();
            db = new uPlayAgainContext();
            _userManager = new ApplicationUserManager(new UserStore<User>(db));
            _signInManager = new SignInManager<User, string>(_userManager, Authentication);
        }

        // POST api/Account/Login
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IHttpActionResult> Login(UserLogin user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifica mail abilitata
            User userLogin = await _userManager.FindAsync(user.Username, user.Password);
            if (userLogin != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(userLogin.Id))
                {
                    return BadRequest("L'utente non ha ancora confermato l'indirizzo mail. Il login non può essere effettuato");
                }
            }
            
            // Login effettivo
            SignInStatus status = await _signInManager.PasswordSignInAsync(user.Username, user.Password, false, true);
            if (status == SignInStatus.LockedOut)
                return BadRequest("L'utente risulta bloccato");
            if (status == SignInStatus.Success)
            {
                await SignInAsync(userLogin, false);
                
                if (userLogin.LastLogin == DateTimeOffset.MinValue || userLogin.LastLogin < DateTimeOffset.Now)
                    userLogin.LastLogin = DateTimeOffset.Now;

                db.Entry(userLogin).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return Ok(userLogin);
            }
            if (status == SignInStatus.Failure)
            {
                return BadRequest("Login errato");
            }
            if (status == SignInStatus.RequiresVerification)
            {
                // TODO: redirect sulla pagina di rigenerazione del token
                //string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                //await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                //return RedirectToAction("ForgotPasswordConfirmation", "Account");
                return BadRequest("L'utente non ha ancora confermato l'indirizzo mail. Il login non può essere effettuato");
            }

            return NotFound();
        }

        
        private async Task SignInAsync(User user, bool isPersistent)
        {
            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            ClaimsIdentity identity = await _userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            Authentication.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
        }

        [AllowAnonymous]
        [Route("Logout")]
        public async Task<IHttpActionResult> Logout(UserLogin user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            User userLogOut = await _userManager.FindAsync(user.Username, user.Password);
            if (userLogOut != null)
            {
                // LogOut
                await SignOutAsync(userLogOut);
                return Ok();
            }
            return BadRequest("Utente non esistente");
        }

        private async Task SignOutAsync(User user)
        {
            await Task.Run(() => {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            });
        }

        [HttpGet]
        [Route("ValidateMail/{userId}/{token}")]
        public async Task<IHttpActionResult> ValidateMailConfirmationToken(string userId, string token)
        {
            string decode = Encoding.Default.GetString(HttpServerUtility.UrlTokenDecode(token));
            bool result = await _repo.ValidateMailTokenAndConfirm(userId, decode);
            if (result)
            {
                User newUser = null;
                try
                {
                    newUser = await _repo.FindByIdAsync(userId);
                    // Aggiungo una libreria all'utente
                    if (await db.Libraries.Where(p => p.UserId == newUser.Id).CountAsync() == 0)
                    {
                        db.Libraries.Add(new Library() { User = newUser });

                        // Non aggiungere un nuovo utente al DB.
                        db.Entry(newUser).State = EntityState.Unchanged;
                        // Non aggiungere un nuovo utente al DB.

                        await db.SaveChangesAsync();
                    }
                    return Ok();
                }
                catch (Exception ex)
                {
                    _log.Error(string.Concat(newUser.Id, ex.Message, ex.InnerException));
                    return BadRequest("Errore del server: " + ex.Message);
                }
            }
            else
            {
                IList<string>Errors = await _repo.ValidateMailTokenMessages(userId, decode);
                string errorsResult = string.Empty;
                Errors.ToList().ForEach(error => { errorsResult = string.Concat(errorsResult, error); });
                return BadRequest("Errore, token non valido per questo utente. Generare un nuovo token");
            }
                
        }
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(UserLogin userModel) // UserLogin
        {
            try
            {
                IdentityResult result = await _repo.RegisterUser(userModel);
                IHttpActionResult errorResult = GetErrorResult(result);
                if (errorResult != null)
                {
                    return errorResult;
                }
                else
                    return Ok();
            }
            catch(Exception ex)
            {
                _log.Error(ex);
                return BadRequest("Errore" + ex.Message);
            }                
        }

        // POST api/Account/RegisterExternal
        [AllowAnonymous]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            User user = await _repo.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            user = new User() { UserName = model.UserName };

            IdentityResult result = await _repo.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            var info = new ExternalLoginInfo()
            {
                DefaultUserName = model.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            result = await _repo.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            //generate access token response
            var accessTokenResponse = GenerateLocalAccessTokenResponse(model.UserName);

            return Ok(accessTokenResponse);
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            IdentityUser user = await _repo.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_user_name={4}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered.ToString(),
                                            externalLogin.UserName);

            return Redirect(redirectUri);

        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {

            Uri redirectUri;

            var redirectUriString = GetQueryString(Request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(Request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _repo.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;

        }

        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                //You can get it from here: https://developers.facebook.com/tools/accesstoken/
                //More about debug_tokn here: http://stackoverflow.com/questions/16641083/how-does-one-get-the-app-access-token-for-debug-token-inspection-on-facebook

                var appToken = "xxxxx";
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, appToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (provider == "Facebook")
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    if (!string.Equals(Startup.facebookAuthOptions.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
                else if (provider == "Google")
                {
                    parsedToken.user_id = jObj["user_id"];
                    parsedToken.app_id = jObj["audience"];

                    if (!string.Equals(Startup.googleAuthOptions.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                }

            }

            return parsedToken;
        }

        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => string.Compare(keyValue.Key, key, true) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }

        private JObject GenerateLocalAccessTokenResponse(string userName)
        {

            var tokenExpiration = TimeSpan.FromDays(1);

            ClaimsIdentity identity = new ClaimsIdentity(OAuthDefaults.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, userName));
            identity.AddClaim(new Claim("role", "user"));

            var props = new AuthenticationProperties()
            {
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.Add(tokenExpiration),
            };

            var ticket = new AuthenticationTicket(identity, props);

            var accessToken = Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);

            JObject tokenResponse = new JObject(
                                        new JProperty("userName", userName),
                                        new JProperty("access_token", accessToken),
                                        new JProperty("token_type", "bearer"),
                                        new JProperty("expires_in", tokenExpiration.TotalSeconds.ToString()),
                                        new JProperty(".issued", ticket.Properties.IssuedUtc.ToString()),
                                        new JProperty(".expires", ticket.Properties.ExpiresUtc.ToString())
            );

            return tokenResponse;
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
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
