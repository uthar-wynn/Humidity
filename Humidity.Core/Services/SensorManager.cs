using Humidity.Core.Sensor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Humidity.Core.Services
{
    public class SensorManager : ISensorManager
    {
        private Eight _buffer;

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

        public int GetCountSensors()
        {

            return Sensor.Sensor.IowKitGetNumDevs();
        }

        public Task<Eight> GetValueSensor(byte trigger)
        {
            throw new NotImplementedException();
        }

        public void ReadSensors(ref List<int> devices)
        {
            for (int i = 0; i <= Sensor.Sensor.IowKitGetNumDevs(); i++)
            {
                devices.Add(Sensor.Sensor.IowKitGetDeviceHandle(i));
            }
        }

        public void ResetSensor(ref List<int> devices)
        {
            _buffer.byte0 = 2;
            _buffer.byte1 = 194;
            _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
            _buffer.byte3 = (byte)SensorStatus.soft_reset;

            Sensor.Sensor.IowKitWrite(devices[1], 1, ref _buffer, 8);
            Sensor.Sensor.IowKitRead(devices[1], 1, ref _buffer, 8);
        }

        public Task SetValueSensor(byte trigger, byte status)
        {
            throw new NotImplementedException();
        }
    }
}
