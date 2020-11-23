using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public interface IDeviceRepository
    {
        Device GetDevice(int id);

        public IEnumerable<Device> GetAllDevices();
    }
}
