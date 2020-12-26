using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    /// <summary>
    /// 设备模型
    /// </summary>
    public class Device
    {
        public int Id { get; set; }

        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名字"), MaxLength(20, ErrorMessage = "名字长度不能超过20个字符")]
        public string Name { get; set; }

        [Display(Name = "类型")]
        [Required]
        public ClassNameEnum? ClassName { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }

        public string PhotoPath { get; set; }

        public string LogPath { get; set; }

    }
}
