using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public class SQLDeviceRepository : IDeviceRepository
    {
        private readonly AppDbContext _context;
        public SQLDeviceRepository(AppDbContext context)
        {
            this._context = context;
        }

        public Device Add(Device device)
        {
            _context.Devices.Add(device);
            _context.SaveChanges();

            return device;
        }



        public Device Delete(int id)
        {
            Device device = _context.Devices.Find(id);

            if (device != null)
            {
                _context.Devices.Remove(device);
                _context.SaveChanges();
            }

            return device;
        }



        public IEnumerable<Device> GetAllDevices()
        {
            return _context.Devices;
        }

        public Device GetDevice(int id)
        {
            return _context.Devices.Find(id);
        }

        public Device Update(Device newdevice)
        {
            var device = _context.Devices.Attach(newdevice);

            device.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _context.SaveChanges();

            return newdevice;
        }

        public DeviceDetail AddDetail(DeviceDetail deviceDetail)
        {
            _context.DeviceDetails.Add(deviceDetail);
            _context.SaveChanges();

            return deviceDetail;
        }

        public DeviceDetail UpdateDetail(DeviceDetail deviceDetail)
        {
            var device = _context.DeviceDetails.Attach(deviceDetail);

            device.State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _context.SaveChanges();

            return deviceDetail;
        }

        public DeviceDetail DeleteDetail(int id)
        {
            DeviceDetail deviceDetail = _context.DeviceDetails.Find(id);

            if (deviceDetail != null)
            {
                _context.DeviceDetails.Remove(deviceDetail);
                _context.SaveChanges();
            }

            return deviceDetail;
        }

        public List<DeviceDetail> GetDeviceDetails(int deviceID)
        {
            throw new NotImplementedException();
        }
    }
}
