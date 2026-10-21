using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using Industrial.UI.Wpf.Models;

namespace Industrial.UI.Wpf.Demo
{
    public sealed class DemoViewModel : ObservableObject, IDisposable
    {
        private EquipmentGroup _selectedTray, _selectedCabinet;
        private DeviceItem _selectedCell, _selectedSensor;
        private string _lastAction = "选择左侧列表，再点击任意卡片的“详情”。", _lastRefresh = "";
        private readonly DispatcherTimer _timer;
        private readonly Random _random = new Random(17);
        public ObservableCollection<EquipmentGroup> Trays { get; } = new ObservableCollection<EquipmentGroup>();
        public ObservableCollection<EquipmentGroup> Cabinets { get; } = new ObservableCollection<EquipmentGroup>();
        public EquipmentGroup SelectedTray { get => _selectedTray; set => Set(ref _selectedTray, value); }
        public EquipmentGroup SelectedCabinet { get => _selectedCabinet; set => Set(ref _selectedCabinet, value); }
        public DeviceItem SelectedCell { get => _selectedCell; set => Set(ref _selectedCell, value); }
        public DeviceItem SelectedSensor { get => _selectedSensor; set => Set(ref _selectedSensor, value); }
        public string LastAction { get => _lastAction; private set => Set(ref _lastAction, value); }
        public string LastRefresh { get => _lastRefresh; private set => Set(ref _lastRefresh, value); }
        public string SimulationButtonText => _timer.IsEnabled ? "暂停模拟刷新" : "继续模拟刷新";
        public ICommand DetailCommand { get; }
        public ICommand ToggleSimulationCommand { get; }

        public DemoViewModel()
        {
            DetailCommand = new DemoCommand(value =>
            {
                var item = (DeviceItem)value;
                LastAction = "详情命令已交给业务方：" + item.Title + " / " + item.Id;
            }, value => value is DeviceItem);
            for (var groupIndex = 1; groupIndex <= 4; groupIndex++)
            {
                var group = new EquipmentGroup { Id = "DEMO-TRAY-" + groupIndex, Title = "托盘 " + groupIndex.ToString("00"), Subtitle = "演示库位 A-" + groupIndex.ToString("00"), Status = DeviceStatus.Running };
                for (var i = 1; i <= 24; i++)
                {
                    var status = i == 5 ? DeviceStatus.Alarm : i == 9 ? DeviceStatus.Warning : i == 13 ? DeviceStatus.Offline : i == 18 ? DeviceStatus.Unknown : DeviceStatus.Normal;
                    var missing = status == DeviceStatus.Offline || status == DeviceStatus.Unknown;
                    var cell = new DeviceItem { Id = group.Id + "-" + i, Title = "#" + i.ToString("000"), Subtitle = "电芯测点", Status = status };
                    cell.Measurements.Add(new MeasurementItem { Label = "负压", Value = missing ? null : (-45 - i * 0.1).ToString("F1", CultureInfo.InvariantCulture), Unit = "kPa" });
                    cell.Measurements.Add(new MeasurementItem { Label = "温度", Value = missing ? null : i == 5 ? "58.6" : i == 9 ? "46.2" : (29 + i * 0.12).ToString("F1", CultureInfo.InvariantCulture), Unit = "℃", Status = status });
                    group.Items.Add(cell);
                }
                Trays.Add(group);
            }
            for (var rack = 1; rack <= 3; rack++)
            {
                var group = new EquipmentGroup { Id = "DEMO-RACK-" + rack, Title = "环境机柜 " + rack.ToString("00"), Subtitle = "相同控件 / 普通设备卡片", Status = DeviceStatus.Normal };
                for (var i = 1; i <= 12; i++)
                {
                    var sensor = new DeviceItem { Id = group.Id + "-" + i, Title = "传感器 " + i.ToString("00"), Subtitle = "区域 " + ((i - 1) / 4 + 1), Status = DeviceStatus.Normal };
                    sensor.Measurements.Add(new MeasurementItem { Label = "温度", Value = "26.5", Unit = "℃" });
                    sensor.Measurements.Add(new MeasurementItem { Label = "湿度", Value = "48.2", Unit = "%RH" });
                    sensor.Measurements.Add(new MeasurementItem { Label = "压差", Value = "12.0", Unit = "Pa" });
                    group.Items.Add(sensor);
                }
                Cabinets.Add(group);
            }
            SelectedTray = Trays[0];
            SelectedCabinet = Cabinets[0];
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += Tick;
            ToggleSimulationCommand = new DemoCommand(value =>
            {
                if (_timer.IsEnabled) _timer.Stop(); else _timer.Start();
                OnPropertyChanged(nameof(SimulationButtonText));
            });
            _timer.Start();
        }

        private void Tick(object sender, EventArgs args)
        {
            // Simulated values belong to the sample application, never to the control library.
            foreach (var group in Trays.Concat(Cabinets))
                foreach (var device in group.Items)
                {
                    if (device.Status != DeviceStatus.Normal) continue;
                    device.Measurements[0].Value = (group.Id.StartsWith("DEMO-TRAY", StringComparison.Ordinal) ? -45 + _random.NextDouble() : 26 + _random.NextDouble()).ToString("F1", CultureInfo.InvariantCulture);
                }
            LastRefresh = "模拟刷新 " + DateTime.Now.ToString("HH:mm:ss");
        }

        public void Dispose() { _timer.Stop(); _timer.Tick -= Tick; }
    }

    public sealed class DemoCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        public DemoCommand(Action<object> execute, Predicate<object> canExecute = null) { _execute = execute; _canExecute = canExecute; }
        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged { add { CommandManager.RequerySuggested += value; } remove { CommandManager.RequerySuggested -= value; } }
    }
}
