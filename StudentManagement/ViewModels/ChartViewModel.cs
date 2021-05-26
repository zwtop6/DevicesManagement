using DeviceManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.ViewModels
{
    public class ChartViewModel
    {
        public int StartYear { get; set; }

        public int EndYear { get; set; }

        public bool SelectWarning { get; set; }

        public bool SecectError { get; set; }
    }
}
