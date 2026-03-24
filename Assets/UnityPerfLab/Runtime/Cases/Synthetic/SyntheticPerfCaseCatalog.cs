using System.Collections.Generic;
using UnityPerfLab.Runtime.Core;

namespace UnityPerfLab.Cases.Synthetic
{
    public static class SyntheticPerfCaseCatalog
    {
        private static readonly int[] WorkloadSizes = { 1000, 100000, 1000000 };

        public static List<IPerfCase> CreateAll()
        {
            List<IPerfCase> cases = new List<IPerfCase>();

            for (int i = 0; i < WorkloadSizes.Length; i++)
            {
                int workloadSize = WorkloadSizes[i];
                cases.Add(new ArrayIntIterationPerfCase(true, workloadSize));
                cases.Add(new ArrayIntIterationPerfCase(false, workloadSize));
                cases.Add(new ListIntIterationPerfCase(true, workloadSize));
                cases.Add(new ListIntIterationPerfCase(false, workloadSize));
                cases.Add(new ListStructIterationPerfCase(true, workloadSize));
                cases.Add(new ListStructIterationPerfCase(false, workloadSize));
                cases.Add(new CallDispatchPerfCase(false, workloadSize));
                cases.Add(new CallDispatchPerfCase(true, workloadSize));
                cases.Add(new ClosureCapturePerfCase(false, workloadSize));
                cases.Add(new ClosureCapturePerfCase(true, workloadSize));
                cases.Add(new DictionaryLookupPerfCase(DictionaryLookupPerfCase.LookupVariant.DictionaryInt, workloadSize));
                cases.Add(new DictionaryLookupPerfCase(DictionaryLookupPerfCase.LookupVariant.DictionaryIntExplicitDefaultComparer, workloadSize));
                cases.Add(new DictionaryLookupPerfCase(DictionaryLookupPerfCase.LookupVariant.SimpleIntDictionary, workloadSize));
                cases.Add(new DictionaryLookupPerfCase(DictionaryLookupPerfCase.LookupVariant.SimpleUIntDictionary, workloadSize));
                cases.Add(new DictionaryLookupPerfCase(DictionaryLookupPerfCase.LookupVariant.SimpleUlongDictionary, workloadSize));
                cases.Add(new DictionaryLookupPerfCase(DictionaryLookupPerfCase.LookupVariant.SimpleStringDictionary, workloadSize));
                cases.Add(new LinkedListIterationPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new LinkedListIterationPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new LinkedListIterationPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new LinkedListAddPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new LinkedListAddPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new LinkedListAddPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new ListTraversalPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new ListTraversalPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new ListTraversalPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new ListAddPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new ListAddPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new ListAddPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new SimpleListTraversalPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new SimpleListTraversalPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new SimpleListTraversalPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new SimpleListAddPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new SimpleListAddPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new SimpleListAddPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new SimpleLinkListIterationPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new SimpleLinkListIterationPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new SimpleLinkListIterationPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new SimpleLinkListAddPerfCase(CollectionPayloadKind.Int, workloadSize));
                cases.Add(new SimpleLinkListAddPerfCase(CollectionPayloadKind.Class, workloadSize));
                cases.Add(new SimpleLinkListAddPerfCase(CollectionPayloadKind.Struct, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.StaticPropertyVectorInLoop, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.StaticPropertyVectorHoistedLocal, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.ConstScalar, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.StaticReadonlyScalar, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.DeclarationOnlyStaticInitialization, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.ExplicitStaticConstructorInitialization, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.StaticReadonlyFieldAccess, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.StaticPropertyAccess, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.FlatHelper, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.HelperChain, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.FixedArityHelper, workloadSize));
                cases.Add(new Il2CppSemanticOverheadPerfCase(Il2CppSemanticScenario.ParamsHelper, workloadSize));
            }

            return cases;
        }
    }
}
