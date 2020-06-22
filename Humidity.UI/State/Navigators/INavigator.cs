using Humidity.UI.ViewModels;

namespace Humidity.UI.State.Navigators
{
    public enum ViewType
    {
        Home
    }

    public interface INavigator
    {
        BaseViewModel CurrentViewModel { get; set; }
    }
}
