using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public sealed class Il2CppSemanticOverheadPerfCase : PerfCaseBase
    {
        private readonly Il2CppSemanticScenario scenario;
        private readonly int workloadSize;
        private readonly PerfCaseDescriptor descriptor;

        private struct VectorLike3
        {
            public int x;
            public int y;
            public int z;
        }

        private static class VectorLikeSource
        {
            private static readonly VectorLike3 basis = new VectorLike3
            {
                x = 3,
                y = 5,
                z = 7
            };

            public static VectorLike3 Basis
            {
                get { return basis; }
            }
        }

        private static class ConstScalarSource
        {
            public const int Value = 31;
        }

        private static class StaticReadonlyScalarSource
        {
            public static readonly int Value = 31;
        }

        private static class DeclarationOnlyStaticHolder
        {
            public static readonly int Value = 17;
        }

        private static class ExplicitStaticConstructorHolder
        {
            public static readonly int Value;

            static ExplicitStaticConstructorHolder()
            {
                Value = 17;
            }
        }

        private static class StaticAccessHolder
        {
            public static readonly int ReadonlyValue = 23;

            public static int PropertyValue
            {
                get { return ReadonlyValue; }
            }
        }

        public Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario scenario, int workloadSize)
        {
            this.scenario = scenario;
            this.workloadSize = workloadSize;

            descriptor = new PerfCaseDescriptor(
                GetCaseName(scenario, workloadSize),
                "SemanticOverhead",
                SyntheticMeasurementDefaults.CreateForTraversal(workloadSize),
                new Dictionary<string, string>
                {
                    { "subject", GetSubject(scenario) },
                    { "variant", GetVariant(scenario) },
                    { "size", workloadSize.ToString() }
                });
        }

        public override PerfCaseDescriptor Descriptor
        {
            get { return descriptor; }
        }

        public override void GlobalSetup()
        {
            // Force type initialization outside the measured path.
            long initializedValue = DeclarationOnlyStaticHolder.Value
                + ExplicitStaticConstructorHolder.Value
                + StaticAccessHolder.ReadonlyValue
                + StaticAccessHolder.PropertyValue
                + VectorLikeSource.Basis.x
                + StaticReadonlyScalarSource.Value;
            PerfVisibleSink.Write(initializedValue);
        }

        public override void Run(int iterations)
        {
            long total = 0;

            switch (scenario)
            {
                case Il2CppSemanticScenario.StaticPropertyVectorInLoop:
                    total = RunStaticPropertyVectorInLoop(iterations);
                    break;
                case Il2CppSemanticScenario.StaticPropertyVectorHoistedLocal:
                    total = RunStaticPropertyVectorHoistedLocal(iterations);
                    break;
                case Il2CppSemanticScenario.ConstScalar:
                    total = RunConstScalar(iterations);
                    break;
                case Il2CppSemanticScenario.StaticReadonlyScalar:
                    total = RunStaticReadonlyScalar(iterations);
                    break;
                case Il2CppSemanticScenario.DeclarationOnlyStaticInitialization:
                    total = RunDeclarationOnlyStaticInitialization(iterations);
                    break;
                case Il2CppSemanticScenario.ExplicitStaticConstructorInitialization:
                    total = RunExplicitStaticConstructorInitialization(iterations);
                    break;
                case Il2CppSemanticScenario.StaticReadonlyFieldAccess:
                    total = RunStaticReadonlyFieldAccess(iterations);
                    break;
                case Il2CppSemanticScenario.StaticPropertyAccess:
                    total = RunStaticPropertyAccess(iterations);
                    break;
                case Il2CppSemanticScenario.FlatHelper:
                    total = RunFlatHelper(iterations);
                    break;
                case Il2CppSemanticScenario.HelperChain:
                    total = RunHelperChain(iterations);
                    break;
                case Il2CppSemanticScenario.FixedArityHelper:
                    total = RunFixedArityHelper(iterations);
                    break;
                default:
                    total = RunParamsHelper(iterations);
                    break;
            }

            PerfVisibleSink.Write(total);
        }

        private long RunStaticPropertyVectorInLoop(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    VectorLike3 basis = VectorLikeSource.Basis;
                    total += basis.x + basis.y + basis.z + i;
                }
            }

            return total;
        }

        private long RunStaticPropertyVectorHoistedLocal(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                VectorLike3 basis = VectorLikeSource.Basis;
                for (int i = 0; i < workloadSize; i++)
                {
                    total += basis.x + basis.y + basis.z + i;
                }
            }

            return total;
        }

        private long RunConstScalar(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += i + ConstScalarSource.Value;
                }
            }

            return total;
        }

        private long RunStaticReadonlyScalar(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += i + StaticReadonlyScalarSource.Value;
                }
            }

            return total;
        }

        private long RunDeclarationOnlyStaticInitialization(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += i + DeclarationOnlyStaticHolder.Value;
                }
            }

            return total;
        }

        private long RunExplicitStaticConstructorInitialization(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += i + ExplicitStaticConstructorHolder.Value;
                }
            }

            return total;
        }

        private long RunStaticReadonlyFieldAccess(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += i + StaticAccessHolder.ReadonlyValue;
                }
            }

            return total;
        }

        private long RunStaticPropertyAccess(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += i + StaticAccessHolder.PropertyValue;
                }
            }

            return total;
        }

        private long RunFlatHelper(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += ComputeFlat(i);
                }
            }

            return total;
        }

        private long RunHelperChain(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += ComputeThroughHelperChain(i);
                }
            }

            return total;
        }

        private long RunFixedArityHelper(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += SumFixedArity(i, i + 1, i + 2);
                }
            }

            return total;
        }

        private long RunParamsHelper(int iterations)
        {
            long total = 0;
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                for (int i = 0; i < workloadSize; i++)
                {
                    total += SumParams(i, i + 1, i + 2);
                }
            }

            return total;
        }

        private static int ComputeFlat(int value)
        {
            return ((value + 3) * 5) - 7;
        }

        private static int ComputeThroughHelperChain(int value)
        {
            return HelperStepSubtract(HelperStepMultiply(HelperStepAdd(value)));
        }

        private static int HelperStepAdd(int value)
        {
            return value + 3;
        }

        private static int HelperStepMultiply(int value)
        {
            return value * 5;
        }

        private static int HelperStepSubtract(int value)
        {
            return value - 7;
        }

        private static int SumFixedArity(int a, int b, int c)
        {
            return a + b + c;
        }

        private static int SumParams(params int[] values)
        {
            return values[0] + values[1] + values[2];
        }

        private static string GetCaseName(Il2CppSemanticScenario scenario, int workloadSize)
        {
            switch (scenario)
            {
                case Il2CppSemanticScenario.StaticPropertyVectorInLoop:
                    return "StaticProperty_VectorLike_InLoop_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.StaticPropertyVectorHoistedLocal:
                    return "StaticProperty_VectorLike_HoistedLocal_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.ConstScalar:
                    return "ConstScalar_InLoop_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.StaticReadonlyScalar:
                    return "StaticReadonlyScalar_InLoop_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.DeclarationOnlyStaticInitialization:
                    return "StaticInit_DeclarationOnly_Field_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.ExplicitStaticConstructorInitialization:
                    return "StaticInit_ExplicitCctor_Field_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.StaticReadonlyFieldAccess:
                    return "StaticHolder_FieldAccess_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.StaticPropertyAccess:
                    return "StaticHolder_PropertyAccess_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.FlatHelper:
                    return "HelperCall_Flat_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.HelperChain:
                    return "HelperCall_Chain_" + FormatSizeLabel(workloadSize);
                case Il2CppSemanticScenario.FixedArityHelper:
                    return "HelperParams_FixedArity_" + FormatSizeLabel(workloadSize);
                default:
                    return "HelperParams_Params_" + FormatSizeLabel(workloadSize);
            }
        }

        private static string GetSubject(Il2CppSemanticScenario scenario)
        {
            switch (scenario)
            {
                case Il2CppSemanticScenario.StaticPropertyVectorInLoop:
                case Il2CppSemanticScenario.StaticPropertyVectorHoistedLocal:
                    return "VectorLikeStaticProperty";
                case Il2CppSemanticScenario.ConstScalar:
                case Il2CppSemanticScenario.StaticReadonlyScalar:
                    return "ScalarConstantShape";
                case Il2CppSemanticScenario.DeclarationOnlyStaticInitialization:
                case Il2CppSemanticScenario.ExplicitStaticConstructorInitialization:
                    return "StaticInitializationShape";
                case Il2CppSemanticScenario.StaticReadonlyFieldAccess:
                case Il2CppSemanticScenario.StaticPropertyAccess:
                    return "StaticHolderAccessShape";
                case Il2CppSemanticScenario.FlatHelper:
                case Il2CppSemanticScenario.HelperChain:
                    return "HelperIndirection";
                default:
                    return "ParamsConvenience";
            }
        }

        private static string GetVariant(Il2CppSemanticScenario scenario)
        {
            switch (scenario)
            {
                case Il2CppSemanticScenario.StaticPropertyVectorInLoop:
                    return "property-in-loop";
                case Il2CppSemanticScenario.StaticPropertyVectorHoistedLocal:
                    return "hoisted-local";
                case Il2CppSemanticScenario.ConstScalar:
                    return "const";
                case Il2CppSemanticScenario.StaticReadonlyScalar:
                    return "static-readonly";
                case Il2CppSemanticScenario.DeclarationOnlyStaticInitialization:
                    return "declaration-only";
                case Il2CppSemanticScenario.ExplicitStaticConstructorInitialization:
                    return "explicit-cctor";
                case Il2CppSemanticScenario.StaticReadonlyFieldAccess:
                    return "static-readonly-field";
                case Il2CppSemanticScenario.StaticPropertyAccess:
                    return "static-property";
                case Il2CppSemanticScenario.FlatHelper:
                    return "flat";
                case Il2CppSemanticScenario.HelperChain:
                    return "helper-chain";
                case Il2CppSemanticScenario.FixedArityHelper:
                    return "fixed-arity";
                default:
                    return "params";
            }
        }
    }

    public enum Il2CppSemanticScenario
    {
        StaticPropertyVectorInLoop,
        StaticPropertyVectorHoistedLocal,
        ConstScalar,
        StaticReadonlyScalar,
        DeclarationOnlyStaticInitialization,
        ExplicitStaticConstructorInitialization,
        StaticReadonlyFieldAccess,
        StaticPropertyAccess,
        FlatHelper,
        HelperChain,
        FixedArityHelper,
        ParamsHelper
    }
}
