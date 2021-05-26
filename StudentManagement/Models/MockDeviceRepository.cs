using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public class MockDeviceRepository : IDeviceRepository
    {

        private readonly List<Device> _devicesList;

        public MockDeviceRepository()
        {
            _devicesList = new List<Device>()
            {
                new Device(){Id=1,Name="PS2-0001",ClassName=ClassNameEnum.PS,City="北京"},
                new Device(){Id=2,Name="PW1-0001",ClassName=ClassNameEnum.PW,City="山西"},
                new Device(){Id=3,Name="PH2-0001",ClassName=ClassNameEnum.PH,City="天津"},
            };
        }

        public Device Add(Device device)
        {
            device.Id = _devicesList.Max(s => s.Id) + 1;
            _devicesList.Add(device);
            return device;
        }

        public DeviceDetail AddDetail(DeviceDetail deviceDetail)
        {
            throw new NotImplementedException();
        }

        public Device Delete(int id)
        {
            Device device = _devicesList.FirstOrDefault(s => s.Id == id);

            if (device != null)
            {
                _devicesList.Remove(device);
            }

            return device;
        }

        public DeviceDetail DeleteDetail(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Device> GetAllDevices()
        {
            return _devicesList;
        }

        public Device GetDevice(int id)
        {
            return _devicesList.FirstOrDefault(a => a.Id == id);
        }

        public List<DeviceDetail> GetDeviceDetails(string guid)
        {
            throw new NotImplementedException();
        }

        public Device Update(Device newdevice)
        {
            Device device = _devicesList.FirstOrDefault(s => s.Id == newdevice.Id);

            if (device != null)
            {
                device.Name = newdevice.Name;
                device.ClassName = newdevice.ClassName;
                device.City = newdevice.City;
            }

            return device;
        }

        public DeviceDetail UpdateDetail(DeviceDetail deviceDetail)
        {
            throw new NotImplementedException();
        }
    }
}
