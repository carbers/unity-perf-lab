namespace UnityPerfLab.Cases.Synthetic
{
    public enum CollectionPayloadKind
    {
        Int,
        Class,
        Struct
    }

    internal sealed class CollectionPayloadClass
    {
        public int a;
        public int b;
        public int c;
    }

    internal struct CollectionPayloadStruct
    {
        public int a;
        public int b;
        public int c;
    }

    internal static class CollectionPayloadFactory
    {
        public static string GetCollectionLabel(CollectionPayloadKind kind)
        {
            switch (kind)
            {
                case CollectionPayloadKind.Int:
                    return "int";
                case CollectionPayloadKind.Class:
                    return "class";
                default:
                    return "struct";
            }
        }

        public static string GetPayloadLabel(CollectionPayloadKind kind)
        {
            switch (kind)
            {
                case CollectionPayloadKind.Int:
                    return "Int";
                case CollectionPayloadKind.Class:
                    return "Class";
                default:
                    return "Struct";
            }
        }

        public static CollectionPayloadClass CreateClassPayload(int value)
        {
            return new CollectionPayloadClass
            {
                a = value,
                b = value + value,
                c = value + value + value
            };
        }

        public static CollectionPayloadStruct CreateStructPayload(int value)
        {
            CollectionPayloadStruct payload = new CollectionPayloadStruct();
            payload.a = value;
            payload.b = value + value;
            payload.c = value + value + value;
            return payload;
        }
    }
}
