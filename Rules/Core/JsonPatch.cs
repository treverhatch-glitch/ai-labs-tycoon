using System;

namespace AiLabs.Rules.Core
{
    /// <summary>
    /// Small edits to a parsed save, for the save migrations: add a key with its default, remove one, or
    /// visit every object a dotted path leads to. A migration is a pure function from the old JSON to the new,
    /// so it never needs the old shape of any C# class.
    /// </summary>
    public static class JsonPatch
    {
        /// <summary>Sets the key only when the object does not have it yet, so a step can run twice safely.</summary>
        public static void AddIfMissing(JsonValue obj, string key, JsonValue value)
        {
            if (obj == null || obj.Kind != JsonKind.Object) throw new JsonException("AddIfMissing needs an object");
            if (obj.Get(key) == null) obj.Set(key, value);
        }

        /// <summary>Takes the key out and returns its value, or null when the object has no such key.</summary>
        public static JsonValue Remove(JsonValue obj, string key)
        {
            if (obj == null || obj.Kind != JsonKind.Object) throw new JsonException("Remove needs an object");
            int at = obj.Keys.IndexOf(key);
            if (at < 0) return null;
            JsonValue value = obj.Items[at];
            obj.Keys.RemoveAt(at);
            obj.Items.RemoveAt(at);
            return value;
        }

        /// <summary>
        /// Calls <paramref name="fn"/> for every object a dotted path reaches from the root. A segment that names a
        /// list visits each of its items, so <c>"labs.ledger.history.totals"</c> reaches the totals of every month
        /// of every lab. An empty path is the root itself. A missing key on the way ends that branch quietly.
        /// </summary>
        public static void ForEach(JsonValue root, string path, Action<JsonValue> fn)
        {
            string[] parts = string.IsNullOrEmpty(path) ? new string[0] : path.Split('.');
            Visit(root, parts, 0, fn);
        }

        static void Visit(JsonValue node, string[] parts, int at, Action<JsonValue> fn)
        {
            if (node == null) return;
            if (node.Kind == JsonKind.Array)
            {
                for (int i = 0; i < node.Items.Count; i++) Visit(node.Items[i], parts, at, fn);
                return;
            }
            if (at == parts.Length)
            {
                if (node.Kind == JsonKind.Object) fn(node);
                return;
            }
            if (node.Kind != JsonKind.Object) return;
            Visit(node.Get(parts[at]), parts, at + 1, fn);
        }

        /// <summary>The whole number under a key, or <paramref name="fallback"/> when it is missing or not a number.</summary>
        public static long IntOr(JsonValue obj, string key, long fallback)
        {
            JsonValue v = obj == null ? null : obj.Get(key);
            return v != null && v.Kind == JsonKind.Int ? v.Int : fallback;
        }

        /// <summary>The text under a key, or <paramref name="fallback"/>.</summary>
        public static string StrOr(JsonValue obj, string key, string fallback)
        {
            JsonValue v = obj == null ? null : obj.Get(key);
            return v != null && v.Kind == JsonKind.String ? v.Str : fallback;
        }

        /// <summary>The flag under a key, or <paramref name="fallback"/> when it is missing or not a boolean.</summary>
        public static bool BoolOr(JsonValue obj, string key, bool fallback)
        {
            JsonValue v = obj == null ? null : obj.Get(key);
            return v != null && v.Kind == JsonKind.Bool ? v.Bool : fallback;
        }
    }
}
