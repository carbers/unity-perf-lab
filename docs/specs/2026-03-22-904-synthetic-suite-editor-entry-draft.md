# Task SPEC: UnityPerfLab MVP 03 Synthetic Suite And Editor Entry

Keep this document narrow. Reference the parent MVP spec when needed instead of restating it.

## Goal
基于已有 runner 和 exporter，交付第一批 Synthetic benchmark case，并提供最小 Editor 入口，使 UnityPerfLab 可以在 Editor 内直接执行 Synthetic suite 并产出结构化结果。

## In Scope
- 实现 Synthetic case catalog / 显式注册机制
- 至少实现以下 Synthetic case 家族：
  - `int[]` foreach vs for
  - `List<int>` foreach vs for
  - `List<struct>` foreach vs for
  - direct call vs interface call
  - closure capture vs no-capture
- 每类 case 至少覆盖三档 workload size：
  - `1K`
  - `100K`
  - `1M`
- 保证 case 命名包含语义和 size，例如：
  - `Array_Int_ForEach_1K`
  - `List_Struct_For_100K`
  - `InterfaceCall_1M`
- 所有 workload 在 `Run(iterations)` 内写入 `PerfVisibleSink`
- 提供 Editor 运行入口：
  - `UnityPerfLab/Run Synthetic Suite`
  - `UnityPerfLab/Run All Available Cases`
- `All Available Cases` 当前允许只包含 Synthetic cases + 空的 RealWorld catalog

## Out of Scope
- 不实现 Win64 IL2CPP 构建入口
- 不实现 Player 启动自动运行
- 不接入真实业务/runtime 实现
- 不实现复杂命令行过滤器、case 选择器或自定义编辑器窗口

## Affected Area
- `Assets/UnityPerfLab/Runtime/Cases/Synthetic/*`
- `Assets/UnityPerfLab/Runtime/Cases/RealWorld/*`
- `Assets/UnityPerfLab/Runtime/Adapters/*`
- `Assets/UnityPerfLab/Runtime/Bootstrap/*`
- `Assets/UnityPerfLab/Editor/Runner/*`

## Done When
- Synthetic suite 已通过显式 catalog 注册
- Editor 菜单可触发 suite 运行
- 导出的 CSV 中包含所有预期 case 变体
- case 命名、分类、参数字段可读且稳定
- RealWorld catalog 和 adapter contract 已预留，但不强行接入虚构实现

## Validation

### Black-box Checks
- 在 Unity Editor 中执行 `UnityPerfLab/Run Synthetic Suite`
- 确认导出目录中存在预期数量的 Synthetic case 结果行
- Spot check 至少每个 case 家族出现一次，并覆盖三档 size
- 检查 `case_name` 和 `parameters` 字段能反映 workload 语义
- 执行 `UnityPerfLab/Run All Available Cases`，确认当前行为稳定且不会因空 RealWorld catalog 报错

### White-box Needed
No

### White-box Trigger
本任务以 case 覆盖面、命名稳定性和 Editor 触达能力为主，主要通过黑盒运行结果即可判断完成度。

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

If yes, what stable information should be written back, and where does it belong?

- Synthetic case 的扩展方式、注册方式、命名约定应写入 `Assets/UnityPerfLab/README.md`

## Risks / Notes
- `Closure_Capture_*` case 代表的是 capture 模式成本，不应和“普通委托调用成本”混为一谈
- `1M` workload 可能让部分 case 在 Editor 下较慢，因此测量参数需要按 workload tier 做区分
- 不要在本任务里引入 reflection-based discovery；catalog 显式注册更稳定、更易审查
