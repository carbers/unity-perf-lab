# IL2CPP Hot-Path Governance

## Scope

This fact records governance guidance derived from the validated Win64 IL2CPP Release Player run:

- run output: `Temp/PlayerValidationResults/20260323T144003790Z-il2cpp-release/20260323T064006621Z-981532/`
- primary evidence:
  - `summary.csv`
  - `overview.md`
  - `metadata.json`
- runtime identity:
  - Unity `2022.3.16f1`
  - `platform = WindowsPlayer`
  - `buildConfiguration = ReleasePlayer`
  - `scriptingBackend = IL2CPP`

Use this fact as a governance reference for hot-path cleanup work, not as a claim that every codebase should always apply the same rule.

## Executive Summary

The current IL2CPP player run shows four governance-relevant findings with large and repeatable deltas:

1. `params` in a hot path is catastrophic in this benchmark shape and should be treated as a default ban for hot loops.
2. Re-reading a static property inside the loop body is much slower than hoisting the same value into a local before the loop.
3. `const` scalar access is materially cheaper than `static readonly` scalar access in this synthetic hot-loop shape.
4. Static property wrappers over static state are materially slower than direct static readonly field access in this synthetic hot-loop shape.

Two benchmark groups did not justify a governance rule from this run:

- declaration-only static initialization vs explicit static constructor
- flat helper vs thin helper chain

## Governance Priorities

### Priority 0: Ban `params` helpers from hot loops

Measured pair:

| Workload | Fixed arity median ns | `params` median ns | Slowdown |
| --- | ---: | ---: | ---: |
| `1K` | `130.7445` | `36845.1071` | `281.81x` |
| `100K` | `10980.0781` | `3854060.2273` | `351.00x` |
| `1M` | `111103.1250` | `57533671.8750` | `517.84x` |

Measured shapes:

```csharp
private static int SumFixedArity(int a, int b, int c)
{
    return a + b + c;
}

private static int SumParams(params int[] values)
{
    return values[0] + values[1] + values[2];
}

for (int i = 0; i < workloadSize; i++)
{
    total += SumFixedArity(i, i + 1, i + 2);
}

for (int i = 0; i < workloadSize; i++)
{
    total += SumParams(i, i + 1, i + 2);
}
```

Governance rule:

- Do not use `params` helpers inside per-element hot loops.
- Add fixed-arity overloads for the hot path and route convenience APIs to non-hot code only.

Rationale:

- This is the largest semantic-overhead delta in the current run.
- The slowdown is not marginal and stays extreme at all workload tiers.
- The measured path intentionally includes the `params` array creation cost, which is exactly why this rule matters for governance.

### Priority 1: Hoist static property reads out of hot loops

Measured pair:

| Workload | Hoisted local median ns | Property-in-loop median ns | Slowdown |
| --- | ---: | ---: | ---: |
| `1K` | `113.0418` | `793.2968` | `7.02x` |
| `100K` | `10323.0469` | `78285.5469` | `7.58x` |
| `1M` | `103181.2500` | `781687.5000` | `7.58x` |

Measured shapes:

```csharp
for (int i = 0; i < workloadSize; i++)
{
    VectorLike3 basis = VectorLikeSource.Basis;
    total += basis.x + basis.y + basis.z + i;
}

VectorLike3 basis = VectorLikeSource.Basis;
for (int i = 0; i < workloadSize; i++)
{
    total += basis.x + basis.y + basis.z + i;
}
```

Governance rule:

- If a static property value is loop-invariant, hoist it to a local before the loop.
- Treat this as a default review comment for IL2CPP hot paths unless readability or correctness would be harmed.

Rationale:

- The slowdown is large and stable across all workload sizes.
- The semantic difference is narrow and easy to remediate without API redesign.

### Priority 1: Prefer `const` over `static readonly` for hot-loop scalar constants

Measured pair:

| Workload | `const` median ns | `static readonly` median ns | Slowdown |
| --- | ---: | ---: | ---: |
| `1K` | `113.7995` | `329.1868` | `2.89x` |
| `100K` | `10746.4844` | `31657.4219` | `2.95x` |
| `1M` | `108390.6250` | `315759.3750` | `2.91x` |

Measured shapes:

```csharp
private static class ConstScalarSource
{
    public const int Value = 31;
}

private static class StaticReadonlyScalarSource
{
    public static readonly int Value = 31;
}

for (int i = 0; i < workloadSize; i++)
{
    total += i + ConstScalarSource.Value;
}

for (int i = 0; i < workloadSize; i++)
{
    total += i + StaticReadonlyScalarSource.Value;
}
```

Governance rule:

- For true compile-time constants used in hot loops, prefer `const`.
- Reserve `static readonly` for values that cannot legally or safely be `const`.

Rationale:

- The delta is smaller than the `params` or hoisting findings, but still consistently near `3x`.
- This is only a good rule for genuine constants. Do not convert values that need runtime initialization or address stability.

### Priority 2: Prefer direct static field access over trivial static property wrappers in hot paths

Measured pair:

| Workload | Field median ns | Property median ns | Slowdown |
| --- | ---: | ---: | ---: |
| `1K` | `327.6930` | `646.0250` | `1.97x` |
| `100K` | `31603.1250` | `62832.0313` | `1.99x` |
| `1M` | `316703.1250` | `629021.8750` | `1.99x` |

Measured shapes:

```csharp
private static class StaticAccessHolder
{
    public static readonly int ReadonlyValue = 23;

    public static int PropertyValue
    {
        get { return ReadonlyValue; }
    }
}

for (int i = 0; i < workloadSize; i++)
{
    total += i + StaticAccessHolder.ReadonlyValue;
}

for (int i = 0; i < workloadSize; i++)
{
    total += i + StaticAccessHolder.PropertyValue;
}
```

Governance rule:

- In a hot path, do not wrap static state in a trivial property if direct field access is available and the visibility boundary permits it.
- If the property must remain public, cache the value before the loop when the value is invariant.

Rationale:

- This pair is not as extreme as `params`, but the `~2x` gap is stable and large enough to matter in per-frame inner loops.

## Low-Priority Or Neutral Findings

### No governance action from static initialization shape

Measured pair:

| Workload | Explicit cctor median ns | Declaration-only median ns | Slowdown |
| --- | ---: | ---: | ---: |
| `1K` | `326.3430` | `331.9535` | `1.02x` |
| `100K` | `32051.5625` | `32268.3594` | `1.01x` |
| `1M` | `321178.1250` | `321440.6250` | `1.00x` |

Governance implication:

- Do not create a broad coding rule around declaration-only static initialization vs explicit static constructor based on this run alone.
- Keep this as an observational benchmark only.

### No governance action from flat helper vs thin helper chain

Measured pair:

| Workload | Flat median ns | Helper-chain median ns | Slowdown |
| --- | ---: | ---: | ---: |
| `1K` | `114.8913` | `113.6383` | `0.99x` |
| `100K` | `10780.0781` | `11083.9844` | `1.03x` |
| `1M` | `107731.2500` | `107740.6250` | `1.00x` |

Governance implication:

- Do not spend engineering effort flattening tiny helper chains solely because they look cheaper on paper.
- Require real profiler evidence before touching readable helper structure for this reason.

## Cross-Suite Reference Points

These are not part of the new semantic-overhead family, but they remain large enough to matter for governance:

| Pair | `1K` slowdown | `100K` slowdown | `1M` slowdown |
| --- | ---: | ---: | ---: |
| direct call -> interface call | `5.10x` | `5.04x` | `5.06x` |
| no-capture closure -> capture closure | `87.17x` | `85.02x` | `88.99x` |

Governance implication:

- Avoid interface dispatch inside the innermost hot loop when a direct path is available.
- Treat closure capture inside hot loops as a default red flag.

## Governance Checklist

- Hot loop calls a `params` helper: replace with fixed arity or prebuilt storage.
- Hot loop reads a static property repeatedly: hoist once to a local.
- Hot loop uses `static readonly` for a true scalar constant: evaluate whether `const` is legal and appropriate.
- Hot loop accesses a trivial static property wrapper: prefer direct field access or local caching.
- Proposed cleanup only targets static initialization shape or tiny helper chaining: require stronger evidence before changing code.

## Interpretation Limits

- These results are synthetic and observational.
- The measured path is intentionally narrow, so the findings are strongest for tight inner loops.
- A governance rule here means "default bias during review," not "mandatory rewrite everywhere."
- Real gameplay code still needs profiler confirmation before broad churn.
