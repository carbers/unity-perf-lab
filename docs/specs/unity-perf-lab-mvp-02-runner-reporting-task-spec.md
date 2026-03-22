# Task SPEC: UnityPerfLab MVP 02 Runner And Reporting Core

Keep this document narrow. Reference the parent MVP spec when needed instead of restating it.

## Goal
实现 UnityPerfLab 的核心 benchmark 执行与导出闭环，包括统一 case 抽象、生命周期执行、两种测量模式、结果统计、结果路径解析，以及 CSV-first 导出能力。

## In Scope
- 定义并实现核心运行时抽象：
  - `IPerfCase`
  - `PerfCaseDescriptor`
  - `PerfMeasurementConfig`
  - `PerfMeasurementMode`
  - `PerfRunRequest`
  - `PerfSample`
  - `PerfCaseRunResult`
  - `PerfRunResult`
  - `PerfRunner`
  - `PerfVisibleSink`
- 支持 benchmark 生命周期：
  - `GlobalSetup`
  - `Setup`
  - `Run`
  - `Teardown`
  - `GlobalTeardown`
- 仅测量 `Run(iterations)` 主体
- 使用 `Stopwatch` 计时，并归一化到 `ns/op`
- 支持两种测量模式：
  - `FixedIterations`
  - 简化版 `TargetDuration`
- 实现统计汇总：
  - `mean`
  - `median`
  - `min`
  - `max`
  - `stddev`
  - `p95`
  - `ops/sec`
- 实现结果导出接口与 CSV exporter：
  - `summary.csv`
  - `raw_samples.csv`
  - 可选 `metadata.json`
- 实现环境信息采集和默认结果路径解析：
  - Editor 默认输出到项目根 `PerfLabResults/<run_id>/`
  - Player 默认输出到 `Application.persistentDataPath/PerfLabResults/<run_id>/`

## Out of Scope
- 不实现具体 Synthetic case 家族
- 不实现 Editor 菜单
- 不实现 Player autorun 行为
- 不实现 Win64 IL2CPP 构建入口
- 不接入 RealWorld case

## Affected Area
- `Assets/UnityPerfLab/Runtime/Core/*`
- `Assets/UnityPerfLab/Runtime/Reporting/*`
- `Assets/UnityPerfLab/Runtime/Environment/*`

## Done When
- 统一 runner 可以接受已注册 case 列表并执行
- setup/teardown 不进入测量窗口
- `FixedIterations` 和 `TargetDuration` 都可用
- 每个 measured sample 都能得到 `elapsed_ns` 和 `ns_per_op`
- `summary.csv` 和 `raw_samples.csv` 字段齐全且格式稳定
- 结果目录规则明确且稳定

## Validation

### Black-box Checks
- 用至少一个 smoke case 跑通 runner
- 确认导出目录中包含 `summary.csv` 和 `raw_samples.csv`
- 检查 measured sample 的 `ns_per_op` 非零
- 检查 warmup sample 和 measured sample 均被记录
- Spot check `summary.csv` 与 `raw_samples.csv` 的样本数、迭代数和 case 名称一致

### White-box Needed
Yes

### White-box Trigger
该任务的核心价值在内部执行正确性。尤其是测量窗口边界、`TargetDuration` 的 probe/clamp 分支、以及 summary 统计推导，都属于回归敏感逻辑。

### Internal Logic To Protect
- `TargetDuration` 的 probe 次数、估算迭代数与 clamp 行为
- `GlobalSetup` / `Setup` / `Teardown` / `GlobalTeardown` 不进入测量时间
- `PerfVisibleSink` 确保 workload 可观察
- `summary.csv` 的统计值来源于 measured raw samples，而非 warmup 或其他临时值
- Editor 与 Player 的结果路径分流规则

## Write-back Needed
No

If yes, what stable information should be written back, and where does it belong?

N/A

## Risks / Notes
- 极快 workload 容易让 sample 太短，`TargetDuration` 的最小迭代 clamp 需要保守
- `Stopwatch` 精度与平台相关，解释结果时需要将 Editor 和 Player 区分开
- 统计字段一旦导出给 CSV，应尽量保持字段名稳定，避免后续长期追踪时破坏兼容性
