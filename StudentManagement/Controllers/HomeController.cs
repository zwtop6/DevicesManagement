using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DeviceManagement.Models;
using DeviceManagement.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DeviceManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger logger;

        //使用构造函数注入的方式注入IDeviceRepository
        public HomeController(IDeviceRepository deviceRepository, IWebHostEnvironment webHostEnvironment, ILogger<HomeController> logger)
        {
            _deviceRepository = deviceRepository;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;

        }

        public IActionResult Index()
        {
            var model = _deviceRepository.GetAllDevices();

            return View(model);
        }

        public IActionResult Details(int id)
        {
            Device model = _deviceRepository.GetDevice(id);

            if (model == null)
            {
                Response.StatusCode = 404;

                return View("DeviceNotFound", id);
            }

            #region 弱类型

            #region ViewData

            ////弱类型的字典对象
            ////使用string类型的键值，存储和查询ViewData字典中的数据
            ////运行时动态解析
            ////没有智能感知，编译时也没有类型检查

            //ViewData["PageTitle"] = "设备信息";
            //ViewData["Device"] = device;

            #endregion

            #region ViewBag

            //ViewBag.PageTitle = "设备信息";
            //ViewBag.Device = device;

            #endregion

            #endregion

            HomeDetailsViewModels homeDetailsViewModels = new HomeDetailsViewModels
            {
                Device = model,
                PageTitle = "设备信息",
            };

            return View(homeDetailsViewModels);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(DeviceCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Device newDevice = new Device
                {
                    Name = model.Name,
                    ClassName = model.ClassName,
                    City = model.City,
                    PhotoPath = ProcessUploadedFile(model),
                };

                _deviceRepository.Add(newDevice);
                return RedirectToAction("Details", new { id = newDevice.Id });

            }

            return View();
        }

        [HttpGet]
        public ViewResult Edit(int id)
        {
            Device device = _deviceRepository.GetDevice(id);

            if (device != null)
            {
                DeviceEditViewModel deviceEditViewModel = new DeviceEditViewModel
                {
                    Id = device.Id,
                    Name = device.Name,
                    ClassName = device.ClassName,
                    City = device.City,
                    ExistingPhotoPath = device.PhotoPath,
                    ExistingLogsPath = device.LogPath,
                    HealthStatus = device.HealthStatus,
                };

                return View(deviceEditViewModel);
            }

            throw new Exception("查询不到这个设备信息");
        }

        [HttpPost]
        public IActionResult Edit(DeviceEditViewModel model)
        {
            //检查提供的数据是否有效，如果没有通过验证，需要重新编辑学生信息
            //这样用户就可以更正并重新提交编辑表单
            if (ModelState.IsValid)
            {
                Device device = _deviceRepository.GetDevice(model.Id);

                device.Name = model.Name;
                device.ClassName = model.ClassName;
                device.City = model.City;
                device.HealthStatus = model.HealthStatus;

                if (model.Photos?.Count > 0)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath, "images", model.ExistingPhotoPath);

                        System.IO.File.Delete(filePath);

                    }

                    device.PhotoPath = ProcessUploadedFile(model);
                }

                if (model.Logs?.Count > 0)
                {
                    device.LogPath = ProcessUploadedFileLogs(model);
                }

                Device updateDevice = _deviceRepository.Update(device);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        /// <summary>
        /// 将日志保存到指定的路径中，并返回唯一的路径名
        /// </summary>
        /// <returns></returns>
        private string ProcessUploadedFileLogs(DeviceEditViewModel model)
        {
            string filePath = null;

            if (model.Logs.Count > 0)
            {
                //确定保存路径
                if (model.ExistingLogsPath != null)
                {
                    filePath = model.ExistingLogsPath;
                }
                else
                {
                    string str1 = Path.Combine(webHostEnvironment.WebRootPath, "logs");
                    string str2 = model.Name + "_" + Guid.NewGuid().ToString();

                    filePath = Path.Combine(str1, str2);

                    System.IO.Directory.CreateDirectory(filePath);
                }

                foreach (var log in model.Logs)
                {
                    string path = Path.Combine(filePath, Guid.NewGuid().ToString() + "_" + log.FileName);

                    //因为使用了非托管资源，所以需要手动进行释放
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        //使用IFormFile接口提供的CopyTo()方法见文件赋值到wwwroot/images文件夹
                        log.CopyTo(fileStream);
                    }
                }
            }

            return filePath;
        }

        /// <summary>
        /// 将照片保存到指定的路径中，并返回唯一的路径名
        /// </summary>
        /// <returns></returns>
        private string ProcessUploadedFile(DeviceCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photos.Count > 0)
            {
                foreach (var photo in model.Photos)
                {
                    //必须将图像上传到wwwroot中的images文件夹
                    //而要获取wwwroot文件夹的路径，我们需要注入ASP.Net Core提供的webHostEnvironment.WebRootPath
                    string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");

                    //为了确保文件名是唯一的，我们在文件名后附加一个新的GUID值和一个下划线
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    //因为使用了非托管资源，所以需要手动进行释放
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        //使用IFormFile接口提供的CopyTo()方法见文件赋值到wwwroot/images文件夹
                        photo.CopyTo(fileStream);
                    }
                }
            }

            return uniqueFileName;
        }

    }
}