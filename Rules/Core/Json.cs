using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace AiLabs.Rules.Core
{
    public enum JsonKind { Null, Bool, Int, String, Array, Object }

    public sealed class JsonException : Exception
    {
        public JsonException(string message) : base(message) { }
    }

    /// <summary>
    /// A small JSON document model. Objects keep their keys in written order, so output is stable.
    /// Numbers are whole (long) only: the rules have no floating point anywhere.
    /// </summary>
    public sealed class JsonValue
    {
        public JsonKind Kind;
        public bool Bool;
        public long Int;
        public string Str;
        public List<JsonValue> Items;
        public List<string> Keys;

        public static JsonValue Null() { return new JsonValue { Kind = JsonKind.Null }; }
        public static JsonValue Of(bool value) { return new JsonValue { Kind = JsonKind.Bool, Bool = value }; }
        public static JsonValue Of(long value) { return new JsonValue { Kind = JsonKind.Int, Int = value }; }
        public static JsonValue Of(string value) { return value == null ? Null() : new JsonValue { Kind = JsonKind.String, Str = value }; }
        public static JsonValue Array() { return new JsonValue { Kind = JsonKind.Array, Items = new List<JsonValue>() }; }
        public static JsonValue Object() { return new JsonValue { Kind = JsonKind.Object, Items = new List<JsonValue>(), Keys = new List<string>() }; }

        public int Count { get { return Items == null ? 0 : Items.Count; } }

        public void Add(JsonValue item)
        {
            if (Kind != JsonKind.Array) throw new JsonException("Add(item) needs an array");
            Items.Add(item);
        }

        public void Set(string key, JsonValue value)
        {
            if (Kind != JsonKind.Object) throw new JsonException("Set(key, value) needs an object");
            int at = Keys.IndexOf(key);
            if (at >= 0) { Items[at] = value; return; }
            Keys.Add(key);
            Items.Add(value);
        }

        /// <summary>The value under key, or null when the object has no such key.</summary>
        public JsonValue Get(string key)
        {
            if (Kind != JsonKind.Object) return null;
            int at = Keys.IndexOf(key);
            return at < 0 ? null : Items[at];
        }
    }

    public static class Json
    {
        public static JsonValue Parse(string text)
        {
            if (text == null) throw new JsonException("no JSON text");
            var p = new Parser(text);
            p.SkipSpace();
            JsonValue v = p.ReadValue(0);
            p.SkipSpace();
            if (!p.AtEnd) throw p.Fail("unexpected text after the document");
            return v;
        }

        public static string Write(JsonValue value, bool indented)
        {
            var sb = new StringBuilder();
            WriteValue(sb, value, indented, 0);
            if (indented) sb.Append('\n');
            return sb.ToString();
        }

        static void WriteValue(StringBuilder sb, JsonValue v, bool indented, int depth)
        {
            switch (v.Kind)
            {
                case JsonKind.Null: sb.Append("null"); return;
                case JsonKind.Bool: sb.Append(v.Bool ? "true" : "false"); return;
                case JsonKind.Int: sb.Append(v.Int.ToString(CultureInfo.InvariantCulture)); return;
                case JsonKind.String: WriteString(sb, v.Str); return;
                case JsonKind.Array:
                    if (v.Items.Count == 0) { sb.Append("[]"); return; }
                    bool flat = !indented || IsFlat(v);
                    sb.Append('[');
                    for (int i = 0; i < v.Items.Count; i++)
                    {
                        if (i > 0) sb.Append(flat && indented ? ", " : ",");
                        if (!flat) NewLine(sb, depth + 1);
                        WriteValue(sb, v.Items[i], indented, depth + 1);
                    }
                    if (!flat) NewLine(sb, depth);
                    sb.Append(']');
                    return;
                case JsonKind.Object:
                    if (v.Items.Count == 0) { sb.Append("{}"); return; }
                    sb.Append('{');
                    for (int i = 0; i < v.Items.Count; i++)
                    {
                        if (i > 0) sb.Append(',');
                        if (indented) NewLine(sb, depth + 1);
                        WriteString(sb, v.Keys[i]);
                        sb.Append(indented ? ": " : ":");
                        WriteValue(sb, v.Items[i], indented, depth + 1);
                    }
                    if (indented) NewLine(sb, depth);
                    sb.Append('}');
                    return;
            }
        }

        /// <summary>An array of plain values is written on one line, even when indenting.</summary>
        static bool IsFlat(JsonValue array)
        {
            for (int i = 0; i < array.Items.Count; i++)
            {
                JsonKind k = array.Items[i].Kind;
                if (k == JsonKind.Array || k == JsonKind.Object) return false;
            }
            return true;
        }

        static void NewLine(StringBuilder sb, int depth)
        {
            sb.Append('\n');
            sb.Append(' ', depth * 2);
        }

        static void WriteString(StringBuilder sb, string s)
        {
            sb.Append('"');
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                switch (c)
                {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < 0x20) sb.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                        else sb.Append(c);
                        break;
                }
            }
            sb.Append('"');
        }

        sealed class Parser
        {
            const int MaxDepth = 64;
            readonly string _s;
            int _i;

            public Parser(string s) { _s = s; }

            public bool AtEnd { get { return _i >= _s.Length; } }

            public JsonException Fail(string why)
            {
                int line = 1, col = 1;
                for (int k = 0; k < _i && k < _s.Length; k++)
                {
                    if (_s[k] == '\n') { line++; col = 1; } else col++;
                }
                return new JsonException("JSON line " + line.ToString(CultureInfo.InvariantCulture) + ", column " + col.ToString(CultureInfo.InvariantCulture) + ": " + why);
            }

            public void SkipSpace()
            {
                while (_i < _s.Length)
                {
                    char c = _s[_i];
                    if (c == ' ' || c == '\n' || c == '\r' || c == '\t') _i++;
                    else break;
                }
            }

            public JsonValue ReadValue(int depth)
            {
                if (depth > MaxDepth) throw Fail("nested too deep");
                if (AtEnd) throw Fail("unexpected end of text");
                char c = _s[_i];
                if (c == '{') return ReadObject(depth);
                if (c == '[') return ReadArray(depth);
                if (c == '"') return JsonValue.Of(ReadString());
                if (c == '-' || (c >= '0' && c <= '9')) return ReadNumber();
                if (Take("true")) return JsonValue.Of(true);
                if (Take("false")) return JsonValue.Of(false);
                if (Take("null")) return JsonValue.Null();
                throw Fail("unexpected character '" + c + "'");
            }

            bool Take(string word)
            {
                if (string.CompareOrdinal(_s, _i, word, 0, word.Length) != 0) return false;
                _i += word.Length;
                return true;
            }

            JsonValue ReadObject(int depth)
            {
                JsonValue obj = JsonValue.Object();
                _i++;
                SkipSpace();
                if (!AtEnd && _s[_i] == '}') { _i++; return obj; }
                while (true)
                {
                    SkipSpace();
                    if (AtEnd || _s[_i] != '"') throw Fail("expected a key in quotes");
                    string key = ReadString();
                    if (obj.Keys.Contains(key)) throw Fail("the key \"" + key + "\" appears twice");
                    SkipSpace();
                    if (AtEnd || _s[_i] != ':') throw Fail("expected ':' after the key \"" + key + "\"");
                    _i++;
                    SkipSpace();
                    obj.Keys.Add(key);
                    obj.Items.Add(ReadValue(depth + 1));
                    SkipSpace();
                    if (AtEnd) throw Fail("unexpected end of text inside an object");
                    if (_s[_i] == ',') { _i++; continue; }
                    if (_s[_i] == '}') { _i++; return obj; }
                    throw Fail("expected ',' or '}'");
                }
            }

            JsonValue ReadArray(int depth)
            {
                JsonValue arr = JsonValue.Array();
                _i++;
                SkipSpace();
                if (!AtEnd && _s[_i] == ']') { _i++; return arr; }
                while (true)
                {
                    SkipSpace();
                    arr.Items.Add(ReadValue(depth + 1));
                    SkipSpace();
                    if (AtEnd) throw Fail("unexpected end of text inside an array");
                    if (_s[_i] == ',') { _i++; continue; }
                    if (_s[_i] == ']') { _i++; return arr; }
                    throw Fail("expected ',' or ']'");
                }
            }

            JsonValue ReadNumber()
            {
                int start = _i;
                if (_s[_i] == '-') _i++;
                while (_i < _s.Length && _s[_i] >= '0' && _s[_i] <= '9') _i++;
                if (_i < _s.Length && (_s[_i] == '.' || _s[_i] == 'e' || _s[_i] == 'E'))
                    throw Fail("only whole numbers are allowed: the rules use permille and whole dollars, never fractions");
                long value;
                if (!long.TryParse(_s.Substring(start, _i - start), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value))
                    throw Fail("not a whole number that fits in 64 bits");
                return JsonValue.Of(value);
            }

            string ReadString()
            {
                var sb = new StringBuilder();
                _i++;
                while (true)
                {
                    if (AtEnd) throw Fail("a string is not closed");
                    char c = _s[_i++];
                    if (c == '"') return sb.ToString();
                    if (c < 0x20) throw Fail("a raw control character inside a string");
                    if (c != '\\') { sb.Append(c); continue; }
                    if (AtEnd) throw Fail("a string is not closed");
                    char e = _s[_i++];
                    switch (e)
                    {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            if (_i + 4 > _s.Length) throw Fail("a \\u escape is cut short");
                            int code;
                            if (!int.TryParse(_s.Substring(_i, 4), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out code))
                                throw Fail("a \\u escape is not four hex digits");
                            sb.Append((char)code);
                            _i += 4;
                            break;
                        default: throw Fail("unknown escape '\\" + e + "'");
                    }
                }
            }
        }
    }
}
