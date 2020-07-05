using System;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Humidity.UI.ViewModels
{
    /// <summary>
    /// The view model for a <see cref="GraphOutputControl"/>
    /// </summary>
    public class GraphOutputViewModel : BaseViewModel
    {
        #region Protected Members

        private bool _isGraphVisible = true;

        private double _value;

        private double[] _stability = new double[60];

        private int _stabilityIndex = 0;

        private LedMessageOutputViewModel _ledMessage;

        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the measured values
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The unit of the measured values eg °C, V, mA, % rH, ...
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Need this particulary graph be visible or not
        /// </summary>
        public bool IsGraphVisible
        {
            get => _isGraphVisible;
            set
            {
                _isGraphVisible = value;
                OnPropertyChanged(nameof(IsGraphVisible));
                ChangeVisibility();
            }
        }

        /// <summary>
        /// The measured value
        /// </summary>
        public double ComponentValue
        {
            get => _value;
            set
            {
                _value = value;
                Task.Run(() => CheckStability()).Wait();
                OnPropertyChanged(nameof(ComponentValue));
            }
        }

        /// <summary>
        /// A view model for the <see cref="LedMessageOutputControl"/>
        /// </summary>
        public LedMessageOutputViewModel LedMessage
        {
            get => _ledMessage;
            set
            {
                _ledMessage = value;
                OnPropertyChanged(nameof(LedMessage));
            }
        }

        /// <summary>
        /// The action to run when the checkbox has changed
        /// </summary>
        public Action CommitAction { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public GraphOutputViewModel()
        {
            LedMessage = new LedMessageOutputViewModel
            {
                Message = "No Message",
                InnerColor = Color.FromRgb(211, 211, 211),
                OuterColor = Color.FromRgb(128, 128, 128)
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Action to commit component visibility on the charts
        /// </summary>
        public void ChangeVisibility()
        {
            CommitAction?.Invoke();
        }

        /// <summary>
        /// Checks the stability of the sensor when it is running
        /// </summary>
        private void CheckStability()
        {
            if (_stabilityIndex == 60)
                _stabilityIndex = 0;

            _stability[_stabilityIndex] = _value;

            double _avgValue = 0;

            for (int i = 0; i < 59; i++)
            {
                _avgValue += _stability[i];
            }

            _avgValue /= 60;

            double __diffValue = Math.Abs(_avgValue - _value);

            LedMessage.Message = string.Format("Gemiddelde: {0:F2} Verschil {1:F2}", _avgValue, __diffValue);
            LedMessage.InnerColor = GetInnerColor(__diffValue);
            LedMessage.OuterColor = GetOuterColor(__diffValue);

            _stabilityIndex++;
        }

        /// <summary>
        /// Get the lighter color for the led, based on the difference given
        /// </summary>
        /// <param name="difference">The differnce</param>
        /// <returns>Color based on the difference</returns>
        private Color GetInnerColor(double difference)
        {
            switch (difference)
            {
                case var _ when difference < 0.1:
                    return Color.FromRgb(144, 238, 144); // Light green
                case var _ when difference < 0.5:
                    return Color.FromRgb(255, 165, 0); // Orange                   
                default:
                    return Color.FromRgb(240, 128, 128); // Light coral                    
            }

        }

        /// <summary>
        /// Get the standard color for the led, based on the difference given
        /// </summary>
        /// <param name="difference">The difference</param>
        /// <returns>Color based on the difference</returns>
        private Color GetOuterColor(double difference)
        {
            switch (difference)
            {
                case var _ when difference < 0.1:
                    return Color.FromRgb(0, 128, 0); // Green                   
                case var _ when difference < 0.5:
                    return Color.FromRgb(255, 140, 0); // Dark orange                    
                default:
                    return Color.FromRgb(128, 0, 0); // Red                   
            }
        }

        #endregion
    }
}
