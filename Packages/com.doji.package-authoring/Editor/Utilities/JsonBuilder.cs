using Newtonsoft.Json.Linq;

namespace Doji.PackageAuthoring.Editor.Utilities {
    public static class JsonBuilder {
        public static JObject Obj(params JProperty[] props) {
            JObject o = new JObject();

            foreach (JProperty p in props)
                if (p != null)
                    o.Add(p);

            return o;
        }

        public static JArray Arr(params object[] items) {
            JArray a = new JArray();

            foreach (object i in items)
                if (i != null)
                    a.Add(i);

            return a;
        }

        public static JProperty Prop(string name, object value) {
            return value == null ? null : new JProperty(name, value);
        }

        public static JProperty PropIf(bool condition, string name, object value) {
            return condition ? new JProperty(name, value) : null;
        }
    }
}
