using DeviceManagement.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.ViewModels
{
    public class DeviceAnalysisViewModel
    {
        public int Id { get; set; }

        [Display(Name = "总体健康状态")]
        public String DeviceStatus { get; set; }

        [Display(Name = "气密性")]
        public String GasStatus { get; set; }

        [Display(Name = "传感器")]
        public String SensorStatus { get; set; }

        [Display(Name = "阀门")]
        public String ValueStatus { get; set; }

        [Display(Name = "真空泵")]
        public String PumpStatus { get; set; }

        [Display(Name = "升降电机")]
        public String MotorStatus { get; set; }

        [Display(Name = "加热炉")]
        public String StoveStatus { get; set; }

        [Display(Name = "建议")]
        public String DeviceAdvice { get; set; }
    }
}
