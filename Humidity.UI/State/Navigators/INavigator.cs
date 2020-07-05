using Humidity.UI.ViewModels;

namespace Humidity.UI.State.Navigators
{
    public enum ViewType
    {
        Home,
        Log
    }

    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
    }
}
