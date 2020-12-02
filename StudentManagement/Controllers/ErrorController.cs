﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Controllers
{
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        private readonly ILogger<ErrorController> logger;

        /// <summary>
        /// 通过ASP.Net Core 依赖注入服务注入ILogger服务
        /// 将指定类型的控制器作为泛型参数
        /// </summary>
        /// <param name="logger"></param>
        public ErrorController(ILogger<ErrorController> logger)
        {
            this.logger = logger;
        }


        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            var stautsCodeResult = HttpContext.Features.Get<IStatusCodeReExecuteFeature>();

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "抱歉，您访问的页面不存在";

                    logger.LogWarning($"发生了一个404错误。路径={stautsCodeResult.OriginalPath}以及查询字符串={stautsCodeResult.OriginalQueryString}");

                    //ViewBag.Path = stautsCodeResult.OriginalPath;
                    //ViewBag.QueryStr = stautsCodeResult.OriginalQueryString;
                    //ViewBag.BasePath = stautsCodeResult.OriginalPathBase;
                    break;
                default:
                    break;
            }

            return View("NotFound");
        }

        [Route("Error")]
        public IActionResult Error()
        {
            //获取异常详情信息
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            //LogError()方法将异常记录作为日志中的错误类别记录
            logger.LogError($"路径{exceptionHandlerPathFeature.Path},产生了一个错误{exceptionHandlerPathFeature.Error}");

            //ViewBag.ExceptionPath = exceptionHandlerPathFeature.Path;
            //ViewBag.ExceptionMessage = exceptionHandlerPathFeature.Error.Message;
            //ViewBag.StackTrace = exceptionHandlerPathFeature.Error.StackTrace;

            return View("Error");
        }
    }
}
