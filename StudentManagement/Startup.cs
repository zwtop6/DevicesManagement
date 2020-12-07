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

            //�����֤����
            services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequiredLength = 8;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                });

            //�Զ���ܾ�����·��
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "/Account/Fangyu";
            });

            //�����֤
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddErrorDescriber<CustomIdentityErrorDescriber>()
                .AddEntityFrameworkStores<AppDbContext>();

            //������Ȩ
            services.AddAuthorization(options =>
            {
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role"));
                options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role"));
                //���Խ��������Ȩ
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));
                //���Խ�϶����ɫ������Ȩ
                options.AddPolicy("SuperAdminPolicy", policy => policy.RequireRole("Admin", "User"));
                
            });

            //MVC Core ֻ�����˺��ĵ�MVC����
            //MVC ������������MVC Core �Լ���صĵ��������õķ���ͷ���
            //services.AddMvcCore();
            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;

                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                options.Filters.Add(new AuthorizeFilter(policy));

            }).AddXmlSerializerFormatters(); ;

            //����ע������ע����񣬽�������
            //��������|ͬһ��Http����Χ|�������ͬHttp����
            //Scoped Service|ͬһ��ʵ��|��ʵ��
            //Transient Service|��ʵ��|��ʵ��
            //Singleton Service|ͬһ��ʵ��|ͬһ��ʵ��
            services.AddScoped<IDeviceRepository, SQLDeviceRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        //֪ʶ�㣺���м���ڹܵ�׷��
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //�����׳��쳣�м��������ʹ��DeveloperExceptionPageOptions��������
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //�����쳣
                app.UseExceptionHandler("/Error");
                //����404�Ҳ�����ҳ����Ϣ
                app.UseStatusCodePagesWithReExecute("/Error/{0}");
            }

            #region Ĭ�Ϻ;�̬�ļ��м��

            //DefaultFilesOptions defaultFilesOptions = new DefaultFilesOptions();
            //defaultFilesOptions.DefaultFileNames.Clear();
            //defaultFilesOptions.DefaultFileNames.Add("52abp.html");

            ////�����ЧҪ�ھ�̬�ļ��м��֮ǰ
            //app.UseDefaultFiles(defaultFilesOptions);

            //app.UseStaticFiles();


            #endregion

            #region ������ļ������м��

            //FileServerOptions fileServerOptions = new FileServerOptions();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Clear();
            //fileServerOptions.DefaultFilesOptions.DefaultFileNames.Add("52abp.html");

            ////����ļ������м��,ȷ������ҳ
            ////�����UseStaticFiles,UseDefaultFiles��UseDirectoryBrowser
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
                    //��ȷ��ʾ����
                    //context.Response.ContentType = "text/plain;charset=utf-8";

                    await context.Response.WriteAsync("Hello World");
                });
            });
        }
    }
}
