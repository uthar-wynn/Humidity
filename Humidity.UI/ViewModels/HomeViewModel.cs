using Humidity.Core.Models;
using Humidity.Core.Sensor;
using Humidity.Core.Services;
using Humidity.UI.ViewModels.Base;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
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

        protected int _selectedSensorResolution;

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

        #endregion

        #region Public Properties

        public bool IsRunning { get; set; }

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
                // SensorResolution();
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

        public ICommand StartMeasuringCommand { get; set; }

        public ICommand StopMeasuringCommand { get; set; }

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

        }

        #endregion

        #region Methods

        private async void _dispatcherTimer_Tick(object sender, System.EventArgs e)
        {
            // Retrieve the values from the sensor
            GetValueSensor((byte)SensorStatus.Trigger_T_measurement_hold);
            Temperature = GetTemperature();
            GetValueSensor((byte)SensorStatus.Trigger_RH_measurement_hold);
            Humidity = GetHumidity();

            // Calculates the dewpoint
            Dewpoint = await _calculator.CalculateDewpoint(_temperature, _humidty);

            // Update the values in the controls aside
            GraphTemperature.ComponentValue = _temperature;
            GraphHumidity.ComponentValue = _humidty;
            GraphDewpoint.ComponentValue = _dewpoint;

            WriteChartAsync();
        }

        // async Task ?
        private void WriteChartAsync()
        {
            ChartValuesTemperature.Add(new MeasureDataModel
            {
                DateTime = DateTime.Now,
                Value = Temperature
            });
            ChartValuesHumidity.Add(new MeasureDataModel
            {
                DateTime = DateTime.Now,
                Value = Humidity
            });
            ChartValuesDewpoint.Add(new MeasureDataModel
            {
                DateTime = DateTime.Now,
                Value = Dewpoint
            });

            // Create space between last point and border chart
            AxisMax = DateTime.Now.Ticks + TimeSpan.FromSeconds(5).Ticks; //1

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
                ChartValuesTemperature.RemoveAt(0);
                ChartValuesHumidity.RemoveAt(0);
                ChartValuesDewpoint.RemoveAt(0);
                AxisMin = DateTime.Now.Ticks - TimeSpan.FromSeconds(304).Ticks;
            }
        }

        private void StartMeasuring()
        {
            IsRunning = true;

            Init();

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

        private void Init()
        {
            try
            {
                Sensor.IowKitOpenDevice();
                HandleDevices = _sensorManager.GetCountSensors();

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
                MessageBox.Show("Error");
                //await ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                //{
                //    Message = "IIC Special mode function write failed",
                //    Title = "Error"
                //});
            }
        }

        /// <summary>
        /// Filling the buffer with the values comming from the sensor with the given trigger
        /// </summary>
        /// <param name="trigger">byte for the kind of parameter required</param>
        private void GetValueSensor(byte trigger)
        {
            try
            {
                _buffer.byte0 = 2;
                _buffer.byte1 = 194;
                _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
                _buffer.byte3 = trigger;

                Sensor.IowKitWrite(mIOHandle[1], 1, ref _buffer, 8);
                Sensor.IowKitRead(mIOHandle[1], 1, ref _buffer, 8);

                if (_buffer.byte1 == 128)
                {
                    IsRunning = false;
                    _dispatcherTimer.Stop();
                    //ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                    //{
                    //    Message = "IIC no ack from sensor during write",
                    //    Title = "Error"
                    //});
                }


                _buffer.byte0 = 3;
                _buffer.byte1 = 3;
                _buffer.byte2 = (byte)SensorStatus.sh21adr_read;

                Sensor.IowKitWrite(mIOHandle[1], 1, ref _buffer, 8);
                Sensor.IowKitRead(mIOHandle[1], 1, ref _buffer, 8);

                if (_buffer.byte1 == 128)
                {
                    IsRunning = false;
                    _dispatcherTimer.Stop();
                    //ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                    //{
                    //    Message = "IIC no ack from sensor during read",
                    //    Title = "Error"
                    //});
                }


            }
            catch (Exception ex)
            {
                IsRunning = false;
                _dispatcherTimer.Stop();
                //ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                //{
                //    Message = ex.Message,
                //    Title = "Error"
                //});
            }
        }

        /// <summary>
        /// Writing values to the sensor, eg: heating etc...
        /// </summary>
        /// <param name="trigger">byte for the kind of parameter required</param>
        /// <param name="status">status for the sensor</param>
        private void SetValueSensor(byte trigger, byte status)
        {
            try
            {
                _buffer.byte0 = 2;
                _buffer.byte1 = 195;
                _buffer.byte2 = (byte)SensorStatus.sh21adr_write;
                _buffer.byte3 = trigger;
                _buffer.byte4 = status;

                Sensor.IowKitWrite(mIOHandle[1], 1, ref _buffer, 8);
                Sensor.IowKitRead(mIOHandle[1], 1, ref _buffer, 8);

                if (_buffer.byte1 == 128)
                {
                    IsRunning = false;
                    _dispatcherTimer.Stop();
                    //ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                    //{
                    //    Message = "IIC no ack from sensor during read",
                    //    Title = "Error"
                    //});
                }
            }
            catch (Exception ex)
            {
                IsRunning = false;
                _dispatcherTimer.Stop();
                //ManagerUI.ShowMessage(new MessageBoxDialogViewModel
                //{
                //    Message = ex.Message,
                //    Title = "Error"
                //});
            }
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

        /// <summary>
        /// Retrieve the temperature from the sensor
        /// </summary>
        /// <returns>The temperature in deg C</returns>
        private double GetTemperature()
        {
            int mask_filter = 65532;
            int value = _buffer.byte2 << 8;
            value += _buffer.byte3;
            value = (value & mask_filter);
            return -46.85 + 175.72 * ((double)value / 65536);
        }

        /// <summary>
        /// Retrieve the humidity from the sensor
        /// </summary>
        /// <returns>The humidity in %rH</returns>
        private double GetHumidity()
        {
            int mask_filter = 65532;
            int value = _buffer.byte2 << 8;
            value += _buffer.byte3;
            value = (value & mask_filter);
            return -6 + 125 * ((double)value / 65536);
        }

        #endregion
    }
}
