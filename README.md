# 工业设备列表与卡片控件 · WPF / .NET Framework 4.8

提取原预充界面的“左侧托盘列表 → 右侧电芯卡片 → 卡片内测量数据”展示结构，改为通用的“分组列表 → 设备对象 → 测量数据项”。保留浅色卡片、圆角、电池轮廓、状态颜色和详情入口，并提供普通面板外观。

**交付状态：源码初版 0.1.0。当前制作环境是 Linux，无 Windows/WPF/MSBuild，未进行 Windows 编译、BAML 编译或实际界面运行。已做的静态检查见 `VALIDATION.md`。请在 Windows 运行 `verify.ps1` 后再接入正式项目。**

## 目录

| 位置 | 用途 |
| --- | --- |
| `Industrial.UI.Wpf.sln` | 可用 Visual Studio 打开的解决方案 |
| `src/Industrial.UI.Wpf` | 独立 net48 WPF 类库 |
| `samples/Industrial.UI.Wpf.Demo` | 托盘/电芯与机柜/传感器两种演示；全部为虚构数据 |
| `tests/Industrial.UI.Wpf.Smoke` | Windows STA 界面与绑定验证程序 |
| `verify.ps1` | Windows 编译、运行验证、生成实际控件截图 |
| `docs/接入说明.md` | 数据绑定、自有模型、主题、迁移说明 |
| `Industrial.UI.Wpf.nuspec` | 编译通过后生成内部 NuGet 包 |

## 快速运行

准备 Windows、Visual Studio 2019/2022 的“.NET 桌面开发”和 **.NET Framework 4.8 Developer/Targeting Pack**。只有运行时不够。工程使用传统 `.csproj`，C# 7.3；不需要固定 .NET SDK，不需要 .NET 8，也没有 NuGet 运行依赖。

1. 解压后打开 `Industrial.UI.Wpf.sln`。
2. 将 `Industrial.UI.Wpf.Demo` 设为启动项目，按 F5。
3. 或在本目录用 Windows PowerShell 执行：

```powershell
.\verify.ps1
```

脚本会重建三个工程，启动 WPF 验证窗口，并在 `artifacts` 写入编译日志、验证结果、真实 WPF 截图。运行时需要正常桌面会话。脚本失败会返回错误，不会把缺少 MSBuild 或跳过执行当作通过。

只编译、不运行界面验证：`.\verify.ps1 -BuildOnly`。

## 控件组成

| 控件 | 可以单独使用 | 用途 |
| --- | --- | --- |
| `DeviceBoard` | 是 | 左列表、右卡片及选择联动布局 |
| `DeviceGrid` | 是 | 卡片排列、键盘/鼠标选择、自定义 ItemTemplate |
| `DeviceCard` | 是 | 标题、状态、任意数量数据项、详情命令；Panel / Battery 两种外观 |
| `StatusBadge` | 是 | 同时显示状态文字与颜色，区分未知、正常、运行、等待、预警、报警、离线 |

`EquipmentGroup / DeviceItem / MeasurementItem` 是可选的展示模型。也可以保留自己的业务对象，通过 DataTemplate 映射到控件属性。

## 最小接入

引用类库工程、编译后的 DLL 或内部 NuGet 包，在页面声明：

```xml
xmlns:ui="clr-namespace:Industrial.UI.Wpf.Controls;assembly=Industrial.UI.Wpf"
```

```xml
<ui:DeviceBoard
    GroupsSource="{Binding Groups}"
    SelectedGroup="{Binding SelectedGroup}"
    DevicesSource="{Binding SelectedGroup.Items}"
    SelectedDevice="{Binding SelectedDevice}"
    DetailCommand="{Binding ShowDetailsCommand}"
    ListHeader="设备列表"
    CardsHeader="设备数据"
    CardShape="Panel" />
```

调用方 ViewModel 的选择属性应实现 `INotifyPropertyChanged`，列表增删使用 `ObservableCollection`。`DevicesSource` 显式绑定到当前选择的集合，便于接入不同业务层级；控件不猜测设备归属、不默认选中第一项。切换分组会清除旧设备选择。

`ShowDetailsCommand` 收到原始卡片对象，调用方决定是否打开窗口、导航或查看历史。控件不会自行操作设备或弹出业务详情窗口。命令为 null 时隐藏详情入口，`CanExecute` 控制按钮可用性。

## 分发给其他项目

最简单方式是引用 `Industrial.UI.Wpf.dll`；主题 BAML 随 DLL 嵌入，无需复制源项目的 Models、Common、BaseStyle、PLC 库或字体图标。

Windows Release 编译并验证后，如已有 NuGet CLI：

```powershell
nuget pack .\Industrial.UI.Wpf.nuspec -BasePath . -OutputDirectory .\artifacts\packages
```

上传到团队的私有 NuGet 源，其他 WPF net48 项目安装 `Industrial.UI.Wpf` 即可。尚未实际生成或发布 `.nupkg`，此压缩包也不包含未经编译验证的 DLL。

## 当前边界

- 仅支持 Windows WPF / .NET Framework 4.8；不是 Avalonia、WinForms、Vue 或跨平台 UI 包。
- 类库只展示外部数据，不包含 PLC 通讯、数据库、轮询定时器、报警阈值、自动数据或设备控制逻辑。
- 原界面密集的电池小卡片已调整为可读的数据卡片；保留视觉语言和联动关系，并非像素级复刻。
- 卡片区域使用 WrapPanel，未实现虚拟化。大量对象与高频刷新需要在真实机器测量，并考虑分页或虚拟化面板；本版不承诺大规模性能。
- 左侧宽度、卡片宽度可配置，卡片按可用空间换行；很窄的容器需要宿主切换布局或另写模板。
- 所有绑定集合与属性变更必须由宿主切回 UI Dispatcher。控件不偷偷创建线程。
- 本版不附加开源许可证；分发权限沿用原代码权属及团队内部约定。未复制原项目第三方 DLL、图标字体和现场配置。

来源与迁移对应见 `docs/接入说明.md`。
