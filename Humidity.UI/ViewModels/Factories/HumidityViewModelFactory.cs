using Humidity.UI.State.Navigators;
using System;

namespace Humidity.UI.ViewModels.Factories
{
    public class HumidityViewModelFactory : IHumidityViewModelFactory
    {
        private readonly CreateViewModel<HomeViewModel> _createHomeViewModel;

        public HumidityViewModelFactory(CreateViewModel<HomeViewModel> createHomeViewModel)
        {
            _createHomeViewModel = createHomeViewModel;
        }

        public BaseViewModel CreateViewModel(ViewType viewType)
        {
            switch (viewType)
            {
                case ViewType.Home:
                    return _createHomeViewModel();
                default:
                    throw new ArgumentException("The ViewType does nog have a ViewModel.", "viewType");
            }
        }
    }
}
