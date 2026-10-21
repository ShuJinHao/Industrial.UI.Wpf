using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Industrial.UI.Wpf.Models;

namespace Industrial.UI.Wpf.Controls
{
    public class DeviceGrid : ListBox
    {
        static DeviceGrid() { DefaultStyleKeyProperty.OverrideMetadata(typeof(DeviceGrid), new FrameworkPropertyMetadata(typeof(DeviceGrid))); }
        public static readonly DependencyProperty CardWidthProperty = DependencyProperty.Register(nameof(CardWidth), typeof(double), typeof(DeviceGrid), new PropertyMetadata(152d), IsPositiveFinite);
        public double CardWidth { get => (double)GetValue(CardWidthProperty); set => SetValue(CardWidthProperty, value); }
        public static readonly DependencyProperty CardShapeProperty = DependencyProperty.Register(nameof(CardShape), typeof(CardShape), typeof(DeviceGrid), new PropertyMetadata(CardShape.Panel));
        public static readonly DependencyProperty DetailCommandProperty = DependencyProperty.Register(nameof(DetailCommand), typeof(ICommand), typeof(DeviceGrid));
        public CardShape CardShape { get => (CardShape)GetValue(CardShapeProperty); set => SetValue(CardShapeProperty, value); }
        public ICommand DetailCommand { get => (ICommand)GetValue(DetailCommandProperty); set => SetValue(DetailCommandProperty, value); }
        private static bool IsPositiveFinite(object value)
        {
            var number = (double)value;
            return number > 0 && !double.IsNaN(number) && !double.IsInfinity(number);
        }
    }
}
