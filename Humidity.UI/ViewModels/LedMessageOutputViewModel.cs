using System.Windows.Media;

namespace Humidity.UI.ViewModels
{   
    public class LedMessageOutputViewModel : BaseViewModel
    {
        #region Protected Members

        private Color _innerColor;

        private Color _outerColor;

        private string _message;

        #endregion

        #region Public Properties

        public Color InnerColor
        {
            get => _innerColor;
            set
            {
                _innerColor = value;
                OnPropertyChanged(nameof(InnerColor));
            }
        }

        public Color OuterColor
        {
            get => _outerColor;
            set
            {
                _outerColor = value;
                OnPropertyChanged(nameof(OuterColor));
            }
        }

        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged(nameof(Message));
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public LedMessageOutputViewModel()
        {

        }

        #endregion
    }
}
