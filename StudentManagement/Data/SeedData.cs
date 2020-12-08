using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DeviceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagement.Data
{
    public static class SeedData
    {
        public static IApplicationBuilder UseDataInitializer(
           this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                var dbcontext = scope.ServiceProvider.GetService<AppDbContext>();

                var userManager = scope.ServiceProvider.GetService<UserManager<ApplicationUser>>();
                System.Console.WriteLine("开始执行迁移数据库...");

                dbcontext.Database.Migrate();
                System.Console.WriteLine("数据库迁移完成...");

                #region 初始化设备数据

                if (!dbcontext.Devices.Any())
                {
                    System.Console.WriteLine("开始创建种子数据中...");

                    dbcontext.Devices.Add(new Device
                    {
                        Id = 1,
                        Name = "PS2-0002",
                        ClassName = ClassNameEnum.PS,
                        City = "北京",
                    });
                    dbcontext.Devices.Add(new Device
                    {
                        Id = 2,
                        Name = "PH1-0004",
                        ClassName = ClassNameEnum.PH,
                        City = "陕西",
                    });
                }

                #endregion 初始化设备数据

                #region 初始化用户

                var adminUser = dbcontext.Users.FirstOrDefault(a => a.UserName == "897289382@qq.com");

                if (adminUser == null)
                {
                    var user = new ApplicationUser
                    {
                        UserName = "897289382@qq.com",
                        Email = "897289382@qq.com",
                        EmailConfirmed = true,
                        City = "山西",
                    };

                    var identityResult = userManager.CreateAsync(user, "zhangwei66").GetAwaiter().GetResult();

                    var role = dbcontext.Roles.Add(new IdentityRole
                    {
                        Name = "Admin",
                        NormalizedName = "ADMIN"
                    });

                    dbcontext.SaveChanges();

                    dbcontext.UserRoles.Add(new IdentityUserRole<string>
                    {
                        RoleId = role.Entity.Id,
                        UserId = user.Id
                    });

                    var userclaims = new List<IdentityUserClaim<string>>();

                    userclaims.Add(new IdentityUserClaim<string>
                    {
                        UserId = user.Id,
                        ClaimType = "Create Role",
                        ClaimValue = "Create Role"

                    });
                    userclaims.Add(new IdentityUserClaim<string>
                    {
                        UserId = user.Id,
                        ClaimType = "Edit Role",
                        ClaimValue = "Edit Role"

                    });
                    userclaims.Add(new IdentityUserClaim<string>
                    {
                        UserId = user.Id,
                        ClaimType = "Delete Role",
                        ClaimValue = "Delete Role"

                    });

                    userclaims.Add(new IdentityUserClaim<string>
                    {
                        UserId = user.Id,
                        ClaimType = "EditDevice",
                        ClaimValue = "EditDevice"

                    });
                    dbcontext.UserClaims.AddRange(userclaims);

                    dbcontext.SaveChanges();

                    #endregion 初始化用户
                }
                else
                {
                    System.Console.WriteLine("无需创建种子数据...");
                }

                return builder;
            }
        }
    }
}