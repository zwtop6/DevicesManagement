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

        public IEnumerable<Device> GetAllDevices()
        {
            return _devicesList;
        }

        public Device GetDevice(int id)
        {
            return _devicesList.FirstOrDefault(a => a.Id == id);
        }
    }
}
