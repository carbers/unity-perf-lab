# Task SPEC: UnityPerfLab MVP 01 Bootstrap And Boundaries

Keep this document narrow. Reference the parent MVP spec when needed instead of restating it.

## Goal
在仓库根目录建立最小可用的 Unity `2022.3 LTS` 项目骨架，并创建隔离的 `UnityPerfLab` 模块目录、asmdef 边界和最小 Bootstrap Scene，为后续 benchmark runtime 和 case 实现提供清晰落点。

## In Scope
- 将仓库根目录组织为 Unity 项目根目录
- 创建最小项目结构：
  - `Assets/`
  - `Packages/manifest.json`
  - `ProjectSettings/ProjectVersion.txt`
  - `ProjectSettings/EditorBuildSettings.asset`
- 创建 `Assets/UnityPerfLab/` 目录结构：
  - `Runtime/Core`
  - `Runtime/Cases/Synthetic`
  - `Runtime/Cases/RealWorld`
  - `Runtime/Adapters`
  - `Runtime/Reporting`
  - `Runtime/Environment`
  - `Runtime/Bootstrap`
  - `Editor/Build`
  - `Editor/Runner`
  - `Scenes`
  - `Config`
- 创建 asmdef 分层：
  - `UnityPerfLab.Runtime`
  - `UnityPerfLab.Cases.Synthetic`
  - `UnityPerfLab.Cases.RealWorld`
  - `UnityPerfLab.Bootstrap`
  - `UnityPerfLab.Editor`
- 确保 build settings 包含 `UnityPerfLabBootstrap` scene
- 视需要更新 `.gitignore`，排除 Unity 常见生成目录与 benchmark 结果目录

## Out of Scope
- 不实现 benchmark runner、统计逻辑或 CSV 导出
- 不实现 Synthetic cases
- 不实现 Editor 菜单命令和构建逻辑
- 不实现 Player 自动运行逻辑
- 不接入 RealWorld case

## Affected Area
- `Packages/`
- `ProjectSettings/`
- `Assets/UnityPerfLab/`
- 根级 `.gitignore`

## Done When
- 仓库可以被当作 Unity 项目打开
- `Assets/UnityPerfLab` 目录结构存在且清晰隔离
- 所有 `UnityPerfLab.*` asmdef 已创建且依赖方向正确
- Bootstrap Scene 已存在并进入 build settings
- Editor 与 Runtime 无反向依赖

## Validation

### Black-box Checks
- 用 Unity 打开仓库，确认项目可导入
- 确认 `UnityPerfLabBootstrap` scene 出现在 build settings
- 确认 asmdef 导入后无循环依赖或平台错误
- 确认 `.gitignore` 已覆盖 Unity 生成目录与 `PerfLabResults/`

### White-box Needed
No

### White-box Trigger
本任务主要是工程骨架与边界建立，验收以结构是否存在、项目是否可打开为主，黑盒检查足够。

### Internal Logic To Protect
N/A

## Write-back Needed
Yes

If yes, what stable information should be written back, and where does it belong?

- 若仓库角色从 SOP-only 变为 Unity 项目，应更新 `docs/facts/project-scope.md`

## Risks / Notes
- `ProjectVersion.txt` 应与本机实际使用的 `2022.3.x` patch 保持一致
- 手写的 scene / asset YAML 在首次被 Unity 打开后应重新保存一次，降低序列化格式风险
- asmdef 边界要在第一步就收紧，避免后续 benchmark 代码直接散落在默认程序集里
