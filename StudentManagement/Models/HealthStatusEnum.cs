using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public enum HealthStatusEnum
    {
        [Display(Name = "未知")]
        Unknown,
        [Display(Name = "正常")]
        Normal,
        [Display(Name = "警告")]
        Warning,
        [Display(Name = "故障")]
        Error,
    }
}
