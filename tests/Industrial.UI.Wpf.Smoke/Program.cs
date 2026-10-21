using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Industrial.UI.Wpf.Controls;
using Industrial.UI.Wpf.Models;

namespace Industrial.UI.Wpf.Smoke
{
    internal static class Program
    {
        private static int _checks;
        [STAThread]
        private static int Main(string[] args)
        {
            var output = Path.GetFullPath(args.Length > 0 ? args[0] : "artifacts");
            Directory.CreateDirectory(output);
            var listener = new BindingErrors();
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Error;
            PresentationTraceSources.DataBindingSource.Listeners.Add(listener);
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
            Window window = null;
            try
            {
                var vm = new TestViewModel();
                var board = new DeviceBoard { DataContext = vm, Margin = new Thickness(16), ListHeader = "托盘列表", CardsHeader = "设备内的数据", CardShape = CardShape.Battery };
                board.SetBinding(DeviceBoard.GroupsSourceProperty, new Binding(nameof(vm.Groups)));
                board.SetBinding(DeviceBoard.SelectedGroupProperty, new Binding(nameof(vm.SelectedGroup)));
                board.SetBinding(DeviceBoard.DevicesSourceProperty, new Binding("SelectedGroup.Items"));
                board.SetBinding(DeviceBoard.SelectedDeviceProperty, new Binding(nameof(vm.SelectedDevice)));
                board.SetBinding(DeviceBoard.DetailCommandProperty, new Binding(nameof(vm.DetailCommand)));
                window = new Window { Title = "工业 UI 控件验证 · 全部为演示数据", Content = board, Width = 1200, Height = 780 };
                window.Show(); Pump();
                Check(board.Template != null, "theme auto-discovery");
                Check(ReferenceEquals(board.DataContext, vm), "control preserves caller DataContext");
                var groups = (ListBox)board.Template.FindName("PART_Groups", board);
                var devices = (DeviceGrid)board.Template.FindName("PART_Devices", board);
                Check(groups.Items.Count == 2 && devices.Items.Count == 8, "group/device data binding");
                Check(Find<DeviceCard>(board).Count() == 8, "device templates materialized");
                devices.SelectedItem = vm.Groups[0].Items[0]; Pump();
                Check(ReferenceEquals(vm.SelectedDevice, vm.Groups[0].Items[0]), "device selection two-way binding");
                groups.SelectedItem = vm.Groups[1]; Pump();
                Check(ReferenceEquals(vm.SelectedGroup, vm.Groups[1]) && devices.Items.Count == 1, "group selection changes card source");
                Check(vm.SelectedDevice == null, "old device selection cleared when group changes");
                groups.SelectedItem = vm.Groups[0]; Pump();
                var first = vm.Groups[0].Items[0];
                first.Measurements[0].Value = "37.8"; Pump();
                Check(Find<TextBlock>(board).Any(t => t.Text == "37.8"), "measurement notification updates visible text");
                first.Status = DeviceStatus.Alarm; Pump();
                Check(Find<StatusBadge>(board).Any(b => b.Status == DeviceStatus.Alarm), "status update reaches template");
                first.Measurements[0].Value = null; Pump();
                Check(Find<TextBlock>(board).Any(t => t.Text == "—"), "missing data is not displayed as zero");
                var card = Find<DeviceCard>(board).First(c => ReferenceEquals(c.DataContext, first));
                var button = (Button)card.Template.FindName("DetailButton", card);
                Check(ReferenceEquals(button.CommandParameter, first), "detail parameter is the original data item");
                Check(button.Command != null && button.Command.CanExecute(button.CommandParameter), "detail command bound");
                button.Command.Execute(button.CommandParameter);
                Check(ReferenceEquals(vm.LastDetail, first), "detail execution delegated to caller");
                board.DetailCommand = null; Pump();
                Check(Find<Button>(devices).Where(b => b.Name == "DetailButton").All(b => b.Visibility == Visibility.Collapsed), "no detail command hides entry");
                board.SetBinding(DeviceBoard.DetailCommandProperty, new Binding(nameof(vm.DetailCommand))); Pump();
                SaveImage(board, Path.Combine(output, "battery-board.png"));

                app.Resources["Industrial.NormalSoft"] = new SolidColorBrush(Color.FromRgb(220, 230, 250));
                first.Status = DeviceStatus.Normal; Pump();
                Check(((SolidColorBrush)card.Background).Color == Color.FromRgb(220, 230, 250), "application theme override");
                app.Resources.Remove("Industrial.NormalSoft");

                // An arbitrary domain type works with an external DataTemplate; DeviceItem is optional.
                board.DeviceItemTemplate = (DataTemplate)XamlReader.Parse(
                    "<DataTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
                    "xmlns:ui='clr-namespace:Industrial.UI.Wpf.Controls;assembly=Industrial.UI.Wpf'>" +
                    "<ui:DeviceCard Title='{Binding Caption}' MetricsSource='{Binding Readings}' Status='Normal'/></DataTemplate>");
                board.CardsHeader = "机柜 / 自定义业务对象";
                board.DevicesSource = new[] { new CustomDevice() }; Pump();
                Check(Find<TextBlock>(board).Any(t => t.Text == "温湿度传感器"), "custom business type and item template");
                SaveImage(board, Path.Combine(output, "generic-device-board.png"));
                board.DevicesSource = new object[0]; Pump();
                Check(((TextBlock)board.Template.FindName("EmptyDevices", board)).Visibility == Visibility.Visible, "empty device state");
                board.GroupsSource = new object[0]; Pump();
                Check(((TextBlock)board.Template.FindName("EmptyGroups", board)).Visibility == Visibility.Visible, "empty group state");
                Check(listener.Errors.Count == 0, "no WPF data-binding errors: " + string.Join(" | ", listener.Errors));
                File.WriteAllText(Path.Combine(output, "windows-verification.txt"), "PASS " + _checks + " checks; Windows=" + Environment.OSVersion + "; CLR=" + Environment.Version + "; UTC=" + DateTime.UtcNow.ToString("O"));
                Console.WriteLine("PASS " + _checks + " checks. Images: " + output);
                return 0;
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                File.WriteAllText(Path.Combine(output, "windows-verification.txt"), "FAIL\r\n" + error);
                return 1;
            }
            finally
            {
                window?.Close();
                app.Shutdown();
                PresentationTraceSources.DataBindingSource.Listeners.Remove(listener);
            }
        }

        private static void Check(bool passed, string message)
        {
            if (!passed) throw new InvalidOperationException("FAILED: " + message);
            _checks++;
            Console.WriteLine("PASS: " + message);
        }
        private static void Pump()
        {
            var frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => frame.Continue = false));
            Dispatcher.PushFrame(frame);
        }
        private static IEnumerable<T> Find<T>(DependencyObject root) where T : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is T) yield return (T)child;
                foreach (var descendant in Find<T>(child)) yield return descendant;
            }
        }
        private static void SaveImage(FrameworkElement element, string path)
        {
            element.UpdateLayout();
            var image = new RenderTargetBitmap((int)Math.Ceiling(element.ActualWidth), (int)Math.Ceiling(element.ActualHeight), 96, 96, PixelFormats.Pbgra32);
            image.Render(element);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using (var stream = File.Create(path)) encoder.Save(stream);
        }
        private sealed class BindingErrors : TraceListener
        {
            public List<string> Errors { get; } = new List<string>();
            public override void Write(string message) { if (!string.IsNullOrEmpty(message)) Errors.Add(message); }
            public override void WriteLine(string message) { Write(message); }
        }
    }

    public sealed class TestViewModel : ObservableObject
    {
        private EquipmentGroup _selectedGroup;
        private DeviceItem _selectedDevice;
        public ObservableCollection<EquipmentGroup> Groups { get; } = new ObservableCollection<EquipmentGroup>();
        public EquipmentGroup SelectedGroup { get => _selectedGroup; set => Set(ref _selectedGroup, value); }
        public DeviceItem SelectedDevice { get => _selectedDevice; set => Set(ref _selectedDevice, value); }
        public DeviceItem LastDetail { get; private set; }
        public ICommand DetailCommand { get; }
        public TestViewModel()
        {
            DetailCommand = new TestCommand(value => LastDetail = (DeviceItem)value);
            var first = new EquipmentGroup { Title = "托盘 01", Subtitle = "演示库位 A-01", Status = DeviceStatus.Running };
            for (var i = 0; i < 8; i++)
            {
                var item = new DeviceItem { Title = "#" + (i + 1).ToString("000"), Subtitle = "电芯测点", Status = (DeviceStatus)(i % 7) };
                item.Measurements.Add(new MeasurementItem { Label = "温度", Value = i == 6 ? null : "30.2", Unit = "℃" });
                item.Measurements.Add(new MeasurementItem { Label = "负压", Value = i == 6 ? null : "-45.6", Unit = "kPa" });
                first.Items.Add(item);
            }
            var second = new EquipmentGroup { Title = "托盘 02", Subtitle = "演示库位 A-02", Status = DeviceStatus.Normal };
            second.Items.Add(new DeviceItem { Title = "第二组对象", Status = DeviceStatus.Normal });
            Groups.Add(first); Groups.Add(second); SelectedGroup = first;
        }
    }
    public sealed class CustomDevice
    {
        public string Caption => "温湿度传感器";
        public ObservableCollection<MeasurementItem> Readings { get; } = new ObservableCollection<MeasurementItem>
        {
            new MeasurementItem { Label = "温度", Value = "25.4", Unit = "℃" },
            new MeasurementItem { Label = "湿度", Value = "48.2", Unit = "%RH" }
        };
    }
    internal sealed class TestCommand : ICommand
    {
        private readonly Action<object> _action;
        public TestCommand(Action<object> action) { _action = action; }
        public bool CanExecute(object parameter) => parameter is DeviceItem;
        public void Execute(object parameter) => _action(parameter);
        public event EventHandler CanExecuteChanged { add { } remove { } }
    }
}
