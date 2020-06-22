using System;
using System.Runtime.InteropServices;

namespace Humidity.Core.Sensor
{
    /// <summary>
    /// Bytes used for interaction with a sensirion adapter
    /// </summary>
    public enum SensorStatus
    {
        sh21adr_read = 129,
        sh21adr_write = 128,
        Trigger_T_measurement_hold = 227,
        Trigger_RH_measurement_hold = 229,
        Trigger_T_measurement_nohold = 243,
        Trigger_RH_measurement_nohold = 245,
        write_user_register = 230,
        read_user_register = 231,
        soft_reset = 254
    }

    /// <summary>
    /// Structure for storing the bytes used for retrieving/sending data from/to sensor
    /// </summary>
    public struct Eight
    {
        public Byte byte0;
        public Byte byte1;
        public Byte byte2;
        public Byte byte3;
        public Byte byte4;
        public Byte byte5;
        public Byte byte6;
        public Byte byte7;
    }

    /// <summary>
    /// Used for calculating the stability
    /// </summary>
    public struct Stability
    {
        public double Humidity;
        public double Temperature;
        public double Dewpoint;
    }

    public class Sensor
    {
        #region DLL imports

        [DllImport("iowkit")]
        public static extern int IowKitOpenDevice();
        [DllImport("iowkit")]
        public static extern int IowKitCloseDevice(int iowHandle);
        [DllImport("iowkit")]
        public static extern int IowKitWrite(int iowHandle, int numPipe, ref Eight m_buffer, int length);
        [DllImport("iowkit")]
        public static extern int IowKitRead(int iowHandle, int numPipe, ref Eight m_buffer, int length);
        [DllImport("iowkit")]
        public static extern int IowKitGetNumDevs();
        [DllImport("iowkit")]
        public static extern string IowKitVersion();
        [DllImport("iowkit")]
        public static extern int IowKitGetDeviceHandle(int numDevice);
        [DllImport("iowkit")]
        public static extern int IowKitGetProductId(int iowHandle);

        #endregion
    }
}
