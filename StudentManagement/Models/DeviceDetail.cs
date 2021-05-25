using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DeviceManagement.Models
{
    /// <summary>
    /// 设备细节
    /// </summary>
    public class DeviceDetail
    {
        public int Id { get; set; }

        public string DeviceGUID { get; set; }

        public DateTime CheckTime { get; set; }

        #region 气密性(低压气密性、高压气密性）

        public double LowPressStartP { get; set; }

        public double LowPressEndP { get; set; }

        public double LowPressDuring { get; set; }

        public double HighPressStartP { get; set; }

        public double HighPressEndP { get; set; }

        public double HighPressDuring { get; set; }

        public string GasAdvice { get; set; }


        #endregion

        #region 传感器(真空准确度、常压准确度、稳定度）

        public double VacuumPress { get; set; }

        public double AirPress { get; set; }

        public double StandardAirPress { get; set; }

        public double AirPressMax { get; set; }

        public double AirPressMin { get; set; }

        public string SenorAdvice { get; set; }

        #endregion

        #region 阀门(打开、关闭、定时关）

        public bool OpenValve { get; set; }

        public bool CloseValve { get; set; }

        public bool AutoValve { get; set; }

        public string ValveAdvice { get; set; }

        #endregion

        #region 真空泵(使用时长、是否需要换油）

        public double UseDuring { get; set; }

        public DateTime LastChangeOilTime { get; set; }

        public double ChangOilTime { get; set; }

        public string PumpAdvice { get; set; }

        #endregion

        #region 升降电机(控制是否正常，上升下降时间）

        public bool UpMotor { get; set; }

        public bool DownMotor { get; set; }

        public bool StopMotor { get; set; }

        public double UpDuring { get; set; }

        public double DownDuring { get; set; }

        public string MotorAdvice { get; set; }

        #endregion

        #region 加热炉(控制是否正常，稳定性，准确性）

        public bool StartStove { get; set; }

        public bool StopStove { get; set; }

        public bool HoldStove { get; set; }

        public double StoveTempMax { get; set; }

        public double StoveTempMin { get; set; }

        public double StoveAirTemp { get; set; }

        public double StandardAirTemp { get; set; }

        public string StoveAdvice { get; set; }

        #endregion
    }
}
