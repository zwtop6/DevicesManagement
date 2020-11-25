using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Device>().HasData(

                new Device
                {
                    Id = 1,
                    Name = "PS1-0001",
                    ClassName = ClassNameEnum.PS,
                    City = "北京",

                },

                new Device
                {
                    Id = 2,
                    Name = "PH2-0001",
                    ClassName = ClassNameEnum.PH,
                    City = "陕西",

                }

                );
        }

    }
}
