using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public interface IDeviceRepository
    {
        Device GetDevice(int id);

        List<DeviceDetail> GetDeviceDetails(string guid);

        public IEnumerable<Device> GetAllDevices();

        public Device Add(Device device);

        public Device Update(Device newdevice);

        public Device Delete(int id);

        public DeviceDetail AddDetail(DeviceDetail deviceDetail);

        public DeviceDetail UpdateDetail(DeviceDetail deviceDetail);

        public DeviceDetail DeleteDetail(int id);
    }
}
