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
                new Device(){Id=1,Name="PS2-0001",ClassName="静态法",City="北京"},
                new Device(){Id=2,Name="PW1-0001",ClassName="重量法",City="山西"},
                new Device(){Id=3,Name="PH2-0001",ClassName="高压法",City="天津"},
            };
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
