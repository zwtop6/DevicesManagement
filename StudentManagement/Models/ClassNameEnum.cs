using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public enum ClassNameEnum
    {
        [Display(Name = "未知")]
        None,
        [Display(Name = "静态法")]
        PS,
        [Display(Name = "重量法")]
        PW,
        [Display(Name = "高压法")]
        PH,

    }
}
