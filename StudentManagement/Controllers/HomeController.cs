using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeviceManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace DeviceManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDeviceRepository _deviceRepository;

        //使用构造函数注入的方式注入IDeviceRepository
        public HomeController(IDeviceRepository deviceRepository)
        {
            _deviceRepository = deviceRepository;
        }

        public String Index()
        {
            return _deviceRepository.GetDevice(1).Name;
        }

        public IActionResult Details()
        {
            Device device = _deviceRepository.GetDevice(1);


            #region ViewData

            //弱类型的字典对象
            //使用string类型的键值，存储和查询ViewData字典中的数据
            //运行时动态解析
            //没有智能感知，编译时也没有类型检查

            ViewData["PageTitle"] = "设备信息";
            ViewData["Device"] = device;

            #endregion

            return View();
        }
    }
}