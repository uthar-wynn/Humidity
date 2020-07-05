using Humidity.Core.Models;
using Humidity.Core.Sensor;
using Humidity.Core.Services;
using Humidity.UI.ViewModels.Base;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Humidity.UI.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        #region Protected Members   

        private ISensorManager _sensorManager;

        private ICalculator _calculator;

        private DispatcherTimer _dispatcherTimer;

        private int _timeInterval = 1;

        private int _handleDevices;

        private bool _devicesConnected = false;

        private double _temperature;

        private double _humidty;

        private double _dewpoint;

        private List<int> mIOHandle = new List<int>();

        private Eight _buffer;

        private double _axisMax;

        private double _axisMin;

        private int _selectedSensorResolution;

        private Object[] _tempValuesTemperature = new Object[5];

        private Object[] _tempValuesHumidity = new Object[5];

        private Object[] _tempValuesDewpoint = new Object[5];

        private int _counter = 0;

        #endregion

        #region Public Properties

        /// <summary>
        /// Not sure yet ?
        /// </summary>
        public string IOHandle { get; set; }

        /// <summary>
        /// Devices found on this pc
        /// </summary>
        public string Found { get; set; }

        /// <summary>
        /// The ID from the  device
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// A flag indicating the application is measuring
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// A flag indicating if the application is writing the measured points
        /// </summary>
        public bool IsLogging { get; set; }

        /// <summary>
        /// A flag indicating if the heater is on or not
        /// </summary>
        public bool IsHeating { get; set; } = false;

        /// <summary>
        /// A flag indicating the application is comparing results
        /// </summary>
        public bool IsCompressor { get; set; }

        public int TimeInterval
        {
            get => _timeInterval;
            set
            {
                _timeInterval = value;
                OnPropertyChanged(nameof(TimeInterval));
            }
        }

        /// <summary>
        /// The temperature from the sensor
        /// </summary>
        public double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }

        /// <summary>
        /// The humidity from the sensor
        /// </summary>
        public double Humidity
        {
            get => _humidty;
            set
            {
                _humidty = value;
                OnPropertyChanged(nameof(Humidity));
            }
        }

        /// <summary>
        /// The dewpoint based on the temperature and humidty from the sensor
        /// </summary>     
        public double Dewpoint
        {
            get => _dewpoint;
            set
            {
                _dewpoint = value;
                OnPropertyChanged(nameof(Dewpoint));
            }
        }

        /// <summary>
        /// The humidty from the cylinder, used for comparing by <see cref="CompressorControl"/>
        /// </summary>
        public double Humidity_cylinder { get; set; } = 0;

        /// <summary>
        /// The humidty from the compressor, used for comparing by <see cref="CompressorControl"/>
        /// </summary>
        public double Humidity_compressor { get; set; } = 0;

        /// <summary>
        /// The humidity difference between cylinder and compressor, used for comparing by <see cref="CompressorControl"/>
        /// </summary>
        public double Humidity_difference { get; set; } = 0;

        /// <summary>
        /// The calculated dewpoint from the compressor, used for comparing by <see cref="CompressorControl"/>
        /// </summary>
        public double Dewpoint_compressor { get; set; } = 0;

        /// <summary>
        /// The count of devices connected
        /// </summary>

        public int HandleDevices
        {
            get => _handleDevices;
            set
            {
                _handleDevices = value;
                OnPropertyChanged(nameof(HandleDevices));
            }
        }

        public bool DevicesConnected
        {
            get => _devicesConnected;
            set
            {
                _devicesConnected = value;
                OnPropertyChanged(nameof(DevicesConnected));
            }
        }

        public GraphOutputViewModel GraphTemperature { get; set; }

        public GraphOutputViewModel GraphHumidity { get; set; }

        public GraphOutputViewModel GraphDewpoint { get; set; }

        public LedMessageOutputViewModel CompressorMessageViewModel { get; set; }

        public ChartValues<MeasureDataModel> ChartValuesTemperature { get; set; }

        public ChartValues<MeasureDataModel> ChartValuesHumidity { get; set; }

        public ChartValues<MeasureDataModel> ChartValuesDewpoint { get; set; }

        public Func<double, string> DateTimeFormatter { get; set; }

        public double AxisMax
        {
            get => _axisMax;
            set
            {
                _axisMax = value;
                OnPropertyChanged(nameof(AxisMax));
            }
        }

        public double AxisMin
        {
            get => _axisMin;
            set
            {
                _axisMin = value;
                OnPropertyChanged(nameof(AxisMin));
            }
        }

        public double AxisStep { get; set; }

        public double AxisUnit { get; set; }

        /// <summary>
        /// The selected sensor resolution
        /// </summary>
        public int SelectedSensorResolution
        {
            get => _selectedSensorResolution;
            set
            {
                _selectedSensorResolution = value;
                OnPropertyChanged(nameof(SelectedSensorResolution));
                SensorResolution();
            }
        }

        /// <summary>
        /// A flag indicating if the temperature is showing on the charts
        /// </summary>
        public bool ChartTemperatureVisible { get; set; } = true;

        /// <summary>
        /// A flag indicating if the humidity is showing on the charts
        /// </summary>
        public bool ChartHumidityVisible { get; set; } = true;

        /// <summary>
        /// A flag indicating if the dewpoint is showing on the charts
        /// </summary>
        public bool ChartDewpointVisible { get; set; } = true;

        #endregion

        #region Public Commands

        /// <summary>
        /// A command when the Run button is clicked
        /// </summary>
        public ICommand StartMeasuringCommand { get; set; }

        /// <summary>
        /// A command when the Stop button is clicked
        /// </summary>
        public ICommand StopMeasuringCommand { get; set; }

        /// <summary>
        /// A command when the Heater box is clicked
        /// </summary>
        public ICommand HeaterCommand { get; set; }

        /// <summary>
        /// A command when the take humidity button is clicked
        /// </summary>
        public ICommand GetHumiditySensorCommand { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public HomeViewModel(ISensorManager sensorManager, ICalculator calculator)
        {
            _sensorManager = sensorManager;
            _calculator = calculator;

            // The commands
            StartMeasuringCommand = new RelayCommand(StartMeasuring);
            StopMeasuringCommand = new RelayCommand(StopMeasuring);
            HeaterCommand = new RelayCommand(Heater);
            GetHumiditySensorCommand = new RelayCommand(() =>
            {
                Humidity_cylinder = _humidty;
            });

            // The Timer
            _dispatcherTimer = new DispatcherTimer(DispatcherPriority.DataBind);
            _dispatcherTimer.Tick += _dispatcherTimer_Tick;

            // Create the controls aside the charts
            GraphTemperature = new GraphOutputViewModel
            {
                Name = "Temperatuur",
                ComponentValue = Temperature,
                Unit = "°C",
                IsGraphVisible = ChartTemperatureVisible,
                CommitAction = ChangeTemperatureVisibility
            };

            GraphHumidity = new GraphOutputViewModel
            {
                Name = "Vochtigheid",
                ComponentValue = Humidity,
                Unit = "%rH",
                IsGraphVisible = ChartHumidityVisible,
                CommitAction = ChangeHumidityVisibility
            };

            GraphDewpoint = new GraphOutputViewModel
            {
                Name = "Dauwpunt",
                ComponentValue = Humidity,
                Unit = "°C",
                IsGraphVisible = ChartDewpointVisible,
                CommitAction = ChangeDewpointVisibility
            };

            // Charts
            var mapper = Mappers.Xy<MeasureDataModel>()
                .X(m => m.DateTime.Ticks)
                .Y(m => m.Value);

            Charting.For<MeasureDataModel>(mapper);
            DateTimeFormatter = value => new DateTime((long)value).ToString("HH:mm:ss");
            AxisStep = TimeSpan.FromSeconds(20).Ticks;
            AxisUnit = TimeSpan.TicksPerSecond;
            ClearCharts();

            // Compressor
            CompressorMessageViewModel = new LedMessageOutputViewModel
            {
                Message = "Not Running",
                InnerColor = Color.FromRgb(211, 211, 211), // Light gray
                OuterColor = Color.FromRgb(128, 128, 128) // Gray
            };
        }

        #endregion

        #region Methods

        private async void _dispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            // Retrieve the values from the sensor            
            Temperature = await Task.Run(() => GetTemperature());
            Humidity = await Task.Run(() => GetHumidity());

            // Calculates the dewpoint
            Dewpoint = await _calculator.CalculateDewpoint(_temperature, _humidty);

            // Update the values in the controls aside
            GraphTemperature.ComponentValue = _temperature;
            GraphHumidity.ComponentValue = _humidty;
            GraphDewpoint.ComponentValue = _dewpoint;

            // Add the values to the ChartValues and update the chart every 5s
            _tempValuesTemperature[_counter] = await AddValues(_temperature);
            _tempValuesHumidity[_counter] = await AddValues(_humidty);
            _tempValuesDewpoint[_counter] = await AddValues(_dewpoint);

            if (_counter == 4)
                await Task.Run(() => WriteChartAsync());

            _counter++;

            if (_counter == 5)
                _counter = 0;

            // Compressor control
            if (IsCompressor)
                await Task.Run(() => Dewpoint_Compressor());
        }

        private async Task<MeasureDataModel> AddValues(double value)
        {
            return await Task.Run(() => new MeasureDataModel
            {
                DateTime = DateTime.Now,
                Value = value
            });
        }

        private async void StartMeasuring()
        {
            IsRunning = true;

            await Task.Run(() => Init());

            if (DevicesConnected)
            {
                _sensorManager.ReadSensors(ref mIOHandle);

                EnableSensor();

                _sensorManager.ResetSensor(ref mIOHandle);

                GetSensorInformation();

                if (IsRunning)
                {
                    ClearCharts();
                    _dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
                    _dispatcherTimer.Start();
                }
            }
            else
            {
                IsRunning = false;
                MessageBox.Show("No sensors found");
            }
        }

        private void StopMeasuring()
        {
            IsRunning = false;
            _dispatcherTimer.Stop();
        }

        private async void Heater()
        {
            byte heater_mask;

            if (IsHeating)
                heater_mask = 4;
            else
                heater_mask = 251;

            _buffer = await _sensorManager.GetValueSensor(mIOHandle[1], (byte)SensorStatus.read_user_register);
            var status = _buffer.byte2;
            status |= heater_mask;
            Task.Run(() => _sensorManager.SetValueSensor(mIOHandle[1], (byte)SensorStatus.write_user_register, (byte)status)).Wait();
        }

        private async void SensorResolution()
        {
            byte heater_mask = 126;

            switch (_selectedSensorResolution)
            {
                case 0:
                    heater_mask = 126;
                    break;
                case 1:
                    heater_mask = 127;
                    break;
                case 2:
                    heater_mask = 128;
                    break;
                case 3:
                    heater_mask = 129;
                    break;
                default:
                    heater_mask = 126;
                    break;
            }

            _buffer = await _sensorManager.GetValueSensor(mIOHandle[1], (byte)SensorStatus.read_user_register);
            var status = _buffer.byte2;
            status |= heater_mask;
            Task.Run(() => _sensorManager.SetValueSensor(mIOHandle[1], (byte)SensorStatus.write_user_register, (byte)status)).Wait();
        }

        private void StartLogging()
        {
            IsLogging = true;
        }

        private void StopLogging()
        {
            IsLogging = false;
        }

        private void Init()
        {
            try
            {
                Task openDevices = Task.Run(() => Sensor.IowKitOpenDevice());
                openDevices.Wait();
                Task countSensors = Task.Run(async () => HandleDevices = await _sensorManager.GetCountSensors());
                countSensors.Wait();

                if (HandleDevices > 0)
                    DevicesConnected = true;
                else
                    DevicesConnected = false;
            }
            catch (Exception ex)
            {
                IsRunning = false;
                MessageBox.Show(ex.Message);
            }
        }

        #endregion

        #region Sensor

        /// <summary>
        /// Enable the IO Warrior kit before enablic IIC and sensibus
        /// </summary>
        private void EnableSensor()
        {
            int writeHex = _sensorManager.EnableSensor(mIOHandle);
            if (writeHex == 0)
            {
                IsRunning = false;
                _dispatcherTimer.Stop();
                MessageBox.Show("IIC Special mode function write failed");
                //await ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                //{
                //    Message = "IIC Special mode function write failed",
                //    Title = "Error"
                //});
            }
        }

        /// <summary>
        /// Retrieves information about the sensor.
        /// To be called after the sensor is starting to measure
        /// </summary>
        private void GetSensorInformation()
        {
            // DLLVersion = Sensor.IowKitVersion();
            IOHandle = mIOHandle[1].ToString();
            Found = Sensor.IowKitGetNumDevs().ToString();
            ID = Sensor.IowKitGetProductId(mIOHandle[1]).ToString();
        }

        private async void Dewpoint_Compressor()
        {
            Humidity_compressor = _humidty;

            Humidity_difference = Math.Abs(Humidity_compressor - Humidity_cylinder);
            Dewpoint_compressor = await _calculator.CalculateDewpoint(_temperature, Humidity_difference);

            switch (Dewpoint_compressor)
            {
                case var _ when Dewpoint_compressor <= -20 && Dewpoint_compressor >= -60:
                    CompressorMessageViewModel.Message = "Daupunt OK!";
                    CompressorMessageViewModel.InnerColor = Color.FromRgb(144, 238, 144); // Light green
                    CompressorMessageViewModel.OuterColor = Color.FromRgb(0, 128, 0); // Green
                    break;
                case var _ when Dewpoint_compressor > -20:
                    CompressorMessageViewModel.Message = "Daupunt te hoog!";
                    CompressorMessageViewModel.InnerColor = Color.FromRgb(144, 238, 144); // Light blue
                    CompressorMessageViewModel.OuterColor = Color.FromRgb(0, 0, 128); // Blue
                    break;
                case var _ when Dewpoint_compressor < -60:
                    CompressorMessageViewModel.Message = "Dauwpunt te laag!";
                    CompressorMessageViewModel.InnerColor = Color.FromRgb(147, 112, 219); // Medium purple
                    CompressorMessageViewModel.OuterColor = Color.FromRgb(128, 0, 128); // Purple
                    break;
                default:
                    CompressorMessageViewModel.Message = "No Valid Value";
                    CompressorMessageViewModel.InnerColor = Color.FromRgb(211, 211, 211); // Light gray
                    CompressorMessageViewModel.OuterColor = Color.FromRgb(128, 128, 128); // Gray
                    break;
            }
        }

        /// <summary>
        /// Retrieve the temperature from the sensor
        /// </summary>
        /// <returns>The temperature in deg C</returns>
        private async Task<double> GetTemperature()
        {
            // await Task.Run(() => GetValueSensor((byte)SensorStatus.Trigger_T_measurement_hold));
            _buffer = await _sensorManager.GetValueSensor(mIOHandle[1], (byte)SensorStatus.Trigger_T_measurement_hold);

            int mask_filter = 65532;
            int value = _buffer.byte2 << 8;
            value += _buffer.byte3;
            value &= mask_filter;
            return -46.85 + 175.72 * ((double)value / 65536);
        }

        /// <summary>
        /// Retrieve the humidity from the sensor
        /// </summary>
        /// <returns>The humidity in %rH</returns>
        private async Task<double> GetHumidity()
        {
            // await Task.Run(() => GetValueSensor((byte)SensorStatus.Trigger_RH_measurement_hold));
            _buffer = await _sensorManager.GetValueSensor(mIOHandle[1], (byte)SensorStatus.Trigger_RH_measurement_hold);

            int mask_filter = 65532;
            int value = _buffer.byte2 << 8;
            value += _buffer.byte3;
            value &= mask_filter;
            return -6 + 125 * ((double)value / 65536);
        }

        #endregion

        #region Chart

        private async void WriteChartAsync()
        {
            await Task.Run(() =>
            {
                ChartValuesTemperature.AddRange(_tempValuesTemperature);
                ChartValuesHumidity.AddRange(_tempValuesHumidity);
                ChartValuesDewpoint.AddRange(_tempValuesDewpoint);
            });

            // Create space between last point and border chart
            AxisMax = await Task.Run(() => DateTime.Now.Ticks + TimeSpan.FromSeconds(5).Ticks); //1

            // Updates the seperators between times depending on the count of points
            switch (ChartValuesTemperature.Count)
            {
                case int c when (c < 120):
                    AxisStep = TimeSpan.FromSeconds(20).Ticks;
                    break;
                case int c when (c >= 120 && c < 420):
                    AxisStep = TimeSpan.FromSeconds(55).Ticks;
                    break;
                case int c when (c >= 420 && c < 600):
                    AxisStep = TimeSpan.FromSeconds(115).Ticks;
                    break;
                case int c when (c >= 600):
                    AxisStep = TimeSpan.FromSeconds(225).Ticks;
                    break;
                default:
                    AxisStep = TimeSpan.FromSeconds(20).Ticks;
                    break;

            }


            // Remove the old values   
            // For performance reasons keep the max value @300 (= 5min)
            if (ChartValuesTemperature.Count > 300)
            {
                await Task.Run(() =>
                {
                    do
                    {
                        ChartValuesTemperature.RemoveAt(0);
                        ChartValuesHumidity.RemoveAt(0);
                        ChartValuesDewpoint.RemoveAt(0);
                    } while (ChartValuesTemperature.Count > 300);
                });


                AxisMin = await Task.Run(() => DateTime.Now.Ticks - TimeSpan.FromSeconds(304).Ticks);
            }

        }

        private void SetAxisLimits(DateTime dateTime)
        {
            AxisMax = dateTime.Ticks + TimeSpan.FromSeconds(1).Ticks;
            AxisMin = dateTime.Ticks + TimeSpan.FromSeconds(4).Ticks;
        }

        /// <summary>
        /// Clear's the charts data
        /// </summary>
        private void ClearCharts()
        {
            ChartValuesTemperature = new ChartValues<MeasureDataModel>();
            ChartValuesHumidity = new ChartValues<MeasureDataModel>();
            ChartValuesDewpoint = new ChartValues<MeasureDataModel>();
            SetAxisLimits(DateTime.Now);
        }

        /// <summary>
        /// Sets the visibility on the chart for the temperature component
        /// </summary>
        public void ChangeTemperatureVisibility()
        {
            ChartTemperatureVisible = GraphTemperature.IsGraphVisible;
        }

        /// <summary>
        /// Sets the visibility on the chart for the humidity component
        /// </summary>
        public void ChangeHumidityVisibility()
        {
            ChartHumidityVisible = GraphHumidity.IsGraphVisible;
        }

        /// <summary>
        /// Sets the visibility on the chart for the dewpoint component
        /// </summary>
        public void ChangeDewpointVisibility()
        {
            ChartDewpointVisible = GraphDewpoint.IsGraphVisible;
        }

        #endregion
    }
}
