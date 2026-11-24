using System;
using System.Collections.Generic;

namespace AiLabs.Rules.Core
{
    /// <summary>
    /// A class that can be written to and read from JSON. It names each field once, in <see cref="Sync"/>,
    /// and that one list serves saving, loading and hashing, so the three can never drift apart.
    /// </summary>
    public interface ISync
    {
        void Sync(Syncer s);
    }

    public sealed class SyncException : Exception
    {
        public SyncException(string message) : base(message) { }
    }

    /// <summary>
    /// Either a reader or a writer over one JSON object. Reading is strict: a missing key and an
    /// unknown key are both errors, so a typo in a data file is found when it is loaded.
    /// </summary>
    public abstract class Syncer
    {
        public abstract bool IsReading { get; }
        public abstract void Int(string key, ref int value);
        public abstract void Long(string key, ref long value);
        public abstract void Bool(string key, ref bool value);
        public abstract void Str(string key, ref string value);
        public abstract void Enum<T>(string key, ref T value) where T : struct, Enum;
        public abstract void Obj<T>(string key, ref T value) where T : class, ISync, new();
        public abstract void ObjList<T>(string key, ref List<T> list) where T : class, ISync, new();
        public abstract void IntList(string key, ref List<int> list);
        public abstract void StrList(string key, ref List<string> list);

        public static JsonValue Write(ISync obj)
        {
            var w = new SyncWriter();
            obj.Sync(w);
            return w.Result;
        }

        public static T Read<T>(JsonValue json, string what) where T : class, ISync, new()
        {
            if (json == null || json.Kind != JsonKind.Object) throw new SyncException(what + " must be a JSON object");
            var r = new SyncReader(json, what);
            var obj = new T();
            obj.Sync(r);
            r.Finish();
            return obj;
        }
    }

    sealed class SyncWriter : Syncer
    {
        public readonly JsonValue Result = JsonValue.Object();

        public override bool IsReading { get { return false; } }
        public override void Int(string key, ref int value) { Result.Set(key, JsonValue.Of(value)); }
        public override void Long(string key, ref long value) { Result.Set(key, JsonValue.Of(value)); }
        public override void Bool(string key, ref bool value) { Result.Set(key, JsonValue.Of(value)); }
        public override void Str(string key, ref string value) { Result.Set(key, JsonValue.Of(value ?? "")); }
        public override void Enum<T>(string key, ref T value) { Result.Set(key, JsonValue.Of(value.ToString())); }

        public override void Obj<T>(string key, ref T value)
        {
            if (value == null) throw new SyncException("cannot write \"" + key + "\": it is not set");
            Result.Set(key, Write(value));
        }

        public override void ObjList<T>(string key, ref List<T> list)
        {
            JsonValue arr = JsonValue.Array();
            if (list != null) for (int i = 0; i < list.Count; i++) arr.Add(Write(list[i]));
            Result.Set(key, arr);
        }

        public override void IntList(string key, ref List<int> list)
        {
            JsonValue arr = JsonValue.Array();
            if (list != null) for (int i = 0; i < list.Count; i++) arr.Add(JsonValue.Of(list[i]));
            Result.Set(key, arr);
        }

        public override void StrList(string key, ref List<string> list)
        {
            JsonValue arr = JsonValue.Array();
            if (list != null) for (int i = 0; i < list.Count; i++) arr.Add(JsonValue.Of(list[i] ?? ""));
            Result.Set(key, arr);
        }
    }

    sealed class SyncReader : Syncer
    {
        readonly JsonValue _json;
        readonly string _what;
        readonly bool[] _used;

        public SyncReader(JsonValue json, string what)
        {
            _json = json;
            _what = what;
            _used = new bool[json.Count];
        }

        public override bool IsReading { get { return true; } }

        JsonValue Take(string key, JsonKind kind)
        {
            int at = _json.Keys.IndexOf(key);
            if (at < 0) throw new SyncException(_what + " is missing \"" + key + "\"");
            _used[at] = true;
            JsonValue v = _json.Items[at];
            if (v.Kind != kind) throw new SyncException(_what + ": \"" + key + "\" must be " + Describe(kind) + ", not " + Describe(v.Kind));
            return v;
        }

        static string Describe(JsonKind kind)
        {
            switch (kind)
            {
                case JsonKind.Int: return "a whole number";
                case JsonKind.Bool: return "true or false";
                case JsonKind.String: return "text";
                case JsonKind.Array: return "a list";
                case JsonKind.Object: return "an object";
                default: return "null";
            }
        }

        /// <summary>Every key of the object must have been read: an unknown key is a typo or a stale file.</summary>
        public void Finish()
        {
            for (int i = 0; i < _used.Length; i++)
                if (!_used[i]) throw new SyncException(_what + " has an unknown key \"" + _json.Keys[i] + "\"");
        }

        public override void Int(string key, ref int value)
        {
            long v = Take(key, JsonKind.Int).Int;
            if (v < int.MinValue || v > int.MaxValue) throw new SyncException(_what + ": \"" + key + "\" is out of range");
            value = (int)v;
        }

        public override void Long(string key, ref long value) { value = Take(key, JsonKind.Int).Int; }
        public override void Bool(string key, ref bool value) { value = Take(key, JsonKind.Bool).Bool; }
        public override void Str(string key, ref string value) { value = Take(key, JsonKind.String).Str; }

        public override void Enum<T>(string key, ref T value)
        {
            string name = Take(key, JsonKind.String).Str;
            T parsed;
            if (!System.Enum.TryParse(name, false, out parsed) || !System.Enum.IsDefined(typeof(T), parsed))
                throw new SyncException(_what + ": \"" + key + "\" is \"" + name + "\", which is not one of " + string.Join(", ", System.Enum.GetNames(typeof(T))));
            value = parsed;
        }

        public override void Obj<T>(string key, ref T value)
        {
            value = Read<T>(Take(key, JsonKind.Object), _what + "." + key);
        }

        public override void ObjList<T>(string key, ref List<T> list)
        {
            JsonValue arr = Take(key, JsonKind.Array);
            list = new List<T>(arr.Count);
            for (int i = 0; i < arr.Count; i++) list.Add(Read<T>(arr.Items[i], _what + "." + key + "[" + i + "]"));
        }

        public override void IntList(string key, ref List<int> list)
        {
            JsonValue arr = Take(key, JsonKind.Array);
            list = new List<int>(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                JsonValue v = arr.Items[i];
                if (v.Kind != JsonKind.Int || v.Int < int.MinValue || v.Int > int.MaxValue)
                    throw new SyncException(_what + ": \"" + key + "\" must be a list of whole numbers");
                list.Add((int)v.Int);
            }
        }

        public override void StrList(string key, ref List<string> list)
        {
            JsonValue arr = Take(key, JsonKind.Array);
            list = new List<string>(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                if (arr.Items[i].Kind != JsonKind.String) throw new SyncException(_what + ": \"" + key + "\" must be a list of text");
                list.Add(arr.Items[i].Str);
            }
        }
    }
}
