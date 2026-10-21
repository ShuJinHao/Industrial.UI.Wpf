using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Industrial.UI.Wpf.Models;

namespace Industrial.UI.Wpf.Controls
{
    public class DeviceCard : Control
    {
        static DeviceCard() { DefaultStyleKeyProperty.OverrideMetadata(typeof(DeviceCard), new FrameworkPropertyMetadata(typeof(DeviceCard))); }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(DeviceCard), new PropertyMetadata(""));
        public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(nameof(Subtitle), typeof(string), typeof(DeviceCard), new PropertyMetadata(""));
        public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(nameof(Status), typeof(DeviceStatus), typeof(DeviceCard), new PropertyMetadata(DeviceStatus.Unknown));
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.Register(nameof(Shape), typeof(CardShape), typeof(DeviceCard), new PropertyMetadata(CardShape.Panel));
        public static readonly DependencyProperty MetricsSourceProperty = DependencyProperty.Register(nameof(MetricsSource), typeof(IEnumerable), typeof(DeviceCard));
        public static readonly DependencyProperty MetricTemplateProperty = DependencyProperty.Register(nameof(MetricTemplate), typeof(DataTemplate), typeof(DeviceCard));
        public static readonly DependencyProperty DetailCommandProperty = DependencyProperty.Register(nameof(DetailCommand), typeof(ICommand), typeof(DeviceCard));
        public static readonly DependencyProperty DetailCommandParameterProperty = DependencyProperty.Register(nameof(DetailCommandParameter), typeof(object), typeof(DeviceCard));
        public static readonly DependencyProperty DetailTextProperty = DependencyProperty.Register(nameof(DetailText), typeof(string), typeof(DeviceCard), new PropertyMetadata("详情"));
        public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
        public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
        public DeviceStatus Status { get => (DeviceStatus)GetValue(StatusProperty); set => SetValue(StatusProperty, value); }
        public CardShape Shape { get => (CardShape)GetValue(ShapeProperty); set => SetValue(ShapeProperty, value); }
        public IEnumerable MetricsSource { get => (IEnumerable)GetValue(MetricsSourceProperty); set => SetValue(MetricsSourceProperty, value); }
        public DataTemplate MetricTemplate { get => (DataTemplate)GetValue(MetricTemplateProperty); set => SetValue(MetricTemplateProperty, value); }
        public ICommand DetailCommand { get => (ICommand)GetValue(DetailCommandProperty); set => SetValue(DetailCommandProperty, value); }
        public object DetailCommandParameter { get => GetValue(DetailCommandParameterProperty); set => SetValue(DetailCommandParameterProperty, value); }
        public string DetailText { get => (string)GetValue(DetailTextProperty); set => SetValue(DetailTextProperty, value); }
    }
}
