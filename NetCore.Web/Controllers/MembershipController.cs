using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Web.Models;

namespace NetCore.Web.Controllers
{
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUserm, SystemUser")]
    public class MembershipController : Controller
    {
        //dependency injection  - constructor
        private readonly IUser _user;
        private readonly IPasswordHasher _hasher;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IPasswordHasher hasher, IUser user)
        {
            _context = accessor.HttpContext;
            _hasher = hasher;
            _user = user;

        }
        #region
        /// <summary>
        /// Local URL || External URL
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(MembershipController.Index), "Membership");
            }
        }
        #endregion

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost("/Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        //data => services => web
        public async Task<IActionResult> LoginAsync(LoginInfo login, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                // viewmodel
                // services
                if(_user.MatchTheUserInfo(login))
                //if(_hasher.MatchTheUserInfo(login.UserId, login.Password))
                {
                    // Authentication | Authorization
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwendByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" +
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type: ClaimTypes.Name,
                                  value:userInfo.UserId),
                        new Claim(type: ClaimTypes.Role,
                                  value:userTopRole.RoleId),
                        new Claim(type: ClaimTypes.UserData,
                                  value: userDataInfo)
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal: new ClaimsPrincipal(identity: identity),
                                               properties: new AuthenticationProperties()
                                               {
                                                   IsPersistent = login.RememberMe,
                                                   ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                               });
                    TempData["Message"] = "You successed the login";
                    //return RedirectToAction("Index", "Membership");
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "you are not logged in the system";
                }
                
            }
            else
            {
                message = "Please input right info.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View("Login", login);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public IActionResult Register(RegisterInfo register, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            string message = string.Empty;
            if(ModelState.IsValid)
            {
                if(_user.RegisterUser(register) > 0)
                {
                    //Register Service
                    TempData["message"] = "You successed the register";
                    return RedirectToAction("Login", "Membership");
                }
                else
                {
                    message = "You are not registered in the system";
                }
            }
            else
            {
                message = "You must input all of details correctly";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(register);
        }
        
        [HttpGet]
        public IActionResult UpdateInfo()
        {
            UserInfo user = _user.GetUserInfoForUpdate(_context.User.Identity.Name);

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateInfo(UserInfo user)
        {
            string message = string.Empty;
            if(ModelState.IsValid)
            {
                // compare changes
                if(_user.CompareInfo(user))
                {
                    message = "You must update the informaiton at least one.";
                    ModelState.AddModelError(string.Empty, message);
                    return View(user);
                }
                // update service
                if (_user.UpdateUser(user) > 0)
                {
                    TempData["Message"] = "You successed the change of information details";
                    return RedirectToAction("UpdateInfo", "Membership");
                }
                else
                {
                    message = "Your information is not updated";
                }
            }
            else
            {
                message = "You must input all of information for information update";
            }
            ModelState.AddModelError(string.Empty, message);
            return View(user);
        }

        [HttpGet("/Logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "Logout is successed <br /> Please login the system"; 
            return RedirectToAction("Index", "Membership");
        }

        [HttpPost("/Withdrawn")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WithdrawnAsync(WithdrawnInfo withdrawn)
        {
            string message = string.Empty;
            if(ModelState.IsValid)
            {
                // withdrawn service
                if(_user.WithdrawnUser(withdrawn) > 0)
                {
                    TempData["Message"] = "You successed the withdrawn the account";

                    await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);

                    return RedirectToAction("Index", "Membership");
                }
                else
                {
                    message = "The processing of withdrawn the account is not worked";
                }
            }
            else
            {
                message = "You must input the information to witdraw the account";
            }
            ViewData["Message"] = message;
            return View(withdrawn);
        }

        [HttpGet]
        public IActionResult Forbidden([FromServices] ILogger<MembershipController> logger)
        {
            StringValues paramReturnUrl;
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            logger.LogTrace($"{MethodBase.GetCurrentMethod().Name}.You have no authrization for this webpage. returnUrl: {paramReturnUrl}");

            ViewData["Message"] = $"You wanted to connect {paramReturnUrl}. <br />" +
                                    "However, your approach was denied. <br />" +
                                    "you must send the mail to administrator to get the authorization.";
            return View();
        }
    }
}