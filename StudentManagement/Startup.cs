using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManagement.Middlewares;
using DeviceManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DeviceManagement
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //test
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<AppDbContext>(
                options => options.UseSqlServer(_configuration.GetConnectionString("DeviceDBConnection"))
                );

            //身份认证配置
            services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                });

            //自定义拒绝访问路径
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/Fangyu";
            });

            //身份认证
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();

            //MVC Core 只包含了核心的MVC功能
            //MVC 包含了依赖于MVC Core 以及相关的第三方常用的服务和方法
            //services.AddMvcCore();
            services.AddMvc(Options =>
            {
                Options.EnableEndpointRouting = false;

                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                Options.Filters.Add(new AuthorizeFilter(policy));

            }).AddXmlSerializerFormatters(); ;

            //依赖注入容器注册服务，建立关联
            //服务类型|同一个Http请求范围|横跨多个不同Http请求
            //Scoped Service|同一个实例|新实例
            //Transient Service|新实例|新实例
            //Singleton Service|同一个实例|同一个实例
            services.AddScoped<IDeviceRepository, SQLDeviceRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //知识点：①中间件②管道追踪
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //可以抛出异常中间件，可以使用DeveloperExceptionPageOptions对其设置
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //拦截异常
                app.UseExceptionHandler("/Error");
                //拦截404找不到的页面信息
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            #region 默认和静态文件中间件

            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("52abp.html");

            ////这个生效要在静态文件中间件之前
            //app.UseDefaultFiles(defaultFilesOptions);

            //app.UseStaticFiles();


            #endregion

            #region 合体的文件服务中间件

            //FileServerOptions fileServerOptions = new FileServerOptions();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("52abp.html");

            ////添加文件服务中间件,确认了主页
            ////结合了UseStaticFiles,UseDefaultFiles和UseDirectoryBrowser
            //app.UseFileServer(fileServerOptions);

            #endregion

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

            //------------------------
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    //正确显示中文
                    //context.Response.ContentType = "text/plain;charset=utf-8";

                    await context.Response.WriteAsync("Hello World");
                });
            });
        }
    }
}
