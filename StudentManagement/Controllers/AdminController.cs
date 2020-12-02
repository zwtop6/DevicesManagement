using DeviceManagement.Models;
using DeviceManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<AdminController> logger;

        public AdminController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger<AdminController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }

        #region 角色管理

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                //如果您尝试床架具有已存在的同名的角色，则会收到验证错误
                IdentityResult result = await roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Admin");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }

        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;

            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            //通过角色Id查找角色
            var role = await roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色id为{id}的信息不存在，请重试。";
                return View("NotFound");
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            var users = userManager.Users.ToList();

            //遍历所有的用户
            foreach (var user in users)
            {
                //如果用户拥有此角色，将用户名添加到EditRoleViewModel模型中的Users属性中
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }

            //然后将对象传递给视图显示到客户端
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await roleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色id为{model.Id}的信息不存在，请重试。";
                return View("NotFound");
            }
            else
            {
                role.Name = model.RoleName;

                //使用异步更新
                var result = await roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        #region 角色中的用户

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            //通过id查询角色实体信息
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色id为{roleId}的信息不存在，请重视。";
                return View("NotFound");
            }

            var model = new List<UserRoleViewModel>();

            //将所有用户数据查询到内容中
            var users = userManager.Users.ToList();

            foreach (var user in users)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                //判断当前用户是否已经存在于角色中
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            //检查当前角色是否存在
            var role = await roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"角色id为{roleId}的信息不存在，请重视。";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                bool isInRole = await userManager.IsInRoleAsync(user, role.Name);

                IdentityResult result = null;

                //被选中，不属于该角色，这个时候，添加到角色中
                if (model[i].IsSelected && !isInRole)
                {

                    result = await userManager.AddToRoleAsync(user, role.Name);
                }
                //没有被选中，但是用户已经在角色中，移除出来
                else if (!model[i].IsSelected && isInRole)
                {

                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                //被选中，已经存在角色中，不发生任何改变的数据
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    //判断当前用户是否为最后一个用户，如果是则跳转回EditRole视图，如果不是则进入下一个循环
                    if (i < model.Count - 1)
                    {
                        continue;
                    }
                    else
                    {
                        return RedirectToAction("EditRole", new { id = roleId });
                    }
                }
            }

            return RedirectToAction("EditRole", new { id = roleId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的角色";
                return View("NotFound");
            }
            else
            {
                try
                {
                    var result = await roleManager.DeleteAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListRoles");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View("ListRoles");
                }
                catch (DbUpdateException ex)
                {
                    logger.LogError($"发生异常:{ex}");

                    //我们使用ViewBag来传递信息到Error视图
                    //Error试图将这些内容显示给用户
                    ViewBag.ErrorTitle = $"角色：{role.Name}正在被使用中...";
                    ViewBag.ErrorMessage = $"无法删除{role.Name}角色，因为此角色中已经存在用户。如果读者想删除此角色，" +
                        $"需要先从该角色中删除用户，然后尝试删除该角色本身。";
                    
                    return View("Error");
                }
            }
        }

        #endregion

        #endregion

        #region 用户管理

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = userManager.Users.ToList();

            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID{id}的用户";
                return View("NotFound");
            }

            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                City = user.City,
                Claims = userClaims.Select(c => c.Value).ToList(),
                Roles = userRoles,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    ViewBag.ErrorMessage = $"无法找到ID{model.Id}的用户";
                    return View("NotFound");
                }
                else
                {
                    user.Email = model.Email;
                    user.UserName = model.UserName;
                    user.City = model.City;

                    var result = await userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("ListUsers");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"无法找到ID为{id}的用户";
                return View("NotFound");
            }
            else
            {
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListUsers");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListUsers");
            }
        }

        #endregion




    }
}
