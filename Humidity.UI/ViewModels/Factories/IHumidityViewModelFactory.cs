using Humidity.UI.State.Navigators;

namespace Humidity.UI.ViewModels.Factories
{
    public interface IHumidityViewModelFactory
    {
        BaseViewModel CreateViewModel(ViewType viewType);
    }
}
