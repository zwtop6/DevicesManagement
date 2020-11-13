using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeviceManagement
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
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

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    //��ȷ��ʾ����
                    //context.Response.ContentType = "text/plain;charset=utf-8";

                    await context.Response.WriteAsync("Hosting Environemt: " + env.EnvironmentName);
                });
            });
        }
    }
}
