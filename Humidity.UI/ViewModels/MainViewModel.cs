using Humidity.UI.Commands;
using Humidity.UI.State.Navigators;
using Humidity.UI.ViewModels.Factories;
using System.Windows.Input;

namespace Humidity.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public INavigator Navigator { get; set; }
        public ICommand UpdateCurrentViewModelCommand { get; }

        public MainViewModel(INavigator navigator, IHumidityViewModelFactory viewModelFactory)
        {
            Navigator = navigator;

            UpdateCurrentViewModelCommand = new UpdateCurrentViewModelCommand(navigator, viewModelFactory);

            UpdateCurrentViewModelCommand.Execute(ViewType.Home);
        }
    }
}
