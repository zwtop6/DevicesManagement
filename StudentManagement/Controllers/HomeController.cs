using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceManagement.Common;
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
                    GUID = Guid.NewGuid().ToString(),
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
                    GUID = device.GUID,
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
                    SaveDeviceDetail(model);
                }

                Device updateDevice = _deviceRepository.Update(device);

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Delete(int id)
        {
            _deviceRepository.Delete(id);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ViewResult Analysis(int id)
        {
            Device device = _deviceRepository.GetDevice(id);

            List<DeviceDetail> deviceDetails = _deviceRepository.GetDeviceDetails(device.GUID);

            DeviceDetail deviceDetail = new DeviceDetail();

            if (deviceDetails != null && deviceDetails.Count > 0)
            {
                deviceDetail = deviceDetails.OrderBy(c => c.CheckTime).Last();
            }
            else
            {
                CreatShowData(deviceDetail);
            }


            if (device != null)
            {
                DeviceAnalysisViewModel deviceAnalysisViewModel = new DeviceAnalysisViewModel
                {
                    Id = device.Id,
                    //分析气密性
                    GasStatus = ConvertStatus(AnalysisGasStatus(deviceDetail)),
                    //分析传感器
                    SensorStatus = ConvertStatus(AnalysisSensorStatus(deviceDetail)),
                    //分析阀门
                    ValueStatus = ConvertStatus(AnalysisValveStatus(deviceDetail)),
                    //分析真空泵
                    PumpStatus = ConvertStatus(AnalysisPumpStatus(deviceDetail)),
                    //分析升降电机
                    MotorStatus = ConvertStatus(AnalysisMotorStatus(deviceDetail)),
                    //分析加热炉
                    StoveStatus = ConvertStatus(AnalysisStoveStatus(deviceDetail)),
                };
                //分析总体健康
                deviceAnalysisViewModel.DeviceStatus = ConvertStatus(AnalysisDeviceStatus(deviceAnalysisViewModel));
                //得出健康建议
                deviceAnalysisViewModel.DeviceAdvice = AnalysisDeviceAdvice(deviceDetail);

                return View(deviceAnalysisViewModel);
            }

            throw new Exception("查询不到这个设备信息");
        }

        [HttpGet]
        public ViewResult Chart(int id)
        {
            Device device = _deviceRepository.GetDevice(id);
            guid = device == null ? "" : device.GUID;

            ChartViewModel model = new ChartViewModel
            {
                StartYear = stratYear,
                EndYear = endYear,
                SecectError = selectError,
                SelectWarning = selectWarning
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Chart(ChartViewModel model)
        {
            if (ModelState.IsValid)
            {
                stratYear = model.StartYear;
                endYear = model.EndYear;
                selectError = model.SecectError;
                selectWarning = model.SelectWarning;

                return RedirectToAction();
            }

            return View();
        }

        #region Chart

        private static string guid = "";
        private static int stratYear = 2016;
        private static int endYear = 2020;
        private static bool selectWarning = true;
        private static bool selectError = true;

        [HttpPost]
        public JsonResult GetCharts()
        {
            SortedDictionary<int, int[]> dict1 = new SortedDictionary<int, int[]>();
            SortedDictionary<int, int[]> dict2 = new SortedDictionary<int, int[]>();

            List<DeviceDetail> deviceDetails = _deviceRepository.GetDeviceDetails(guid);

            if (deviceDetails != null && deviceDetails.Count > 0)
            {
                foreach (var item in deviceDetails)
                {
                    int year = item.CheckTime.Year;

                    if (!dict1.ContainsKey(year))
                    {
                        dict1.Add(year, new int[7]);
                    }
                    else
                    {
                        dict1[year][0] += item.WarningNum;
                        dict1[year][1] += item.WarningNum1;
                        dict1[year][2] += item.WarningNum2;
                        dict1[year][3] += item.WarningNum3;
                        dict1[year][4] += item.WarningNum4;
                        dict1[year][5] += item.WarningNum5;
                        dict1[year][6] += item.WarningNum6;
                    }

                    if (!dict2.ContainsKey(year))
                    {
                        dict2.Add(year, new int[7]);
                    }
                    else
                    {
                        dict2[year][0] += item.ErrorNum;
                        dict2[year][1] += item.ErrorNum1;
                        dict2[year][2] += item.ErrorNum2;
                        dict2[year][3] += item.ErrorNum3;
                        dict2[year][4] += item.ErrorNum4;
                        dict2[year][5] += item.ErrorNum5;
                        dict2[year][6] += item.ErrorNum6;
                    }
                }
            }

            List<dynamic> Labels = new List<dynamic>();
            List<dynamic> W = new List<dynamic>();
            List<dynamic> W1 = new List<dynamic>();
            List<dynamic> W2 = new List<dynamic>();
            List<dynamic> W3 = new List<dynamic>();
            List<dynamic> W4 = new List<dynamic>();
            List<dynamic> W5 = new List<dynamic>();
            List<dynamic> W6 = new List<dynamic>();
            List<dynamic> E = new List<dynamic>();
            List<dynamic> E1 = new List<dynamic>();
            List<dynamic> E2 = new List<dynamic>();
            List<dynamic> E3 = new List<dynamic>();
            List<dynamic> E4 = new List<dynamic>();
            List<dynamic> E5 = new List<dynamic>();
            List<dynamic> E6 = new List<dynamic>();

            foreach (var item in dict1)
            {
                Labels.Add(item.Key);
                var tmp = item.Value;
                W.Add(tmp[0]);
                W1.Add(tmp[1]);
                W2.Add(tmp[2]);
                W3.Add(tmp[3]);
                W4.Add(tmp[4]);
                W5.Add(tmp[5]);
                W6.Add(tmp[6]);
            }

            foreach (var item in dict2)
            {
                var tmp = item.Value;
                E.Add(tmp[0]);
                E1.Add(tmp[1]);
                E2.Add(tmp[2]);
                E3.Add(tmp[3]);
                E4.Add(tmp[4]);
                E5.Add(tmp[5]);
                E6.Add(tmp[6]);
            }

            return Json(new { labels = Labels, w = W, w1 = W1, w2 = W2, w3 = W3, w4 = W4, w5 = W5, w6 = W6, e = E, e1 = E1, e2 = E2, e3 = E3, e4 = E4, e5 = E5, e6 = E6 });
        }

        #endregion

        #region ShowData

        private void CreatShowData(DeviceDetail device)
        {
            device = new DeviceDetail
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

        #endregion

        #region Private Funtion

        private void SaveDeviceDetail(DeviceEditViewModel model)
        {
            foreach (var log in model.Logs)
            {
                Stream stream = log.OpenReadStream();
                using (StreamReader reader = new StreamReader(stream))
                {
                    string strxml = reader.ReadToEnd();

                    DeviceDetail deviceDetail = HelperXML.DESerializer<DeviceDetail>(strxml);
                    deviceDetail.DeviceGUID = model.GUID;

                    _deviceRepository.AddDetail(deviceDetail);
                }

            }
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

        private HealthStatusEnum AnalysisGasStatus(DeviceDetail model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model;

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

        private HealthStatusEnum AnalysisSensorStatus(DeviceDetail model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model;

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

        private HealthStatusEnum AnalysisValveStatus(DeviceDetail model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model;

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

        private HealthStatusEnum AnalysisPumpStatus(DeviceDetail model)
        {
            HealthStatusEnum result = HealthStatusEnum.Normal;
            DeviceDetail deviceDetail = model;

            if ((DateTime.Now - deviceDetail.LastChangeOilTime).TotalHours > deviceDetail.ChangOilTime)
            {
                result = HealthStatusEnum.Warning;

                deviceDetail.PumpAdvice = "距离上次更换泵油时间过长，建议更换泵油。";
            }

            return result;
        }

        private HealthStatusEnum AnalysisMotorStatus(DeviceDetail model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model;

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

        private HealthStatusEnum AnalysisStoveStatus(DeviceDetail model)
        {
            HealthStatusEnum result = HealthStatusEnum.Unknown;
            DeviceDetail deviceDetail = model;

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