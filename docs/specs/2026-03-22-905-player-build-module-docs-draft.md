# Task SPEC: UnityPerfLab MVP 04 Player Autorun, Build, And Module Docs

Keep this document narrow. Reference the parent MVP spec when needed instead of restating it.

## Goal
补齐 UnityPerfLab MVP 的真实验证路径：提供最小 Bootstrap Scene 自动运行流程、Win64 IL2CPP Release 脚本化构建入口、Player 结果输出能力，以及模块 README 文档。

## In Scope
- 实现 Bootstrap Scene 对应的 runtime 启动逻辑
- Player 启动后自动：
  - 解析最小命令行参数
  - 选择 suite
  - 运行 benchmark
  - 导出结果
  - 输出日志
  - 在独立平台上完成后退出
- 支持最小命令行参数：
  - `--suite=<name>`
  - `--outputDir=<path>`
- 提供 Win64 IL2CPP Release 脚本化构建入口
- 提供对应 Editor 菜单：
  - `UnityPerfLab/Build/Win64 IL2CPP Release`
- 规定明确且可预测的构建输出目录
- 编写 `Assets/UnityPerfLab/README.md`，覆盖：
  - 模块目的
  - 目录结构
  - asmdef 边界
  - 如何新增 Synthetic case
  - 如何新增 RealWorld adapter / case
  - 如何从 Editor 运行
  - 如何构建 Win64 IL2CPP Release
  - 结果输出位置
  - 已知限制

## Out of Scope
- 不实现多平台构建矩阵
- 不实现 Development Build 全矩阵
- 不实现复杂 CLI 配置系统
- 不实现 charting、baseline、ratio 报告
- 不接入真实 RealWorld benchmark case

## Affected Area
- `Assets/UnityPerfLab/Runtime/Bootstrap/*`
- `Assets/UnityPerfLab/Scenes/*`
- `Assets/UnityPerfLab/Editor/Build/*`
- `Assets/UnityPerfLab/README.md`

## Done When
- Win64 IL2CPP Release 可通过 Editor 菜单或 batchmode 静态方法触发构建
- 构建产物路径固定、可预期
- 生成的 Player 启动后自动执行 benchmark 并写出结果
- Player 在完成后退出
- README 足以支持后续继续扩展 Synthetic case 和 RealWorld adapter

## Validation

### Black-box Checks
- 在 Unity 中执行 `UnityPerfLab/Build/Win64 IL2CPP Release`
- 确认输出目录下生成 `UnityPerfLab.exe`
- 启动 Player，确认自动运行并写出：
  - `summary.csv`
  - `raw_samples.csv`
  - `metadata.json`
- 检查 Player 日志包含结果输出目录
- 用 `--suite=synthetic` 与 `--outputDir=<path>` 进行至少一次启动验证
- 检查 README 是否覆盖运行、构建、输出和扩展说明

### White-box Needed
No

### White-box Trigger
该任务的主要验收点是 Player 端外部行为、构建入口是否可用，以及 README 是否足够支持使用者，黑盒验证优先。

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

If yes, what stable information should be written back, and where does it belong?

- `Assets/UnityPerfLab/README.md` 作为模块级稳定说明必须更新
- 若仓库长期以 Unity 项目为主，应保持 `docs/facts/project-scope.md` 与当前角色一致

## Risks / Notes
- Release + IL2CPP 才是主要性能验证路径，Editor 运行只能作为功能检查
- `Application.Quit` 在不同平台与不同启动方式下行为略有差异，独立平台验证必不可少
- 构建脚本需要保持简单直接，避免在 MVP 阶段引入太多 build profile 或平台分支
