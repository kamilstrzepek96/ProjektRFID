using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProjektSpoleczenstwo.Models;
using ProjektSpoleczenstwo.Models.Entities;

namespace ProjektSpoleczenstwo.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult CardRequest(int EmployeeId)
        {
            Employee employee = db.Employee.Find(EmployeeId);
            if (db.Employee.Where(x => x.Id != EmployeeId).Any(x => x.RegisterCard == true)) //allow only one
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); // mozna rejestrowac jednoczesnie tylko 1 karte
            }
            if (employee.RegisterCard == false)
            {
                employee.RegisterCard = true;
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {

                employee.RegisterCard = false;
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.Created);
            }


        }
        [AllowAnonymous]
        public ActionResult Punch(string UID)
        {
            //jesli karty nie ma zarejestrowanej i zaden uzytykownik nie chce rejestrować----
            if (db.Employee.Any(x => x.RegisterCard == true) && !db.Employee.Any(x => x.CardUID == UID))
            {
                Employee employee = db.Employee.SingleOrDefault(x => x.RegisterCard == true);
                employee.CardUID = UID;
                employee.RegisterCard = false;
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            Employee emp = db.Employee.SingleOrDefault(x => x.CardUID == UID);
            if (emp == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Conflict);
            }
            //jesli ktos pracuje za dlugo to ustawia mu i tak maksymalna godzine pracy?
            //jesli ktos sie nie odbil jednego dnia to drugiego dnia ustawia mu wyjscie na godzinę ukonczenia pracy w dbjob
            //check if punch today - if not, enum.in and set


            if (DateTime.Now.Hour < emp.Job.WorkFromTime.Hours && DateTime.Now.Minute < emp.Job.WorkFromTime.Minutes
                || DateTime.Now.Hour > emp.Job.WorkToTime.Hours && DateTime.Now.Minute > emp.Job.WorkToTime.Minutes)//nie mozna sie odbic, kiedy nie obowiazuja godziny pracy
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            WorkHours newPunch = new WorkHours()
            {
                PunchTime = DateTime.Now,
                EmployeeId = emp.Id
            };
            var punch = db.WorkHours.Where(x => x.EmployeeId == emp.Id).OrderBy(x => x.PunchTime).ToList().LastOrDefault();
            var currentHour = DateTime.Now.Hour;
            var currentMinutes = DateTime.Now.Minute;

            if (punch == null || punch.PunchType == PunchEnum.OUT)
            {
                newPunch.PunchType = PunchEnum.IN;
                newPunch.ElapsedTime = TimeSpan.Zero;
            }
            else if (punch.PunchType == PunchEnum.IN && punch.PunchTime < DateTime.Now.AddHours(-currentHour).AddMinutes(-currentMinutes))
            { // jesli ktos nie odbil sie kiedys to ustawia mu wyjscie na godzine o ktorej konczy prace jego stanowisko
                var oldPunchTime = new DateTime(punch.PunchTime.Year,
                                             punch.PunchTime.Month,
                                             punch.PunchTime.Day,
                                             emp.Job.WorkToTime.Hours,
                                             emp.Job.WorkToTime.Minutes,
                                             emp.Job.WorkToTime.Seconds);
                var LastIn = db.WorkHours
                               .Where(x => x.EmployeeId == emp.Id)
                               .Where(x => x.PunchType == PunchEnum.IN)
                               .OrderBy(x => x.PunchTime)
                               .ToList()
                               .LastOrDefault();
                TimeSpan elapsed = oldPunchTime.Subtract(LastIn.PunchTime);
                WorkHours yesterdayPunch = new WorkHours()
                {
                    EmployeeId = emp.Id,
                    PunchType = PunchEnum.OUT,
                    PunchTime = oldPunchTime,
                    ElapsedTime = elapsed
                };
                db.WorkHours.Add(yesterdayPunch);
                newPunch.PunchType = PunchEnum.IN;
                newPunch.ElapsedTime = TimeSpan.Zero;
                db.WorkHours.Add(newPunch);
                db.SaveChanges();
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else if (punch.PunchType == PunchEnum.IN)
            {
                var LastIn = db.WorkHours
                   .Where(x => x.EmployeeId == emp.Id)
                   .Where(x => x.PunchType == PunchEnum.IN)
                   .OrderBy(x => x.PunchTime)
                   .ToList()
                   .LastOrDefault();
                TimeSpan elapsed = DateTime.Now.Subtract(LastIn.PunchTime);
                newPunch.PunchType = PunchEnum.OUT;
                newPunch.ElapsedTime = elapsed;
            }
            db.WorkHours.Add(newPunch);
            db.SaveChanges();
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Manage");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Nieudana próba logowania.");
                    return View(model);
            }
        }
        //
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // For more information on how to enable account confirmation and password reset please visit https://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Manage");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Account");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}