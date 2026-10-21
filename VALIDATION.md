# 验证记录

本文件区分已经执行的检查和交付的待运行验证，不把源码检查当作编译通过。

## 已执行：Linux 静态检查

- 7 份 XML 文件（3 个 XAML、3 个 csproj、1 个 nuspec）均可解析。
- 三个工程的目标框架均为 `v4.8`，语言版本为 C# 7.3。
- 每个项目的 Compile / Page / ApplicationDefinition 输入和 ProjectReference 路径存在，全部源码均已明确纳入项目。
- 类库未引用原项目的 Common、Models、GlobalManager、PLC 访问、业务 ViewModel 或轮询定时器。
- 类库没有 PackageReference、DLL HintPath 或第三方运行依赖。
- 主题中引用的 24 个静态/动态资源键均有定义。
- 含 x:Class 的 XAML 均有对应的 partial code-behind。
- 显式提供未知、正常、运行、等待、预警、报警、离线七种状态。

详细机器可读结果见 `static-check-results.json`。这些检查不验证 WPF 的 BAML 编译、绑定运行时行为或像素效果。

## 未执行：Windows 构建与 WPF 实际运行

制作环境没有 .NET、MSBuild、Windows 桌面及 WPF 运行时，因此以下均未执行：

- C# 编译与引用解析；
- XAML / BAML 编译及主题实际加载；
- Windows UI 行为验证；
- 原生 WPF 截图和视觉验收；
- NuGet pack；
- 接入原项目、真实 PLC、实际刷新负载和生产验证。

## 随包提供的 Windows 验证

执行 `verify.ps1` 会先构建整个解决方案，然后运行 STA 桌面验证程序。程序检查主题加载、外部 DataContext 保留、分组/设备绑定、双向选择、切换清除旧选择、测量值通知、状态通知、空值显示、详情参数和命令绑定、无命令入口隐藏、应用主题覆盖、自定义业务模板、空列表状态，并监听 WPF 绑定错误。

程序仅在全部检查成功后写入 PASS；失败会返回非零退出码。它还生成 `battery-board.png` 和 `generic-device-board.png`，作为 Windows 真实控件渲染输出。截图的美观性、高 DPI 和现场可读性仍需要人工确认。

无需为了交付文件而再次执行已经完成的静态检查；只有新增改动或检查失败才重验相关部分。
