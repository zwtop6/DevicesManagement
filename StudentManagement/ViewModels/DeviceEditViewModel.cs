using DeviceManagement.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.ViewModels
{
    public class DeviceEditViewModel : DeviceCreateViewModel
    {

        public int Id { get; set; }

        public string GUID { get; set; }

        public string ExistingPhotoPath { get; set; }

        public string ExistingLogsPath { get; set; }

        [Display(Name = "日志")]
        public List<IFormFile> Logs { get; set; }

        [Display(Name = "健康状态")]
        public HealthStatusEnum HealthStatus { get; set; }


    }
}
