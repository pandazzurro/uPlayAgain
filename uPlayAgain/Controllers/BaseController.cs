using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using uPlayAgain.Data.EF.Context;
using NLog;
using Microsoft.AspNet.Identity;
using uPlayAgain.Data.EF.Models;
using Microsoft.Owin.Security;
using System.Net.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using uPlayAgain.Utilities;

namespace uPlayAgain.Controllers
{
    public class BaseController : ApiController
    {
        protected uPlayAgainContext db;
        protected Logger _log;
        protected AuthRepository _repo = null;
        protected UserManager<User> _userManager;
        protected IAuthenticationManager Authentication
        {
            get
            {
                if (Request != null)
                    return Request.GetOwinContext().Authentication;
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public BaseController()
        {
            db = new uPlayAgainContext();
            _log = LogManager.GetLogger("uPlayAgain");
            _userManager = new ApplicationUserManager(new UserStore<User>(db));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}