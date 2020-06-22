using System;
using System.Threading.Tasks;

namespace Humidity.Core.Services
{
    public class Calculator : ICalculator
    {
        /// <summary>
        /// Calculates the dewpoint
        /// </summary>
        /// <param name="temperature">The temperature in deg Celsius</param>
        /// <param name="humidity">The relative humidity in %rH</param>
        /// <returns>Dewpoint</returns>
        public async Task<double> CalculateDewpoint(double temperature, double humidity)
        {
            return await Task.Run(() =>
            {
                double dbl1, dbl2;
                dbl1 = (17.62 * temperature) / (243.12 + temperature);
                dbl2 = Math.Log(humidity / 100);
                return 243.12 * ((dbl1 + dbl2) / (17.62 - (dbl2 - dbl1)));
            });
        }

        /// <summary>
        /// Converts the given temperature from deg Celsius to Fahrenheit
        /// </summary>
        /// <param name="temperature">The temperature in deg Celsius</param>
        /// <returns>The temperature in deg Fahrenheit</returns>
        public async Task<double> ConvertFromCToF(double temperature)
        {
            return await Task.Run(() => temperature * 1.8 + 32);
        }

        /// <summary>
        /// Converts the given temperature from deg Fahrenheit to Celsius
        /// </summary>
        /// <param name="temperature">The temperature in deg Fahrenheit</param>
        /// <returns>The temperature in deg Celsius</returns>
        public async Task<double> ConvertFromFToC(double temperature)
        {
            return await Task.Run(() => (temperature - 32) / 1.8);
        }
    }
}
