﻿using System;
using System.Collections.Generic;
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

        public string Name { get; set; }

        public string ClassName { get; set; }

        public string City { get; set; }

    }
}