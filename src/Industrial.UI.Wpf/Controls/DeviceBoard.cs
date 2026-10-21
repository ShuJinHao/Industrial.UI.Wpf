using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Industrial.UI.Wpf.Models;

namespace Industrial.UI.Wpf.Controls
{
    public class DeviceBoard : Control
    {
        static DeviceBoard() { DefaultStyleKeyProperty.OverrideMetadata(typeof(DeviceBoard), new FrameworkPropertyMetadata(typeof(DeviceBoard))); }
        public static readonly DependencyProperty GroupsSourceProperty = DependencyProperty.Register(nameof(GroupsSource), typeof(IEnumerable), typeof(DeviceBoard));
        public static readonly DependencyProperty DevicesSourceProperty = DependencyProperty.Register(nameof(DevicesSource), typeof(IEnumerable), typeof(DeviceBoard));
        public static readonly DependencyProperty SelectedGroupProperty = DependencyProperty.Register(nameof(SelectedGroup), typeof(object), typeof(DeviceBoard), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnGroupChanged));
        public static readonly DependencyProperty SelectedDeviceProperty = DependencyProperty.Register(nameof(SelectedDevice), typeof(object), typeof(DeviceBoard), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty GroupItemTemplateProperty = DependencyProperty.Register(nameof(GroupItemTemplate), typeof(DataTemplate), typeof(DeviceBoard));
        public static readonly DependencyProperty DeviceItemTemplateProperty = DependencyProperty.Register(nameof(DeviceItemTemplate), typeof(DataTemplate), typeof(DeviceBoard));
        public static readonly DependencyProperty DetailCommandProperty = DependencyProperty.Register(nameof(DetailCommand), typeof(ICommand), typeof(DeviceBoard));
        public static readonly DependencyProperty CardShapeProperty = DependencyProperty.Register(nameof(CardShape), typeof(CardShape), typeof(DeviceBoard), new PropertyMetadata(CardShape.Panel));
        public static readonly DependencyProperty ListHeaderProperty = DependencyProperty.Register(nameof(ListHeader), typeof(string), typeof(DeviceBoard), new PropertyMetadata("设备列表"));
        public static readonly DependencyProperty CardsHeaderProperty = DependencyProperty.Register(nameof(CardsHeader), typeof(string), typeof(DeviceBoard), new PropertyMetadata("设备数据"));
        public static readonly DependencyProperty EmptyGroupsTextProperty = DependencyProperty.Register(nameof(EmptyGroupsText), typeof(string), typeof(DeviceBoard), new PropertyMetadata("暂无列表数据"));
        public static readonly DependencyProperty EmptyDevicesTextProperty = DependencyProperty.Register(nameof(EmptyDevicesText), typeof(string), typeof(DeviceBoard), new PropertyMetadata("请选择列表项，或当前设备暂无数据"));
        public static readonly DependencyProperty ListWidthProperty = DependencyProperty.Register(nameof(ListWidth), typeof(double), typeof(DeviceBoard), new PropertyMetadata(280d), IsPositiveFinite);
        public static readonly DependencyProperty CardWidthProperty = DependencyProperty.Register(nameof(CardWidth), typeof(double), typeof(DeviceBoard), new PropertyMetadata(152d), IsPositiveFinite);
        public IEnumerable GroupsSource { get => (IEnumerable)GetValue(GroupsSourceProperty); set => SetValue(GroupsSourceProperty, value); }
        public IEnumerable DevicesSource { get => (IEnumerable)GetValue(DevicesSourceProperty); set => SetValue(DevicesSourceProperty, value); }
        public object SelectedGroup { get => GetValue(SelectedGroupProperty); set => SetValue(SelectedGroupProperty, value); }
        public object SelectedDevice { get => GetValue(SelectedDeviceProperty); set => SetValue(SelectedDeviceProperty, value); }
        public DataTemplate GroupItemTemplate { get => (DataTemplate)GetValue(GroupItemTemplateProperty); set => SetValue(GroupItemTemplateProperty, value); }
        public DataTemplate DeviceItemTemplate { get => (DataTemplate)GetValue(DeviceItemTemplateProperty); set => SetValue(DeviceItemTemplateProperty, value); }
        public ICommand DetailCommand { get => (ICommand)GetValue(DetailCommandProperty); set => SetValue(DetailCommandProperty, value); }
        public CardShape CardShape { get => (CardShape)GetValue(CardShapeProperty); set => SetValue(CardShapeProperty, value); }
        public string ListHeader { get => (string)GetValue(ListHeaderProperty); set => SetValue(ListHeaderProperty, value); }
        public string CardsHeader { get => (string)GetValue(CardsHeaderProperty); set => SetValue(CardsHeaderProperty, value); }
        public string EmptyGroupsText { get => (string)GetValue(EmptyGroupsTextProperty); set => SetValue(EmptyGroupsTextProperty, value); }
        public string EmptyDevicesText { get => (string)GetValue(EmptyDevicesTextProperty); set => SetValue(EmptyDevicesTextProperty, value); }
        public double ListWidth { get => (double)GetValue(ListWidthProperty); set => SetValue(ListWidthProperty, value); }
        public double CardWidth { get => (double)GetValue(CardWidthProperty); set => SetValue(CardWidthProperty, value); }
        private static void OnGroupChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            // A selection from the old equipment must not leak into the new equipment.
            sender.SetCurrentValue(SelectedDeviceProperty, null);
        }
        private static bool IsPositiveFinite(object value)
        {
            var number = (double)value;
            return number > 0 && !double.IsNaN(number) && !double.IsInfinity(number);
        }
    }
}
