using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Industrial.UI.Wpf.Models;

namespace Industrial.UI.Wpf.Controls
{
    public sealed class StatusBadge : Control
    {
        static StatusBadge() { DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBadge), new FrameworkPropertyMetadata(typeof(StatusBadge))); }
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(DeviceStatus), typeof(StatusBadge), new PropertyMetadata(DeviceStatus.Unknown));
        public DeviceStatus Status { get => (DeviceStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }
    }

    public sealed class StatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is DeviceStatus)) return "未知";
            switch ((DeviceStatus)value)
            {
                case DeviceStatus.Normal: return "正常";
                case DeviceStatus.Running: return "运行";
                case DeviceStatus.Waiting: return "等待";
                case DeviceStatus.Warning: return "预警";
                case DeviceStatus.Alarm: return "报警";
                case DeviceStatus.Offline: return "离线";
                default: return "未知";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    }
}
