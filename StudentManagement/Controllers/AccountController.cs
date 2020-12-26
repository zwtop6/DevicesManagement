using DeviceManagement.Models;
using DeviceManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManagement.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        #region 注册

        [HttpGet]
        public IActionResult Register()
        {
            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                //将数据从RegisterViewModel赋值到ApplicationUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    City = model.City
                };

                //将用户数据存储在AspNetUser数据库表中
                var result = await userManager.CreateAsync(user, model.Password);

                //如果成功创建用户，则使用登录服务登录用户信息
                //并重定向到home controller的索引操作
                if (result.Succeeded)
                {
                    //生成电子邮箱的令牌
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                    //生成电子邮件的确认链接
                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = token }, Request.Scheme);

                    //需要注入ILogger<AccountController> _logger;服务，记录生成的URL链接
                    logger.Log(LogLevel.Warning, confirmationLink);


                    //如果用户已登录并属于Admin角色
                    //那么就是Admin正在创建新用户
                    //所以重定向Admin用户对ListRoles的试图列表
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("ListUsers", "Admin");
                    }

                    StringBuilder strB = new StringBuilder();
                    strB.AppendLine("在你登入系统前,我们已经给您发了一份邮件，需要您先进行邮件验证，点击确认链接即可完成。");
                    strB.AppendLine("链接为：");
                    strB.AppendLine(confirmationLink.ToString());

                    ViewBag.ErrorTitle = "注册成功";
                    ViewBag.ErrorMessage = strB.ToString();

                    return View("Error");

                    //await signInManager.SignInAsync(user, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                }

                //如果有任何错误，将它们添加到ModelState对象中
                //将由验证摘要标记助手显示到试图中
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        #endregion

        #region 登录

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
                {
                    ModelState.AddModelError(string.Empty, "你的邮箱还没有通过验证，请前往验证。");
                    return View(model);
                }

                //PasswordSignInAsync()我们将最后一个参数从false修改为了true,用于启用账户锁定。

                //每次登录失败后，都会将AspNetUsers表中的AccessFailedCount列值增加1。当它等于5时，
                //MaxFailedAccessAttempts将会锁定账户，然后修改LockoutEnd列,添加解锁时间。
                //即使我们提供正确的用户名和密码， PasswordSignInAsync()方法的返回值依然是Lockedout即被锁定。
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, true);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        //防止开放式重定向攻击
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "home");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Index", "home");
                    }
                }

                if (result.IsLockedOut)
                {
                    return View("AccountLocked");
                }

                ModelState.AddModelError(string.Empty, "登录失败，请重试");
            }

            return View(model);
        }

        #endregion


        #region 注销

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }

        #endregion

        #region 验证账号是否存在

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"邮箱：{email}已经被注册使用了。");
            }
        }

        #endregion

        #region 确认邮箱

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"当前{userId}无效";
                return View("NotFound");
            }
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }

            ViewBag.ErrorTitle = "您的电子邮箱还未进行验证";
            return View("Error");
        }

        #endregion 确认邮箱

        #region 激活邮箱

        [HttpGet]
        public IActionResult ActivateUserEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ActivateUserEmail(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (!await userManager.IsEmailConfirmedAsync(user))
                    {
                        //生成电子邮件确认令牌
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

                        //生成电子邮件的确认链接
                        var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, Request.Scheme);

                        logger.Log(LogLevel.Warning, confirmationLink);
                        ViewBag.Message = "如果你在我们系统有注册账户，我们已经发了邮件到您的邮箱中，请前往邮箱激活您的用户。";

                        //重定向用户到忘记密码确认视图
                        return View("ActivateUserEmailConfirmation", ViewBag.Message);
                    }
                }

                ViewBag.Message = "请确认邮箱是否存在异常，现在我们无法给您发送激活链接。";

                // 为了避免帐户枚举和暴力攻击，所以不进行用户不存在或邮箱未验证的提示
                return View("ActivateUserEmailConfirmation", ViewBag.Message);
            }

            return View();
        }

        #endregion 激活邮箱

        #region 找回密码 & 重置密码

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(EmailAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过邮件地址查询用户地址
                var user = await userManager.FindByEmailAsync(model.Email);
                // 如果找到了用户并且确认了电子邮件
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    //生成重置密码令牌
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    // 生成密码重置链接
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    // 将密码重置链接记录到文件中
                    logger.Log(LogLevel.Warning, passwordResetLink);
                    //重定向用户到忘记密码确认视图
                    return View("ForgotPasswordConfirmation");
                }

                // 为了避免帐户穷举和暴力攻击，所以不进行用户不存在或邮箱未验证的提示
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            //如果密码的token或者邮箱地址为空，用户有可能在试图篡改密码重置的URL
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "当前的密码重置令牌无效");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 通过电子邮件查找用户
                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    //重置用户密码
                    var result = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        //密码成功重置后，如果当前账户被锁定，则设置该账户锁定结束日期为当前UTC日期时间。
                        //这样用户就可以用新密码登录系统。
                        if (await userManager.IsLockedOutAsync(user))
                        {
                            await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                            //DateTimeOffset指是UTC时间即格林威治时间。
                        }
                        return View("ResetPasswordConfirmation");
                    }

                    //告诉它验证不通过的错误信息
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }

                // 为了避免帐户穷举和暴力攻击，不要提示用户不存在
                return View("ResetPasswordConfirmation");
            }
            // 如果模型验证未通过，则显示验证错误
            return View(model);
        }

        #endregion 找回密码& 重置密码

        #region 修改密码
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var user = await userManager.GetUserAsync(User);

            //判断当前用户是否拥有密码，如果没有重定向到添加密码视图
            var userHasPassword = await userManager.HasPasswordAsync(user);

            if (!userHasPassword)
            {
                return RedirectToAction("AddPassword");
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.ConfirmPassword);
                //如果新密码不符合复杂性规则或当前密码不正确，我们需要将错误提示，返回到ChangePassword视图页面中
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await signInManager.RefreshSignInAsync(user);

                return View("ChangePasswordConfirmation");
            }

            return View(model);
        }

        #endregion 修改密码

        //#region 添加密码功能(扩展登录）

        //[HttpGet]
        //public async Task<IActionResult> AddPassword()
        //{
        //    var user = await userManager.GetUserAsync(User);

        //    var userHasPassword = await userManager.HasPasswordAsync(user);

        //    if (userHasPassword)
        //    {
        //        return RedirectToAction("ChangePassword");
        //    }
        //    return View();
        //}


        //[HttpPost]
        //public async Task<IActionResult> AddPassword(AddPasswordViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var user = await userManager.GetUserAsync(User);
        //        //为用户添加密码
        //        var result = await userManager.AddPasswordAsync(user, model.NewPassword);

        //        if (!result.Succeeded)
        //        {
        //            foreach (var error in result.Errors)
        //            {
        //                ModelState.AddModelError(string.Empty, error.Description);
        //            }
        //            return View();
        //        }

        //        //刷新当前用户的Cookie
        //        await signInManager.RefreshSignInAsync(user);

        //        return View("AddPasswordConfirmation");

        //    }

        //    return View(model);
        //}

        //#endregion

    }
}
