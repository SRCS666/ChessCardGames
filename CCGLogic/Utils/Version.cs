using System.Text.Json.Nodes;

namespace CCGLogic.Utils
{
    public enum VersionType
    {
        Alpha,
        Beta,
        Official,
        Unknown
    }

    public class Version(int major, int minor, int build,
        VersionType type = VersionType.Official, int revision = 1)
    {
        public int Major { get; } = major;
        public int Minor { get; } = minor;
        public int Build { get; } = build;
        public VersionType Type { get; } = type;
        public int Revision { get; } = revision;

        public JsonArray ToJsonArray()
        {
            JsonArray array = [];

            array.Add(Major);
            array.Add(Minor);
            array.Add(Build);
            array.Add(Convert.ToInt32(Type));
            array.Add(Revision);

            return array;
        }

        public static Version Parse(JsonArray array)
        {
            int major = array[0].GetValue<int>();
            int minor = array[1].GetValue<int>();
            int build = array[2].GetValue<int>();
            VersionType type = (VersionType)array[3].GetValue<int>();
            int revision = array[4].GetValue<int>();

            return new(major, minor, build, type, revision);
        }

        public override string ToString() =>
            string.Format("{0}.{1}.{2}-{3}{4}", Major, Minor, Build, Type, Revision);

        public override bool Equals(object obj)
        {
            if (obj is Version)
            {
                Version v2 = obj as Version;
                return this == v2;
            }

            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Major, Minor, Build, Type, Revision);

        public static bool operator <(Version v1, Version v2)
        {
            if (v1.Major < v2.Major) { return true; }
            if (v1.Minor < v2.Minor) { return true; }
            if (v1.Build < v2.Build) { return true; }
            if (v1.Type < v2.Type) { return true; }
            if (v1.Revision < v2.Revision) { return true; }

            return false;
        }

        public static bool operator >(Version v1, Version v2) => v2 < v1;
        public static bool operator !=(Version v1, Version v2) => v1 > v2 || v1 < v2;
        public static bool operator ==(Version v1, Version v2) => !(v1 != v2);
        public static bool operator <=(Version v1, Version v2) => !(v1 > v2);
        public static bool operator >=(Version v1, Version v2) => !(v1 < v2);
    }
}
