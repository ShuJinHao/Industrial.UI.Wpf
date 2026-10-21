using System.Collections.ObjectModel;

namespace Industrial.UI.Wpf.Models
{
    public enum DeviceStatus { Unknown, Normal, Running, Waiting, Warning, Alarm, Offline }
    public enum CardShape { Panel, Battery }

    // Optional presentation models. Custom ItemTemplates may bind to the caller's own types.
    public sealed class MeasurementItem : ObservableObject
    {
        private string _label = "", _value, _unit = "";
        private DeviceStatus _status = DeviceStatus.Normal;
        public string Label { get => _label; set => Set(ref _label, value); }
        public string Value
        {
            get => _value;
            set { if (Set(ref _value, value)) OnPropertyChanged(nameof(DisplayValue)); }
        }
        public string DisplayValue => string.IsNullOrWhiteSpace(Value) ? "—" : Value;
        public string Unit { get => _unit; set => Set(ref _unit, value); }
        public DeviceStatus Status { get => _status; set => Set(ref _status, value); }
    }

    public sealed class DeviceItem : ObservableObject
    {
        private string _id = "", _title = "", _subtitle = "";
        private DeviceStatus _status = DeviceStatus.Unknown;
        public string Id { get => _id; set => Set(ref _id, value); }
        public string Title { get => _title; set => Set(ref _title, value); }
        public string Subtitle { get => _subtitle; set => Set(ref _subtitle, value); }
        public DeviceStatus Status { get => _status; set => Set(ref _status, value); }
        public ObservableCollection<MeasurementItem> Measurements { get; } = new ObservableCollection<MeasurementItem>();
        public object Tag { get; set; }
    }

    public sealed class EquipmentGroup : ObservableObject
    {
        private string _id = "", _title = "", _subtitle = "";
        private DeviceStatus _status = DeviceStatus.Unknown;
        public string Id { get => _id; set => Set(ref _id, value); }
        public string Title { get => _title; set => Set(ref _title, value); }
        public string Subtitle { get => _subtitle; set => Set(ref _subtitle, value); }
        public DeviceStatus Status { get => _status; set => Set(ref _status, value); }
        public ObservableCollection<DeviceItem> Items { get; } = new ObservableCollection<DeviceItem>();
        public object Tag { get; set; }
    }
}
