using Humidity.UI.State.Navigators;
using System;

namespace Humidity.UI.ViewModels.Factories
{
    public class HumidityViewModelFactory : IHumidityViewModelFactory
    {
        private readonly CreateViewModel<HomeViewModel> _createHomeViewModel;
        private readonly CreateViewModel<LogViewModel> _createLogViewModel;

        public HumidityViewModelFactory(CreateViewModel<HomeViewModel> createHomeViewModel, CreateViewModel<LogViewModel> createLogViewModel)
        {
            _createHomeViewModel = createHomeViewModel;
            _createLogViewModel = createLogViewModel;
        }

        public BaseViewModel CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Home:
                    return _createHomeViewModel();
                case ViewType.Log:
                    return _createLogViewModel();
                default:
                    throw new ArgumentException("The ViewType does nog have a ViewModel.", "viewType");
            }
        }
    }
}
