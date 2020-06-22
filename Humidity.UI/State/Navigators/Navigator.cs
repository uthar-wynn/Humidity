using Humidity.UI.ViewModels;

namespace Humidity.UI.State.Navigators
{
    public class Navigator : BaseViewModel, INavigator
    {
        private BaseViewModel _currentViewModel;

        public BaseViewModel CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }
    }
}
