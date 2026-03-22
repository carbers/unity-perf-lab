# Task SPEC: UnityPerfLab MVP

Keep this document narrow. Reference the plan when needed instead of restating it.

## Goal
在仓库根目录内建立一个可运行的 Unity `2022.3 LTS` 项目，并交付一个隔离的 `UnityPerfLab` 基准测试模块。该模块必须形成一个闭环 MVP，用于 Unity / IL2CPP 微基准实验，支持统一 case 抽象、Synthetic benchmark 套件、CSV 导出、Editor 触发，以及 Win64 IL2CPP Release Player 自动运行路径。

## In Scope
- 将当前仓库根目录转为 Unity 项目根目录
- 创建 `Assets/UnityPerfLab` 模块目录与 asmdef 分层
- 建立统一 benchmark 抽象与执行路径：
  - `IPerfCase`
  - `PerfCaseDescriptor`
  - `PerfRunRequest`
  - `PerfSample`
  - `PerfRunResult`
  - `PerfRunner`
  - `IResultExporter`
- 支持两种测量模式：
  - `FixedIterations`
  - 简化版 `TargetDuration`
- 严格保证仅测量 `Run(iterations)` 主体，不将 `GlobalSetup` / `Setup` / `Teardown` / `GlobalTeardown` 计入测量
- 通过可见 sink 保持 workload 可观察，避免 benchmark 退化为空循环
- 输出结构化结果文件：
  - `summary.csv`
  - `raw_samples.csv`
  - `metadata.json`
- 提供首批 Synthetic case，至少覆盖：
  - `int[]` foreach vs for
  - `List<int>` foreach vs for
  - `List<struct>` foreach vs for
  - direct call vs interface call
  - closure capture vs no-capture
- 每类 Synthetic case 至少覆盖显式 workload size：
  - `1K`
  - `100K`
  - `1M`
- 提供 Editor 菜单入口：
  - `UnityPerfLab/Run Synthetic Suite`
  - `UnityPerfLab/Run All Available Cases`
  - `UnityPerfLab/Build/Win64 IL2CPP Release`
- 提供最小 Bootstrap Scene，使 Player 启动后自动运行 UnityPerfLab 并写出结果
- 提供 Win64 IL2CPP Release 脚本化构建入口
- 提供模块内 README，说明结构、运行方式、构建方式、输出位置和扩展方式
- 为未来 RealWorld case 预留 Adapter / Catalog 骨架，但不要求本轮接入真实业务实现

## Out of Scope
- 不实现复杂的 reflection-based case 自动发现框架
- 不实现大型插件化平台或扩展系统
- 不实现图表、ratio/baseline 报表、跨版本批处理矩阵
- 不实现多平台构建矩阵
- 不要求接入真实业务/runtime 模块作为 RealWorld case
- 不将 benchmark 模块反向耦合进未来业务代码
- 不为理论扩展性引入额外抽象层

## Affected Area
- Unity 项目基础结构：
  - `Packages/`
  - `ProjectSettings/`
  - `Assets/`
- `Assets/UnityPerfLab/Runtime/*`
- `Assets/UnityPerfLab/Editor/*`
- `Assets/UnityPerfLab/Scenes/*`
- `Assets/UnityPerfLab/README.md`
- 必要时更新根级 `.gitignore`
- 仅在稳定范围变化时更新 `docs/facts/project-scope.md`

## Done When
- 仓库可被 Unity `2022.3.x` 作为项目打开
- `UnityPerfLab` 相关 asmdef 编译边界清晰，Editor 与 Runtime 无反向依赖
- 可通过统一 runner 执行已注册的 Synthetic cases
- 所有测量结果归一化到 `ns/op`
- `summary.csv` 至少包含：
  - `run_id`
  - `timestamp`
  - `unity_version`
  - `platform`
  - `build_config`
  - `case_name`
  - `category`
  - `parameters`
  - `measurement_mode`
  - `warmup_count`
  - `sample_count`
  - `total_iterations`
  - `target_duration_ms`
  - `mean_ns`
  - `median_ns`
  - `min_ns`
  - `max_ns`
- `raw_samples.csv` 至少包含：
  - `run_id`
  - `case_name`
  - `sample_index`
  - `is_warmup`
  - `iterations`
  - `elapsed_ns`
  - `ns_per_op`
- Editor 菜单可以跑通 Synthetic suite 并导出结果
- Win64 IL2CPP Release 构建入口可生成 Player
- Player 启动后自动执行 Bootstrap flow，导出结果，并在独立平台上完成后退出
- README 能说明：
  - UnityPerfLab 的用途
  - 目录结构
  - 如何新增 Synthetic case
  - 如何新增 RealWorld adapter / case
  - 如何从 Editor 运行
  - 如何构建 Win64 IL2CPP Release Player
  - 结果输出位置
  - 已知限制

## Validation

### Black-box Checks
- 用 Unity 打开项目，确认工程可导入且 asmdef 无编译错误
- 在 Editor 中触发 `UnityPerfLab/Run Synthetic Suite`，确认生成新的 run 目录
- 确认 run 目录包含：
  - `summary.csv`
  - `raw_samples.csv`
  - `metadata.json`
- 检查 `summary.csv` 中每个 case 都有非零 `mean_ns` / `median_ns` / `min_ns` / `max_ns`
- 检查 `raw_samples.csv` 中 warmup 与 measured sample 均被记录
- 通过 `UnityPerfLab/Build/Win64 IL2CPP Release` 生成 Win64 Player
- 启动生成出的 Player，确认会自动跑 benchmark、写出结果并退出
- Spot check 至少一个 case 名称，确认包含 workload 语义与 size，例如：
  - `Array_Int_ForEach_1K`
  - `InterfaceCall_1M`

### White-box Needed
Yes

### White-box Trigger
Runner 与统计逻辑属于分支明确、回归敏感的内部逻辑。仅靠黑盒检查很难长期保证：
- `TargetDuration` 的 probe/clamp 逻辑不回退
- setup/teardown 不会误入测量窗口
- 汇总统计不会和 raw sample 脱节

### Internal Logic To Protect
- `TargetDuration` 模式下的 probe 迭代估算与 `min_iterations` / `max_iterations` clamp
- 仅 `Run(iterations)` 被 `Stopwatch` 包裹测量
- warmup sample 与 measured sample 的分离
- `summary.csv` 中 mean / median / min / max / p95 / stddev / ops/sec 由 raw sample 正确推导
- Result exporter 的路径选择规则：
  - Editor 默认写到项目根 `PerfLabResults/`
  - Player 默认写到 `Application.persistentDataPath/PerfLabResults/`

## Write-back Needed
Yes

If yes, what stable information should be written back, and where does it belong?

- 若本任务首次将仓库根目录转为 Unity 项目，应更新 `docs/facts/project-scope.md`
- `UnityPerfLab` 的使用说明、目录约定、扩展方式应写入 `Assets/UnityPerfLab/README.md`
- 不把 task-local 推理过程、调参细节、一次性调试记录写入 facts

## Risks / Notes
- `ProjectVersion.txt` 需要与本机实际安装的 Unity `2022.3.x` patch 保持一致，否则导入时可能出现版本摩擦
- 手写 `.unity` / `.asset` YAML 在未经过 Unity 重新序列化前存在格式风险，首次导入后应由 Unity 重新保存校正
- IL2CPP Release 才是主要验证路径，Editor 结果不能替代最终结论
- 极快 workload 仍可能需要继续调节 `TargetDuration` 的 clamp 参数，避免 sample 过短
- `Closure_Capture_*` case 会刻意包含额外分配与捕获成本，解释结果时应注意其语义是“模式对比”，不是通用函数调用成本
- RealWorld 目录与 adapter 仅为本轮扩展点，若未来接入真实实现，必须保持单向依赖：业务/runtime 代码不能依赖 `UnityPerfLab`
