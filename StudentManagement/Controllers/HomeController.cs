using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
        const string NORMAL = "正常";
        const string ERROR = "错误";
        const string WARING = "警告";
        const string UNKONW = "未知";

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
            //检查提供的数据是否有效，如果没有通过验证，需要重新编辑设备信息
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

        [HttpGet]
        public ViewResult Analysis(int id)
        {
            Device device = _deviceRepository.GetDevice(id);

            if (device != null)
            {
                CreatShowData(device);

                DeviceAnalysisViewModel deviceAnalysisViewModel = new DeviceAnalysisViewModel
                {
                    Id = device.Id,
                    //分析气密性
                    GasStatus = ConvertStatus(AnalysisGasStatus(device)),
                    //分析传感器
                    SensorStatus = ConvertStatus(AnalysisSensorStatus(device)),
                    //分析阀门
                    ValueStatus = ConvertStatus(AnalysisValveStatus(device)),
                    //分析真空泵
                    PumpStatus = ConvertStatus(AnalysisPumpStatus(device)),
                    //分析升降电机
                    MotorStatus = ConvertStatus(AnalysisMotorStatus(device)),
                    //分析加热炉
                    StoveStatus = ConvertStatus(AnalysisStoveStatus(device)),
                };
                //分析总体健康
                deviceAnalysisViewModel.DeviceStatus = ConvertStatus(AnalysisDeviceStatus(deviceAnalysisViewModel));
                //得出健康建议
                deviceAnalysisViewModel.DeviceAdvice = AnalysisDeviceAdvice(device.DeviceDetail);

                return View(deviceAnalysisViewModel);
            }

            throw new Exception("查询不到这个设备信息");
        }

        [HttpGet]
        public IActionResult Chart()
        {
            return View();
        }

        #region ShowData

        private void CreatShowData(Device device)
        {
            if (device.DeviceDetail == null)
            {
                device.DeviceDetail = new DeviceDetail
                {
                    //气密性
                    LowPressStartP = 10,
                    LowPressEndP = 20,
                    LowPressDuring = 2,
                    HighPressStartP = 20000,
                    HighPressDuring = 10,
                    HighPressEndP = 21000,

                    //传感器
                    VacuumPress = 2,
                    AirPress = 10001,
                    StandardAirPress = 10000,
                    AirPressMax = 10010,
                    AirPressMin = 9960,

                    //阀门
                    OpenValve = true,
                    CloseValve = true,
                    AutoValve = true,

                    //真空泵
                    UseDuring = 3100,
                    LastChangeOilTime = DateTime.Now,
                    ChangOilTime = 3000,

                    //电机
                    UpMotor = true,
                    DownMotor = true,
                    StopMotor = true,
                    UpDuring = 30,
                    DownDuring = 35,

                    //加热炉
                    StartStove = true,
                    StopStove = true,
                    HoldStove = true,
                    StoveTempMax = 310,
                    StoveTempMin = 290,
                    StandardAirTemp = 30,
                    StoveAirTemp = 31,
                };
            }
        }

        #endregion

        #region Private Funtion



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

        private HealthStatusEnum AnalysisDeviceStatus(DeviceAnalysisViewModel model)
        {
            HealthStatusEnum result = HealthStatusEnum.Normal;

            List<string> list = new List<string>();

            list.Add(model.GasStatus);
            list.Add(model.SensorStatus);
            list.Add(model.PumpStatus);
            list.Add(model.MotorStatus);
            list.Add(model.ValueStatus);
            list.Add(model.StoveStatus);

            if (list.Contains(ERROR))
            {
                result = HealthStatusEnum.Error;
            }
            else if (list.Contains(WARING))
            {
                result = HealthStatusEnum.Warning;
            }

            return result;
        }

        const double LOWGASNORMAL = 100;
        const double LOWGASWARING = 200;
        const double HIGHGASNORMAL = 1000;
        const double HIGHGASWARINGL = 2000;

        private HealthStatusEnum AnalysisGasStatus(Device model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model.DeviceDetail;

            //低压气密性
            double lowGas = (deviceDetail.LowPressEndP - deviceDetail.LowPressStartP) / deviceDetail.LowPressDuring;

            //高压气密性
            double highGas = (deviceDetail.HighPressEndP - deviceDetail.HighPressStartP) / deviceDetail.HighPressDuring;

            if (lowGas <= LOWGASNORMAL && highGas <= HIGHGASNORMAL)
            {
                result = HealthStatusEnum.Normal;
            }
            else if (lowGas > LOWGASWARING || highGas > HIGHGASWARINGL)
            {
                result = HealthStatusEnum.Error;
                deviceDetail.GasAdvice = "气密性异常，会造成测试不准确。";
            }
            else
            {
                result = HealthStatusEnum.Warning;

                if (lowGas <= LOWGASNORMAL)
                {
                    deviceDetail.GasAdvice = "低压气密性较差。";
                }

                if (highGas <= HIGHGASWARINGL)
                {
                    deviceDetail.GasAdvice += "高压气密性较差。";
                }
            }

            return result;
        }

        const double DIFFERENTPRESS = 30;
        const double STABLEPRESS = 20;

        private HealthStatusEnum AnalysisSensorStatus(Device model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model.DeviceDetail;

            //真空压力差值
            double vacuumDiff = Math.Abs(deviceDetail.VacuumPress);
            //常压压力差值
            double airDiff = Math.Abs(deviceDetail.AirPress - deviceDetail.StandardAirPress);
            //稳定度
            double stableDiff = deviceDetail.AirPressMax - deviceDetail.AirPressMin;

            if (vacuumDiff <= DIFFERENTPRESS && airDiff <= DIFFERENTPRESS && stableDiff <= STABLEPRESS)
            {
                result = HealthStatusEnum.Normal;
            }
            else
            {
                result = HealthStatusEnum.Warning;

                if (vacuumDiff > LOWGASNORMAL)
                {
                    deviceDetail.SenorAdvice = "真空条件下读数不准确。";
                }

                if (airDiff > HIGHGASWARINGL)
                {
                    deviceDetail.SenorAdvice += "常压条件下读数不准确。";
                }

                if (stableDiff > STABLEPRESS)
                {
                    deviceDetail.SenorAdvice += "读数稳定性较差。";
                }
            }

            return result;
        }

        private HealthStatusEnum AnalysisValveStatus(Device model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model.DeviceDetail;

            if (deviceDetail.OpenValve && deviceDetail.CloseValve && deviceDetail.AutoValve)
            {
                result = HealthStatusEnum.Normal;
            }
            else
            {
                result = HealthStatusEnum.Error;

                deviceDetail.ValveAdvice = "阀门操作异常，无法正常运行。";

                if (!deviceDetail.OpenValve)
                {
                    deviceDetail.ValveAdvice += "打开阀门操作失败。";
                }

                if (!deviceDetail.CloseValve)
                {
                    deviceDetail.ValveAdvice += "关闭阀门操作失败。";
                }

                if (!deviceDetail.AutoValve)
                {
                    deviceDetail.ValveAdvice += "自动控制阀门操作失败。";
                }
            }

            return result;
        }

        private HealthStatusEnum AnalysisPumpStatus(Device model)
        {
            HealthStatusEnum result = HealthStatusEnum.Normal;
            DeviceDetail deviceDetail = model.DeviceDetail;

            if ((DateTime.Now - deviceDetail.LastChangeOilTime).TotalHours > deviceDetail.ChangOilTime)
            {
                result = HealthStatusEnum.Warning;

                deviceDetail.PumpAdvice = "距离上次更换泵油时间过长，建议更换泵油。";
            }

            return result;
        }

        private HealthStatusEnum AnalysisMotorStatus(Device model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model.DeviceDetail;

            if (deviceDetail.UpMotor && deviceDetail.DownMotor && deviceDetail.StopMotor)
            {
                result = HealthStatusEnum.Normal;
            }
            else
            {
                result = HealthStatusEnum.Error;

                deviceDetail.MotorAdvice = "电机操作异常，无法正常运行。";

                if (!deviceDetail.UpMotor)
                {
                    deviceDetail.MotorAdvice += "上升电机操作失败。";
                }

                if (!deviceDetail.DownMotor)
                {
                    deviceDetail.MotorAdvice += "下降电机操作失败。";
                }

                if (!deviceDetail.StopMotor)
                {
                    deviceDetail.MotorAdvice += "停止电机操作失败。";
                }
            }

            return result;
        }

        const double DIFFERENTTEMP = 5;

        private HealthStatusEnum AnalysisStoveStatus(Device model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model.DeviceDetail;

            double tempDiff = deviceDetail.StoveTempMax - deviceDetail.StoveTempMin;

            if (deviceDetail.StartStove && deviceDetail.StopStove && deviceDetail.HoldStove && tempDiff <= DIFFERENTTEMP)
            {
                result = HealthStatusEnum.Normal;
            }
            else if (!deviceDetail.StartStove || !deviceDetail.StopStove || !deviceDetail.HoldStove)
            {
                result = HealthStatusEnum.Error;
                deviceDetail.StoveAdvice = "加热炉操作异常，无法正常运行。";

                if (!deviceDetail.StartStove)
                {
                    deviceDetail.StoveAdvice += "开始加热操作失败。";
                }

                if (!deviceDetail.StopStove)
                {
                    deviceDetail.StoveAdvice += "停止加热操作失败。";
                }

                if (!deviceDetail.HoldStove)
                {
                    deviceDetail.StoveAdvice += "保持温度操作失败。";
                }
            }
            else
            {
                result = HealthStatusEnum.Warning;
                deviceDetail.StoveAdvice = "加热炉保持温度稳定性较差，会影响测试结果。";
            }

            return result;
        }

        private string AnalysisDeviceAdvice(DeviceDetail model)
        {
            StringBuilder result = new StringBuilder();

            result.Append(model.GasAdvice);
            result.Append(model.SenorAdvice);
            result.Append(model.ValveAdvice);
            result.Append(model.PumpAdvice);
            result.Append(model.MotorAdvice);
            result.Append(model.StoveAdvice);

            if (result.Length > 0)
            {
                result.Append("建议尽早联系售后人员进行检查");
            }

            //result.Append("     该设备整体健康状态良好,可以正常工作。但是气密性相较之前变差，处于不影响测试的边缘，建议尽早联系售后人员进行检查；而真空泵也长时间未更换泵油，建议更换泵油。");

            return result.ToString();
        }

        private string ConvertStatus(HealthStatusEnum status)
        {
            string result = "";

            switch (status)
            {
                case HealthStatusEnum.Unknown:
                    result = UNKONW;
                    break;
                case HealthStatusEnum.Normal:
                    result = NORMAL;
                    break;
                case HealthStatusEnum.Warning:
                    result = WARING;
                    break;
                case HealthStatusEnum.Error:
                    result = ERROR;
                    break;
                default:
                    break;
            }

            return result;
        }


        #endregion


    }
}