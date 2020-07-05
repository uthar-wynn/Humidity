using Humidity.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Humidity.Core.Services
{
    /// <summary>
    /// Implementation for the <see cref="ISensorManager"/>
    /// </summary>
    public class SensorManager : ISensorManager
    {
        private Eight _buffer;

        /// <summary>
        /// Enable the IO Warrior kit before enablic IIC and sensibus
        /// </summary>
        public int EnableSensor(List<int> device)
        {
            _buffer.byte0 = 1;
            _buffer.byte1 = 1;
            _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
            _buffer.byte3 = 0;
            _buffer.byte4 = 0;
            _buffer.byte5 = 0;
            _buffer.byte6 = 0;
            _buffer.byte7 = 0;

            return Sensor.Sensor.IowKitWrite(device[1], 1, ref _buffer, 8);
        }

        /// <summary>
        /// Counts the connected devices
        /// </summary>
        /// <returns>The number of connected devices</returns>
        public Task<int> GetCountSensors()
        {

            return Task.Run(() => Sensor.Sensor.IowKitGetNumDevs());
        }

        /// <summary>
        /// Filling the buffer with the values comming from the sensor with the given trigger
        /// </summary>
        /// <param name="trigger">byte for the kind of parameter required</param>
        public async Task<Eight> GetValueSensor(int ioHandle, byte trigger)
        {
            try
            {
                _buffer.byte0 = 2;
                _buffer.byte1 = 194;
                _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
                _buffer.byte3 = trigger;

                Task.Run(() => Sensor.Sensor.IowKitWrite(ioHandle, 1, ref _buffer, 8)).Wait();
                Task.Run(() => Sensor.Sensor.IowKitRead(ioHandle, 1, ref _buffer, 8)).Wait();

                if (_buffer.byte1 == 128)
                    throw new ArgumentException("IIC Special mode function write failed");

                _buffer.byte0 = 3;
                _buffer.byte1 = 3;
                _buffer.byte2 = (byte)SensorStatus.sh21adr_read;

                Task.Run(() => Sensor.Sensor.IowKitWrite(ioHandle, 1, ref _buffer, 8)).Wait();
                Task.Run(() => Sensor.Sensor.IowKitRead(ioHandle, 1, ref _buffer, 8)).Wait();

                if (_buffer.byte1 == 128)
                    throw new ArgumentException("IIC Special mode function write failed");

                await Task.Delay(0);

                return _buffer;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Get the sensors and put them into a list
        /// </summary>
        public void ReadSensors(ref List<int> devices)
        {
            for (int i = 0; i <= Sensor.Sensor.IowKitGetNumDevs(); i++)
            {
                devices.Add(Sensor.Sensor.IowKitGetDeviceHandle(i));
            }
        }

        /// <summary>
        /// Gives the sensor a soft reset
        /// </summary>
        public void ResetSensor(ref List<int> devices)
        {
            _buffer.byte0 = 2;
            _buffer.byte1 = 194;
            _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
            _buffer.byte3 = (byte)SensorStatus.soft_reset;

            Sensor.Sensor.IowKitWrite(devices[1], 1, ref _buffer, 8);
            Sensor.Sensor.IowKitRead(devices[1], 1, ref _buffer, 8);
        }

        /// <summary>
        /// Writing values to the sensor, eg: heating etc...
        /// </summary>
        /// <param name="trigger">byte for the kind of parameter required</param>
        /// <param name="status">status for the sensor</param>  
        public void SetValueSensor(int ioHandle, byte trigger, byte status)
        {
            try
            {
                _buffer.byte0 = 2;
                _buffer.byte1 = 195;
                _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
                _buffer.byte3 = trigger;
                _buffer.byte4 = status;

                Task.Run(() => Sensor.Sensor.IowKitWrite(ioHandle, 1, ref _buffer, 8)).Wait();
                Task.Run(() => Sensor.Sensor.IowKitRead(ioHandle, 1, ref _buffer, 8)).Wait();

                if (_buffer.byte1 == 128)
                    throw new ArgumentException("IIC no ack from sensor during read");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
