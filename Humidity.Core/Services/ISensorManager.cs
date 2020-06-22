using Humidity.Core.Sensor;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Humidity.Core.Services
{
    public interface ISensorManager
    {
        /// <summary>
        /// Counts the connected devices
        /// </summary>
        /// <returns>The number of connected devices</returns>
        int GetCountSensors();

        /// <summary>
        /// Get the sensors and put them into a list
        /// </summary>
        void ReadSensors(ref List<int> devices);

        /// <summary>
        /// Enable the IO Warrior kit before enablic IIC and sensibus
        /// </summary>
        int EnableSensor(List<int> device);

        /// <summary>
        /// Gives the sensor a soft reset
        /// </summary>
        void ResetSensor(ref List<int> devices);

        /// <summary>
        /// Filling the buffer with the values comming from the sensor with the given trigger
        /// </summary>
        /// <param name="trigger">byte for the kind of parameter required</param>
        Task<Eight> GetValueSensor(byte trigger);

        /// <summary>
        /// Writing values to the sensor, eg: heating etc...
        /// </summary>
        /// <param name="trigger">byte for the kind of parameter required</param>
        /// <param name="status">status for the sensor</param>
        Task SetValueSensor(byte trigger, byte status);
    }
}
