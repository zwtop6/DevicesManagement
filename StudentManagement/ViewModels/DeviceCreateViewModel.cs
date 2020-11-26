using DeviceManagement.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.ViewModels
{
    public class DeviceCreateViewModel
    {
        [Display(Name = "名称")]
        [Required(ErrorMessage = "请输入名字"), MaxLength(20, ErrorMessage = "名字长度不能超过20个字符")]
        public string Name { get; set; }

        [Display(Name = "类型")]
        [Required]
        public ClassNameEnum? ClassName { get; set; }

        [Display(Name = "城市")]
        public string City { get; set; }

        [Display(Name = "图片")]
        public List<IFormFile> Photos { get; set; }
    }
}
