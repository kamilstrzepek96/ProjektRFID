﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ProjektSpoleczenstwo.Models;
using ProjektSpoleczenstwo.Models.Entities;
using ProjektSpoleczenstwo.ViewModels;

namespace ProjektSpoleczenstwo.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        ApplicationDbContext db = new ApplicationDbContext();

        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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

        //
        // GET: /Manage/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePasswordSuccess ? "Hasło zostało zmienione."
                : message == ManageMessageId.Error ? "Wystąpił błąd."
                : "";

            var userId = User.Identity.GetUserId();
            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                Logins = await UserManager.GetLoginsAsync(userId),
            };
            return View(model);
        }

        public ActionResult RegisterCard()
        {
            List<UserNameViewModel> usersList = db.Employee.Select(x => new UserNameViewModel { Name = x.Name + " " + x.Surname, CardRequest = x.RegisterCard, Id = x.Id, CardUID = String.IsNullOrEmpty(x.CardUID) ? "brak karty" : x.CardUID }).ToList();

            return View(usersList);
        }
        [HttpGet]
        public ActionResult Summary()
        {
            DateTime ThisMonth = new DateTime(DateTime.Now.Year,DateTime.Now.Month,1);
            List<WorkHours> HoursList = db.WorkHours.Where(x=>x.PunchTime >= ThisMonth).ToList();
            List<SummaryViewModel> summary = new List<SummaryViewModel>();
            foreach (var emp in db.Employee.ToList())
            {
                
                var workhrs = HoursList.Where(x=>x.EmployeeId == emp.Id).Select(x=>x.ElapsedTime).Aggregate(TimeSpan.Zero, (subtotal, t) => subtotal.Add(t));
                summary.Add(new SummaryViewModel() {
                    Employee = emp.Name+" "+emp.Surname,
                    Salary = workhrs.Hours*emp.Job.Salary,
                    Department = emp.Department.Name,
                    Job = emp.Job.Title,
                    WorkHours = workhrs.Hours
                });
            }
            return View(summary);
        }

        public ActionResult CreateJob()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateJob(JobViewModel job)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Nieprawidłowe dane";
                return View();
            }
            if (db.Jobs.Any(x => x.Title == job.Title))
            {
                ViewBag.Error = "Stanowisko już istnieje";
                return View();
            }
            Jobs newJob = new Jobs()
            {
                Title = job.Title,
                Salary = job.Salary,
                WorkFromTime = job.FromTime,
                WorkToTime = job.ToTime
            };

            db.Jobs.Add(newJob);
            db.SaveChanges();
            ViewBag.Success = "Operacja wykonana pomyślnie";

            return View();
        }

        public ActionResult CreateDepartment()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateDepartment(DepartmentViewModel department)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Nieprawidłowe dane";
                return View();
            }
            if (db.Departments.Any(x => x.Name == department.Name))
            {
                ViewBag.Error = "Oddział już istnieje";
                return View();
            }
            Department newDep = new Department()
            {
                Name = department.Name,
                Location = department.Location
            };

            db.Departments.Add(newDep);
            db.SaveChanges();
            ViewBag.Success = "Operacja wykonana pomyślnie";
            return View();
        }

        public ActionResult Activity()
        {
            PunchIn hours = new PunchIn()
            {
                WorkHours = db.WorkHours.OrderBy(x => x.PunchTime).ToList()
            };
            return View(hours);
        }

        public ActionResult CreateEmployee()
        {
            List<SelectListItem> deps = db.Departments.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();
            List<SelectListItem> jobs = db.Jobs.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Title
            }).ToList();
            EmployeeViewModel employee = new EmployeeViewModel()
            {
                Deps = deps,
                Jobs = jobs
            };

            return View(employee);
        }

        [HttpPost]
        public ActionResult CreateEmployee(EmployeeViewModel employee)
        {
            if (!ModelState.IsValid)
            {
                //ViewBag.Error = "Nieprawidłowe dane";
                return View(employee);
            }
            Employee newEmployee = new Employee()
            {
                Name = employee.Name,
                Surname = employee.Surname,
                Age = employee.Age,
                //Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                Department = db.Departments.Find(employee.DepartmentId),
                Job = db.Jobs.Find(employee.JobId)
            };
            db.Employee.Add(newEmployee);
            db.SaveChanges();
            ViewBag.Succes = "Użytkownik został dodany";
            return View(employee);
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Manage/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }
        //
        // GET: /Manage/ManageLogins
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
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

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}