using System.Threading.Tasks;

namespace Humidity.Core.Services
{
    public interface ICalculator
    {
        Task<double> CalculateDewpoint(double temperature, double humidity);

        Task<double> ConvertFromCToF(double temperature);

        Task<double> ConvertFromFToC(double temperature);
    }
}
