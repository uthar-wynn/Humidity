using Humidity.UI.State.Navigators;
using Humidity.UI.ViewModels.Factories;
using System;
using System.Windows.Input;

namespace Humidity.UI.Commands
{
    /// <summary>
    /// A command that changes the view model
    /// </summary>
    public class UpdateCurrentViewModelCommand : ICommand
    {
        #region Private Members

        private readonly INavigator _navigator;
        private readonly IHumidityViewModelFactory _viewModelFactory;

        #endregion

        #region Public Events

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };

        #endregion

        #region Constructor

        public UpdateCurrentViewModelCommand(INavigator navigator, IHumidityViewModelFactory viewModelFactory)
        {
            _navigator = navigator;
            _viewModelFactory = viewModelFactory;
        }

        #endregion

        #region Command Methods

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (parameter is ViewType)
            {
                ViewType viewType = (ViewType)parameter;

                _navigator.CurrentViewModel = _viewModelFactory.CreateViewModel(viewType);
            }
        }

        #endregion
    }
}
