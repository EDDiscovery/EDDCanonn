
using QuickJSON.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using static QuickJSON.JToken;
using static QuickJSON.JTokenExtensions;
using System.Text;
namespace QuickJSON
{
    internal static class NamespaceDoc { } // just for documentation purposes
    [System.Diagnostics.DebuggerDisplay("L{Level}:{Name}:{TokenType}= {ToString()}")]
    public partial class JToken : IEnumerable<JToken>, IEnumerable
    {
        public enum TType
        {
            Null,
            Boolean,
            String,
            Double,
            Long,
            ULong,
            BigInt,
            Object,
            Array,
            EndObject,
            EndArray,
            Error
        }
        public TType TokenType { get; set; }
        public Object Value { get; set; }
        public string Name { get; set; }
        public string OriginalName { get; set; }
        public string ParsedName { get { return OriginalName ?? Name; } }
        static public bool IsKeyNameSynthetic(string name) { return (name.StartsWith("!!!EmptyName") && name.EndsWith("!!!")) || (name.StartsWith("!!!Repeat-") && name.EndsWith("]!!!")); }
        public int Level { get; set; }
        public bool IsProperty { get { return Name != null; } }
        public bool HasValue { get { return Value != null; } }
        public bool IsString { get { return TokenType == TType.String; } }
        public bool IsInt { get { return TokenType == TType.Long || TokenType == TType.ULong || TokenType == TType.BigInt; } }
        public bool IsLong { get { return TokenType == TType.Long; } }
        public bool IsBigInt { get { return TokenType == TType.BigInt; } }
        public bool IsULong { get { return TokenType == TType.ULong; } }
        public bool IsDouble { get { return TokenType == TType.Double; } }
        public bool IsNumber { get { return TokenType == TType.Double || TokenType == TType.Long || TokenType == TType.ULong || TokenType == TType.BigInt; } }
        public bool IsBool { get { return TokenType == TType.Boolean; } }
        public bool IsArray { get { return TokenType == TType.Array; } }
        public bool IsObject { get { return TokenType == TType.Object; } }
        public bool IsNull { get { return TokenType == TType.Null; } }
        public bool IsEndObject { get { return TokenType == TType.EndObject; } }    // only seen for TokenReader
        public bool IsEndArray { get { return TokenType == TType.EndArray; } }      // only seen for TokenReader
        public bool IsInError { get { return TokenType == TType.Error; } }          // only seen for FromObject when asking for error return
        public string GetSchemaTypeName()
        {
            if (IsInt)
                return "integer";
            else if (IsDouble)
                return "number";
            else if (IsString)
                return "string";
            else if (IsBool)
                return "boolean";
            else if (IsArray)
                return "array";
            else if (IsObject)
                return "object";
            else if (IsNull)
                return "null";
            else
                System.Diagnostics.Debug.WriteLine($"{TokenType} is not a valid schema type");
            return null;
        }
        #region Construction
        public JToken()
        {
            TokenType = TType.Null;
        }
        public JToken(JToken other)
        {
            TokenType = other.TokenType;
            Value = other.Value;
            Name = other.Name;
        }
        public JToken(TType tokentype, Object value = null, int level = 0)
        {
            TokenType = tokentype; Value = value; Level = level;
        }
        public static implicit operator JToken(string v)
        {
            if (v == null)
                return new JToken(TType.Null);
            else
                return new JToken(TType.String, v);
        }
        public static implicit operator JToken(bool v)      // same order as https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/built-in-types
        {
            return new JToken(TType.Boolean, v);
        }
        public static implicit operator JToken(byte v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(sbyte v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(char v)
        {
            return new JToken(TType.String, v.ToString());
        }
        public static implicit operator JToken(decimal v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(double v)
        {
            return new JToken(TType.Double, v);
        }
        public static implicit operator JToken(float v)
        {
            return new JToken(TType.Double, (double)v);
        }
        public static implicit operator JToken(int v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(uint v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(long v)
        {
            return new JToken(TType.Long, v);
        }
        public static implicit operator JToken(ulong v)
        {
            return new JToken(TType.ULong, v);
        }
        public static implicit operator JToken(short v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(ushort v)
        {
            return new JToken(TType.Long, (long)v);
        }
        public static implicit operator JToken(DateTime v)
        {
            return new JToken(TType.String, v.ToStringZulu());
        }
        public static JToken CreateToken(Object obj, bool except = true)
        {
            if (obj == null)
                return Null();
            else if (obj is string)
                return (string)obj;
            else if (obj is bool || obj is bool?)
                return (bool)obj;
            else if (obj is byte || obj is byte?)
                return (byte)obj;
            else if (obj is sbyte || obj is sbyte?)
                return (sbyte)obj;
            else if (obj is char || obj is char?)
                return (char)obj;
            else if (obj is decimal || obj is decimal?)
                return (decimal)obj;
            else if (obj is double || obj is double?)
                return (double)obj;
            else if (obj is float || obj is float?)
                return (float)obj;
            else if (obj is int || obj is int?)
                return (int)obj;
            else if (obj is uint || obj is uint?)
                return (uint)obj;
            else if (obj is long || obj is long?)
                return (long)obj;
            else if (obj is ulong || obj is ulong?)
                return (ulong)obj;
            else if (obj is short || obj is short?)
                return (short)obj;
            else if (obj is ushort || obj is ushort?)
                return (ushort)obj;
            else if (obj is JArray)
            {
                var ja = obj as JArray;
                return ja.Clone();
            }
            else if (obj is JObject)
            {
                var jo = obj as JObject;
                return jo.Clone();
            }
            else if (obj is Enum)
            {
                return obj.ToString();
            }
            else if (obj is DateTime)
            {
                return ((DateTime)obj).ToStringZulu();        // obeys current culture and calandar
            }
            else if (except)
                throw new NotImplementedException();
            else
            {
                System.Diagnostics.Trace.WriteLine("QuickJSON Failed to serialise type " + obj.GetType().Name);
                return null;
            }
        }
        public static JToken Null()
        {
            return new JToken(TType.Null);
        }
        public static explicit operator string(JToken tk)
        {
            if (tk.TokenType == TType.Null || tk.TokenType != TType.String)
                return null;
            else
                return (string)tk.Value;
        }
        public static explicit operator int?(JToken tk)
        {
            if (tk.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                return (int)(long)tk.Value;
            else if (tk.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                return (int)(double)tk.Value;
            else
                return null;
        }
        public static explicit operator int(JToken tk)
        {
            if (tk.TokenType == TType.Long)
                return (int)(long)tk.Value;
            else if (tk.TokenType == TType.Double)
                return (int)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator uint?(JToken tk)
        {
            if (tk.TokenType == TType.Long && (long)tk.Value >= 0)        // it won't be a ulong/bigint since that would be too big for an uint
                return (uint)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)   // doubles get trunced
                return (uint)(double)tk.Value;
            else
                return null;
        }
        public static explicit operator uint(JToken tk)
        {
            if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                return (uint)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                return (uint)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator long?(JToken tk)
        {
            if (tk.TokenType == TType.Long)
                return (long)tk.Value;
            else if (tk.TokenType == TType.Double)
                return (long)(double)tk.Value;
            else
                return null;
        }
        public static explicit operator long(JToken tk)
        {
            if (tk.TokenType == TType.Long)              // it won't be a ulong/bigint since that would be too big for an long
                return (long)tk.Value;
            else if (tk.TokenType == TType.Double)       // doubles get trunced
                return (long)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator ulong?(JToken tk)
        {
            if (tk.TokenType == TType.ULong)             // it won't be a bigint since that would be too big for an ulong
                return (ulong)tk.Value;
            else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                return (ulong)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)       // doubles get trunced
                return (ulong)(double)tk.Value;
            else
                return null;
        }
        public static explicit operator ulong(JToken tk)
        {
            if (tk.TokenType == TType.ULong)
                return (ulong)tk.Value;
            else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                return (ulong)(long)tk.Value;
            else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                return (ulong)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator double?(JToken tk)
        {
            if (tk.TokenType == TType.Long)                      // any of these types could be converted to double
                return (double)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (double)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (double)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (double)tk.Value;
            else
                return null;
        }
        public static explicit operator double(JToken tk)
        {
            if (tk.TokenType == TType.Long)
                return (double)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (double)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (double)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (double)tk.Value;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator float?(JToken tk)
        {
            if (tk.TokenType == TType.Long)                  // any of these types could be converted to double
                return (float)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (float)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (float)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (float)(double)tk.Value;
            else
                return null;
        }
        public static explicit operator float(JToken tk)
        {
            if (tk.TokenType == TType.Long)
                return (float)(long)tk.Value;
            else if (tk.TokenType == TType.ULong)
                return (float)(ulong)tk.Value;
#if JSONBIGINT
            else if (tk.TokenType == TType.BigInt)
                return (float)(System.Numerics.BigInteger)tk.Value;
#endif
            else if (tk.TokenType == TType.Double)
                return (float)(double)tk.Value;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator bool?(JToken tk)
        {
            if (tk.TokenType == TType.Boolean)
                return (bool)tk.Value;
            else if (tk.TokenType == TType.Long)       // accept LONG 1/0 as boolean
                return (long)tk.Value != 0;
            else
                return null;
        }
        public static explicit operator bool(JToken tk)
        {
            if (tk.TokenType == TType.Boolean)
                return (bool)tk.Value;
            else if (tk.TokenType == TType.Long)
                return (long)tk.Value != 0;
            else
                throw new InvalidOperationException();
        }
        public static explicit operator DateTime(JToken t)
        {
            if (t.IsString && System.DateTime.TryParse((string)t.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                return ret;
            else
                return new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);        //Minvalue in utc mode
        }
        public JToken Clone()   // make a copy of the token
        {
            switch (TokenType)
            {
                case TType.Array:
                    {
                        JArray copy = new JArray();
                        foreach (JToken t in this)
                        {
                            copy.Add(t.Clone());
                        }
                        return copy;
                    }
                case TType.Object:
                    {
                        JObject copy = new JObject();
                        foreach (var kvp in (JObject)this)
                        {
                            copy[kvp.Key] = kvp.Value.Clone();
                        }
                        return copy;
                    }
                default:
                    return new JToken(this);
            }
        }
        #endregion
        #region Equality
        public bool ValueEquals(Object value)
        {
            if (value is string)
                return ((string)this) != null && ((string)this).Equals((string)value);
            else if (value is int)
                return ((int?)this) != null && ((int)this).Equals((int)value);
            else if (value is uint)
                return ((uint?)this) != null && ((uint)this).Equals((uint)value);
            else if (value is ulong)
                return ((ulong?)this) != null && ((ulong)this).Equals((ulong)value);
            else if (value is long)
                return ((long?)this) != null && ((long)this).Equals((long)value);
            else if (value is bool)
                return ((bool?)this) != null && ((bool)this).Equals((bool)value);
            else
                return false;
        }
        public bool ValueEquals(JToken other)
        {
            if (Value is string)
            {
                if (other.IsString)
                    return ((string)Value).Equals((string)other);
            }
            else if (Value is long)
            {
                if (other.IsLong || other.IsDouble)
                    return ((long)Value).Equals((long)other);
            }
            else if (Value is ulong)
            {
                if (other.IsULong)
                    return ((ulong)Value).Equals((ulong)other);
            }
            else if (Value is double)
            {
                if (other.IsNumber)
                    return ((double)Value).Equals((double)other);
            }
            else if (Value is bool)
            {
                if (other.IsBool)
                    return ((bool)Value).Equals((bool)other);
            }
            else if (TokenType == TType.Null)
            {
                if (other.TokenType == TType.Null)
                    return true;
            }
            return false;
        }
        #endregion
        #region Paths
        public JToken GetTokenSchemaPath(string path)
        {
            JToken token = this;
            while (path.Length > 0)
            {
                if (!token.IsObject)
                    return null;
                int indexofslash = path.IndexOf("/");
                string name = indexofslash == -1 ? path : path.Substring(0, indexofslash);
                path = indexofslash == -1 ? "" : path.Substring(indexofslash + 1);
                if (token.Object().Contains(name))
                    token = token.Object()[name];
                else
                    return null;
            }
            return token;
        }
        public JToken GetToken(string path)
        {
            JToken token = this;
            while (path.Length > 0)
            {
                if (token.IsArray)
                {
                    if (path[0] == '[')
                    {
                        int indexofbracket = path.IndexOf("]");
                        if (indexofbracket == -1)
                            return null;
                        int? index = path.Substring(1, indexofbracket - 1).InvariantParseIntNull();
                        if (index == null)
                            return null;
                        if (index >= token.Count || index < 0)
                            return null;
                        token = token[index];
                        path = path.Substring(indexofbracket + 1);
                    }
                    else
                        return null;
                }
                else if (token.IsObject)
                {
                    if (path[0] == '.')
                    {
                        path = path.Substring(1);
                        int indexofdot = path.IndexOfAny(new char[] { '[', '.' });
                        string name = indexofdot != -1 ? path.Substring(0, indexofdot) : path;
                        path = indexofdot != -1 ? path.Substring(indexofdot) : "";
                        if (token.Object().Contains(name))
                            token = token[name];
                        else
                            return null;
                    }
                    else
                        return null;
                }
                else
                    return null;
            }
            return token;
        }
        #endregion
        #region Operators and functions
        public virtual JToken this[object key] { get { return null; } set { throw new NotImplementedException(); } }
        public virtual bool Contains(string name) { return false; }
        public virtual void Add(JToken value) { throw new NotImplementedException(); }
        public virtual void Add<T>(T value) { throw new NotImplementedException(); }
        public virtual void AddRange(IEnumerable<JToken> o) { throw new NotImplementedException(); }
        public virtual void AddRange<T>(IEnumerable<T> values) { throw new NotImplementedException(); }
        public virtual void Add(string key, JToken value) { throw new NotImplementedException(); }
        public virtual void Add<T>(string key, T value) { throw new NotImplementedException(); }
        public virtual JToken First() { throw new NotImplementedException(); }
        public virtual JToken Last() { throw new NotImplementedException(); }
        public virtual JToken FirstOrDefault() { throw new NotImplementedException(); }
        public virtual JToken LastOrDefault() { throw new NotImplementedException(); }
        public IEnumerator<JToken> GetEnumerator()
        {
            return GetSubClassTokenEnumerator();
        }
        internal virtual IEnumerator<JToken> GetSubClassTokenEnumerator() { throw new NotImplementedException(); }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetSubClassEnumerator();
        }
        internal virtual IEnumerator GetSubClassEnumerator() { throw new NotImplementedException(); }
        public virtual int Count { get { return 0; } }
        public virtual void Clear() { throw new NotImplementedException(); }
        #endregion
        #region Debug Output Switch
        public static bool TraceOutput { get; set; } = false;
        #endregion
    }
}
namespace QuickJSON
{
    public class JArray : JToken
    {
        public JArray()
        {
            TokenType = TType.Array;
            Elements = new List<JToken>(16);
        }
        public JArray(params Object[] data) : this()
        {
            foreach (Object o in data)
                this.Add(JToken.CreateToken(o));
        }
        public JArray(IEnumerable data) : this()
        {
            foreach (Object o in data)
                this.Add(JToken.CreateToken(o));
        }
        public JArray(JToken othertoken) : this()        // construct with this token at start
        {
            Add(othertoken);
        }
        public override JToken this[object key]
        {
            get { if (key is int && (int)key >= 0 && (int)key < Elements.Count) return Elements[(int)key]; else return null; }
            set { System.Diagnostics.Debug.Assert(key is int); Elements[(int)key] = (value == null) ? JToken.Null() : value; }
        }
        public JToken this[int element] { get { return Elements[element]; } set { Elements[element] = value; } }
        public override JToken First() { return Elements[0]; }
        public override JToken Last() { return Elements[Elements.Count - 1]; }
        public override JToken FirstOrDefault() { return Elements.Count > 0 ? Elements[0] : null; }
        public override JToken LastOrDefault() { return Elements.Count > 0 ? Elements[Elements.Count - 1] : null; }
        public bool TryGetValue(int index, out JToken token) { if (index >= 0 && index < Elements.Count) { token = Elements[index]; return true; } else { token = null; return false; } }
        public override int Count { get { return Elements.Count; } }
        public int IndexOf(JToken tk) { return Elements.IndexOf(tk); }
        public override void Add(JToken o) { Elements.Add(o); }
        public override void Add<T>(T value) { dynamic x = value; Elements.Add((JToken)x); }
        public override void AddRange(IEnumerable<JToken> o) { Elements.AddRange(o); }
        public override void AddRange<T>(IEnumerable<T> values) { foreach (dynamic x in values) Elements.Add((JToken)x); }
        public void RemoveAt(int index) { Elements.RemoveAt(index); }
        public void RemoveRange(int index, int count) { Elements.RemoveRange(index, count); }
        public override void Clear() { Elements.Clear(); }
        public JToken Find(System.Predicate<JToken> predicate) { return Elements.Find(predicate); }
        public T Find<T>(System.Predicate<JToken> predicate) { Object r = Elements.Find(predicate); return (T)r; }
        public List<string> String() { return Elements.ConvertAll<string>((o) => { return o.TokenType == TType.String ? ((string)o.Value) : null; }); }
        public List<bool> Bool() { return Elements.ConvertAll<bool>((o) => { return (bool)o.Value; }); }
        public List<int> Int() { return Elements.ConvertAll<int>((o) => { return (int)((long)o.Value); }); }
        public List<long> Long() { return Elements.ConvertAll<long>((o) => { return ((long)o.Value); }); }
        public List<float> Float() { return Elements.ConvertAll<float>((o) => { return ((float)o.Value); }); }
        public List<double> Double() { return Elements.ConvertAll<double>((o) => { return ((double)o.Value); }); }
        public List<DateTime> DateTime() { return Elements.ConvertAll<DateTime>((o) => { return ((DateTime)o); }); }
        public new static JArray Parse(string text, ParseOptions flags = ParseOptions.None)
        {
            var res = JToken.Parse(text, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JArray))
                throw new JsonException("Parse is not returning a JArray");
            return res as JArray;
        }
        public new static JArray ParseThrowCommaEOL(string text)
        {
            var res = JToken.Parse(text, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.ThrowOnError);
            if (!(res is JArray))
                throw new JsonException("Parse is not returning a JArray");
            return res as JArray;
        }
        public new static JArray Parse(string text, out string error, ParseOptions flags)
        {
            var res = JToken.Parse(text, out error, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JArray))
                throw new JsonException("Parse is not returning a JArray");
            return res as JArray;
        }
        internal override IEnumerator<JToken> GetSubClassTokenEnumerator() { return Elements.GetEnumerator(); }
        internal override IEnumerator GetSubClassEnumerator() { return Elements.GetEnumerator(); }
        private List<JToken> Elements { get; set; }
    }
}
namespace QuickJSON
{
    public sealed class JsonIgnoreAttribute : Attribute
    {
        public class SetSetting
        {
            public string Set { get; set; }
            public string[] Ignore { get; set; }
            public string[] IncludeOnly { get; set; }
            public SetSetting() { }
            public SetSetting(string set) { Set = set; }
            public SetSetting(string set, Operation ignoreorinclude, string[] names)
            {
                Set = set;
                if (ignoreorinclude == Operation.Include) IncludeOnly = names; else Ignore = names;
            }
        }
        public SetSetting[] Setting { get; set; }
        public JsonIgnoreAttribute() { Setting = null; }
        public JsonIgnoreAttribute(params string[] setnames)
        {
            Setting = new SetSetting[setnames.Length];
            for (int i = 0; i < setnames.Length; i++)
                Setting[i] = new SetSetting(setnames[i]);
        }
        public enum Operation
        {
            Ignore,
            Include
        };
        public JsonIgnoreAttribute(Operation ignoreorinclude, params string[] names)
        {
            Setting = new SetSetting[1] { new SetSetting(null, ignoreorinclude, names) };
        }
        public JsonIgnoreAttribute(string setname1, Operation ignoreorinclude1, string[] names1,
            string setname2, Operation ignoreorinclude2, string[] names2)
        {
            Setting = new SetSetting[2] { new SetSetting(setname1, ignoreorinclude1, names1),
                new SetSetting(setname2, ignoreorinclude2, names2)
            };
        }
        public JsonIgnoreAttribute(string setname1, Operation ignoreorinclude1, string[] names1,
            string setname2, Operation ignoreorinclude2, string[] names2,
            string setname3, Operation ignoreorinclude3, string[] names3)
        {
            Setting = new SetSetting[3] { new SetSetting(setname1, ignoreorinclude1, names1),
                new SetSetting(setname2, ignoreorinclude2, names2), new SetSetting(setname3, ignoreorinclude3, names3)
            };
        }
    }
    public sealed class JsonNameAttribute : Attribute
    {
        public string[] Sets { get; set; }
        public string[] Names { get; set; }
        public JsonNameAttribute(params string[] names) { Names = names; Sets = null; }
        public JsonNameAttribute(string[] sets, string[] names) { Names = names; Sets = sets; System.Diagnostics.Trace.Assert(Names.Length == Sets.Length); }
    }
    public sealed class JsonIgnoreIfNullAttribute : Attribute
    {
        public string[] Sets { get; set; }
        public JsonIgnoreIfNullAttribute() { Sets = null; }
        public JsonIgnoreIfNullAttribute(params string[] setnames) { Sets = setnames; }
    }
    public sealed class JsonCustomFormat : Attribute
    {
        public string[] Sets { get; set; }
        public JsonCustomFormat() { Sets = null; }
        public JsonCustomFormat(params string[] setnames) { Sets = setnames; }
    }
    public sealed class JsonCustomFormatArray : Attribute
    {
        public string[] Sets { get; set; }
        public JsonCustomFormatArray() { Sets = null; }
        public JsonCustomFormatArray(params string[] setnames) { Sets = setnames; }
    }
    public sealed class JsonCustomArrayLength : Attribute
    {
        public string[] Sets { get; set; }
        public int[] MinimumLength { get; set; }
        public JsonCustomArrayLength(int minlen) { Sets = null; MinimumLength = new int[1] { minlen }; }
        public JsonCustomArrayLength(string[] setnames, int[] lengths) { Sets = setnames; MinimumLength = lengths; }
    }
    public sealed class JsonAlwaysCreate : Attribute
    {
        public string[] Sets { get; set; }
        public JsonAlwaysCreate() { Sets = null; }
        public JsonAlwaysCreate(string[] sets) { Sets = sets; }
    }
}
namespace QuickJSON
{
    public partial class JToken
    {
        public bool DeepEquals(JToken other)
        {
            switch (TokenType)
            {
                case TType.Array:
                    {
                        JArray us = (JArray)this;
                        if (other.TokenType == TType.Array)
                        {
                            JArray ot = (JArray)other;
                            if (ot.Count == us.Count)
                            {
                                for (int i = 0; i < us.Count; i++)
                                {
                                    if (!us[i].DeepEquals(other[i]))
                                        return false;
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                case TType.Object:
                    {
                        JObject us = (JObject)this;
                        if (other.TokenType == TType.Object)
                        {
                            JObject ot = (JObject)other;
                            if (ot.Count == us.Count)
                            {
                                foreach (var kvp in us)
                                {
                                    if (!ot.Contains(kvp.Key))
                                    {
                                        return false;
                                    }
                                    else
                                    {
                                        if (!kvp.Value.DeepEquals(ot[kvp.Key]))       // order unimportant to kvp)
                                            return false;
                                    }
                                }
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                default:
                    if (this.TokenType == TType.Double || other.TokenType == TType.Double)          // either is double
                    {
                        double? usd = (double?)this;          // try and convert us to double
                        double? otherd = (double?)other;      // try and convert the other to double
                        if (usd != null && otherd != null)          // if we could, compare
                        {
                            const double epsilon = 2.2204460492503131E-12;                  // picked to be less than std E print range (14 digits).
                            bool equals = usd.Value.ApproxEquals(otherd.Value, epsilon);
                            return equals;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (this.TokenType == TType.Null || other.TokenType == TType.Null)    // if either is null
                    {
                        return this.TokenType == other.TokenType;       // if both are the same, ie. null, its true
                    }
                    else if (other.TokenType == this.TokenType)         // if both the same token type, use Equals (int, string, boolean)
                    {
                        bool equals = this.Value.Equals(other.Value);
                        return equals;
                    }
                    else if (this.TokenType == TType.Boolean || other.TokenType == TType.Boolean)          // either is boolean
                    {
                        bool? usb = (bool?)this;              // try and convert us to bool
                        bool? otherb = (bool?)other;          // try and convert the other to bool
                        if (usb != null && otherb != null)          // if we could, compare
                        {
                            bool equals = usb.Value == otherb.Value;
                            return equals;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
            }
        }
        static public bool DeepEquals(JToken left, JToken right)
        {
            return left != null && right != null && left.DeepEquals(right);
        }
    }
}
namespace QuickJSON
{
    public static class JTokenExtensionsGet
    {
        public static JToken I(this JToken token, object id)           // safe [] allowing previous to be null
        {
            return token != null ? token[id] : null;
        }
        public static bool IsNull(this JToken token)
        {
            return token == null || token.IsNull;
        }
        public static string MultiStr(this JObject token, string[] propertynameslist, string def = "")
        {
            JToken t = token?.Contains(propertynameslist);
            return t != null && t.IsString ? (string)t.Value : def;
        }
        public static string MultiStr(this JObject token, string def, params string[] propertynameslist)
        {
            JToken t = token?.Contains(propertynameslist);
            return t != null && t.IsString ? (string)t.Value : def;
        }
        public static string Str(this JToken token, string def = "")       // if not string, or null, return def.
        {
            return token != null ? ((string)token ?? def) : def;
        }
        public static string StrNull(this JToken token)
        {
            return token != null ? (string)token : null;
        }
        public static T Enum<T>(this JToken token, T def)
        {
            if (token != null && token.IsLong)
            {
                var i = (int)(long)token.Value;
                return (T)System.Enum.ToObject(typeof(T), i);
            }
            else
                return def;
        }
        public static T EnumStr<T>(this JToken token, T def, bool ignorecase = true) where T : struct
        {
            if (token != null && token.IsString)
            {
                string s = (string)token.Value;
                if (System.Enum.TryParse(s, ignorecase, out T result))
                {
                    return result;
                }
            }
            return def;
        }
        public static int Int(this JToken token, int def = 0)
        {
            if (token != null)
                return (int?)token ?? def;
            else
                return def;
        }
        public static int? IntNull(this JToken token)
        {
            return token != null ? (int?)token : null;
        }
        public static bool TryGetInt(this JToken token, out int value)
        {
            if (token != null)
            {
                int? res = (int?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        public static uint UInt(this JToken token, uint def = 0)
        {
            if (token != null)
                return (uint?)token ?? def;
            else
                return def;
        }
        public static uint? UIntNull(this JToken token)
        {
            return token != null ? (uint?)token : null;
        }
        public static bool TryGetUInt(this JToken token, out uint value)
        {
            if (token != null)
            {
                uint? res = (uint?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        public static long Long(this JToken token, long def = 0)
        {
            if (token != null)
                return (long?)token ?? def;
            else
                return def;
        }
        public static long? LongNull(this JToken token)
        {
            return token != null ? (long?)token : null;
        }
        public static bool TryGetLong(this JToken token, out long value)
        {
            if (token != null)
            {
                long? res = (long?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        public static ulong ULong(this JToken token, ulong def = 0)
        {
            if (token != null)
                return (ulong?)token ?? def;
            else
                return def;
        }
        public static ulong? ULongNull(this JToken token)
        {
            return token != null ? (ulong?)token : null;
        }
        public static bool TryGetULong(this JToken token, out ulong value)
        {
            if (token != null)
            {
                ulong? res = (ulong?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        public static double Double(this JToken token, double def = 0)
        {
            if (token != null)
                return (double?)token ?? def;
            else
                return def;
        }
        public static double Double(this JToken token, double scale, double def)
        {
            if (token != null)
            {
                double? v = (double?)token;
                if (v != null)
                    return v.Value * scale;
            }
            return def;
        }
        public static double? DoubleNull(this JToken token)
        {
            return token != null ? (double?)token : null;
        }
        public static double? DoubleNull(this JToken token, double scale)
        {
            if (token != null)
            {
                double? v = (double?)token;
                if (v != null)
                    return v.Value * scale;
            }
            return null;
        }
        public static bool TryGetDouble(this JToken token, out double value)
        {
            if (token != null)
            {
                double? res = (double?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }
        public static float Float(this JToken token, float def = 0)
        {
            if (token != null)
                return (float?)token ?? def;
            else
                return def;
        }
        public static float Float(this JToken token, float scale, float def)
        {
            if (token != null)
            {
                float? v = (float?)token;
                if (v != null)
                    return v.Value * scale;
            }
            return def;
        }
        public static float? FloatNull(this JToken token)
        {
            return token != null ? (float?)token : null;
        }
        public static float? FloatNull(this JToken token, float scale)
        {
            if (token != null)
            {
                float? v = (float?)token;
                if (v != null)
                    return v.Value * scale;
            }
            return null;
        }
        public static bool TryGetFloat(this JToken token, out float value)
        {
            if (token != null)
            {
                float? res = (float?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = 0;
            return false;
        }
#if JSONBIGINT
        public static System.Numerics.BigInteger BigInteger(this JToken token, System.Numerics.BigInteger def)
        {
            if (token == null)
                return def;
            else if (token.TokenType == JToken.TType.ULong)
                return (ulong)token.Value;
            else if (token.IsLong )
                return (long)token.Value;
            else if (token.TokenType == JToken.TType.BigInt)
                return (System.Numerics.BigInteger)token.Value;
            else
                return def;
        }
        public static System.Numerics.BigInteger? BigIntegerNull(this JToken token)
        {
            if (token == null)
                return null;
            else if (token.TokenType == JToken.TType.ULong)
                return (ulong)token.Value;
            else if (token.IsLong )
                return (long)token.Value;
            else if (token.TokenType == JToken.TType.BigInt)
                return (System.Numerics.BigInteger)token.Value;
            else
                return null;
        }
#endif
        public static bool Bool(this JToken token, bool def = false)
        {
            if (token != null)
                return (bool?)token ?? def;
            else
                return def;
        }
        public static bool? BoolNull(this JToken token)
        {
            return token != null ? (bool?)token : null;
        }
        public static bool TryGetBool(this JToken token, out bool value)
        {
            if (token != null)
            {
                bool? res = (bool?)token;
                if (res.HasValue)
                {
                    value = res.Value;
                    return true;
                }
            }
            value = false;
            return false;
        }
        public static DateTime? DateTime(this JToken token, System.Globalization.CultureInfo cultureinfo, System.Globalization.DateTimeStyles datetimestyle = System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, cultureinfo, datetimestyle, out DateTime ret))
                return ret;
            else
                return null;
        }
        public static DateTime DateTime(this JToken token, DateTime def, System.Globalization.CultureInfo cultureinfo, System.Globalization.DateTimeStyles datetimestyle = System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, cultureinfo, datetimestyle, out DateTime ret))
                return ret;
            else
                return def;
        }
        public static DateTime DateTimeUTC(this JToken token)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                return ret;
            else
                return new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);        //Minvalue in utc mode
        }
        public static DateTime DateTimeUTC(this JToken token, DateTime def)
        {
            if (token != null && token.IsString && System.DateTime.TryParse((string)token.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                return ret;
            else
                return def.ToUniversalTime();
        }
        public static T Enumeration<T>(this JToken token, T def, Func<string, string> preprocess = null)
        {
            if (token != null && token.IsString)
            {
                try
                {
                    string v = (string)token.Value;
                    if (preprocess != null)
                        v = preprocess(v);
                    return (T)System.Enum.Parse(typeof(T), v, true);
                }
                catch
                {
                }
            }
            return def;
        }
        public static JArray Array(this JToken token)       // null if not
        {
            return token as JArray;
        }
        public static JObject Object(this JToken token)     // null if not
        {
            return token as JObject;
        }
    }
}
namespace QuickJSON
{
    public static class JTokenExtensionsGetSet
    {
        public static T GetSet<T>(this JToken token, string name, T defaultvalue)
        {
            if (token.IsObject)
            {
                var tk = token.Object().I(name);

                if (tk != null)         // if value is there
                {
                    try
                    {
                        if (tk.IsLong)
                        {
                            dynamic v = (long)tk.Value;     // T may be another numeric type, so we need to give it assistance (say int)
                            return (T)v;
                        }
                        else if (tk.IsULong)
                        {
                            dynamic v = (ulong)tk.Value;     // T may be another numeric type, so we need to give it assistance
                            return (T)v;
                        }
                        else if (tk.IsDouble)
                        {
                            dynamic v = (double)tk.Value;     // T may be another float type, so we need to give it assistance
                            return (T)v;
                        }
                        return (T)tk.Value;                   // get it, implicit converstion to this type, if fails it will except and we will overwrite
                    }
                    catch
                    {
                    }
                }
                token[name] = JToken.CreateToken(defaultvalue, true);
            }
            else
            {
                throw new JToken.JsonException("GetSet on a token which is not a JObject");
            }
            return defaultvalue;
        }
    }
}
namespace QuickJSON
{
    public static class JTokenExtensionsOther
    {
        public static JObject RenameObjectFields(this JToken jobject, string pattern, string replace, bool startswith = false)
        {
            JObject o = jobject.Object();
            if (o != null)
            {
                JObject ret = new JObject();
                foreach (var kvp in o)
                {
                    string name = kvp.Key;
                    if (startswith)
                    {
                        if (name.StartsWith(pattern))
                            name = replace + name.Substring(pattern.Length);
                    }
                    else
                    {
                        name = name.Replace(pattern, replace);
                    }
                    ret[name] = kvp.Value;
                }
                return ret;
            }
            else
                return null;
        }
        public static JObject RenameObjectFieldsUnderscores(this JToken jo)
        {
            return jo.RenameObjectFields("_", "");
        }
        public static JObject RemoveObjectFieldsKeyPrefix(this JToken jo, string prefix)
        {
            return jo.RenameObjectFields(prefix, "", true);
        }
        public static JToken JSONParse(this string text, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            if (text != null)
                return JToken.Parse(text, flags);
            else
                return null;
        }
        public static JObject JSONParseObject(this string text, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            if (text != null)
                return JObject.Parse(text, flags);
            else
                return null;
        }
        public static JArray JSONParseArray(this string text, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            if (text != null)
                return JArray.Parse(text, flags);
            else
                return null;
        }
        public static JToken ReadJSONFile(this string filepath, JToken.ParseOptions flags = JToken.ParseOptions.None)
        {
            try
            {
                if (System.IO.File.Exists(filepath))
                {
                    string ftext = System.IO.File.ReadAllText(filepath);
                    return JToken.Parse(ftext, flags);
                }
            }
            catch
            {
            }
            return null;
        }
        public static bool WriteJSONFile(this JToken jo, string path, bool verbose = false)
        {
            try
            {
                string text = jo.ToString(verbose);
                System.IO.File.WriteAllText(path, text);
                return true;
            }
            catch
            {
            }
            return false;
        }
        public static JToken ReadJSONAndConvertBack(this string jsontext, JToken.ParseOptions flags = JToken.ParseOptions.None, bool verbose = true)
        {
            JToken tk = JToken.Parse(jsontext, flags);
            return tk?.ToString(verbose);
        }
        public static JObject CreateObjectTree(this JToken token, params string[] names)
        {
            if (token.IsObject)
            {
                foreach (string s in names)
                {
                    if (token.Contains(s))
                    {
                        if (!token.IsObject)
                        {
                            throw new JToken.JsonException($"CreateObjects failed on {s} due to it not being an object");
                        }
                        token = token[s];                       // get the object, jump to it
                    }
                    else
                    {
                        token = token[s] = new JObject();       // make an object, jump to it
                    }
                }
                return token.Object();
            }
            else
            {
                throw new JToken.JsonException("CreateObjects on a token which is not a JObject");
            }
        }
    }
}
namespace QuickJSON
{
    public partial class JToken
    {
        public static JToken FromObject(Object obj)
        {
            return FromObject(obj, false);
        }
        public static JToken FromObject(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256,
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false, string setname = null, Func<MemberInfo, Object, JToken> customconverter = null,
            Func<Type, Type, object, string> process = null)
        {
            FromObjectConverter cnv = new FromObjectConverter(ignoreunserialisable, ignored, maxrecursiondepth, membersearchflags, ignoreobjectpropertyifnull, setname, process, customconverter);
            var r = cnv.Execute(obj, 0, null, null, null);
            System.Diagnostics.Debug.Assert(cnv.ObjectCount == 0);
            return r.IsInError ? null : r;
        }
        public static JToken FromObjectWithError(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256,
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            bool ignoreobjectpropertyifnull = false, string setname = null,
            Func<MemberInfo, Object, JToken> customconverter = null,
            Func<Type, Type, object, string> process = null)
        {
            FromObjectConverter cnv = new FromObjectConverter(ignoreunserialisable, ignored, maxrecursiondepth, membersearchflags, ignoreobjectpropertyifnull, setname, process, customconverter);
            var r = cnv.Execute(obj, 0, null, null, null);
            System.Diagnostics.Debug.Assert(cnv.ObjectCount == 0);
            return r;
        }
        [System.Diagnostics.DebuggerDisplay("{MemberInfo.Name}->{Name} cf:{CustomFormat} im:{IgnoreMemberIfNull}")]
        public class MemberAttributeSettings
        {
            public MemberInfo MemberInfo;
            public string Name { get; set; }
            public bool UseCustomConverter { get; set; }
            public bool UseCustomConverterOnArrayElements { get; set; }
            public MethodInfo ToJTokenConvert { get; set; }
            public bool IgnoreMemberIfNull { get; set; }
            public HashSet<string> IncludeSet { get; set; }
            public HashSet<string> IgnoreSet { get; set; }
        }
        public static void ClearFromObjectCache()
        {
            FromObjectConverter.CacheOfTypesFromMembers.Clear();
        }
        static public Dictionary<string, JToken.MemberAttributeSettings> GetMemberAttributeSettings(Type tt, string setname = null, System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
        {
            var ret = FromObjectConverter.GetOrCreate(tt, setname, membersearchflags);
            return ret;
        }
    }
    internal class FromObjectConverter
    {
        public static Dictionary<Tuple<string, Type, BindingFlags>, Dictionary<string, JToken.MemberAttributeSettings>> CacheOfTypesFromMembers
                    = new Dictionary<Tuple<string, Type, BindingFlags>, Dictionary<string, JToken.MemberAttributeSettings>>();
        private bool ignoreunserialisable;
        private Type[] ignoredtypes;
        private int maxrecursiondepth;
        private BindingFlags membersearchflags;
        private bool ignoreobjectpropertyifnull;
        private string setname;
        Func<MemberInfo, Object, JToken> customconvert;
        private Func<Type, Type, object, string> processenumdatetime;
        private Stack<Object> objectlist;   // holds the objects converted to check for recursion
        public int ObjectCount => objectlist.Count();
        public FromObjectConverter(bool ignoreunserialisable, Type[] ignoredtypes, int maxrecursiondepth,
                                    BindingFlags membersearchflags, bool ignoreobjectpropertyifnull, string setname,
                                    Func<Type, Type, object, string> process,
                                    Func<MemberInfo, Object, JToken> customconvert)
        {
            this.ignoreunserialisable = ignoreunserialisable;
            this.ignoredtypes = ignoredtypes;
            this.maxrecursiondepth = maxrecursiondepth;
            this.membersearchflags = membersearchflags;
            this.ignoreobjectpropertyifnull = ignoreobjectpropertyifnull;
            this.setname = setname;
            this.customconvert = customconvert;
            this.processenumdatetime = process;
            this.objectlist = new Stack<object>();
        }
        public JToken Execute(Object o, int lvl, HashSet<string> memberignore, HashSet<string> memberinclude, MemberInfo forcecustomconvertwithmi = null, MethodInfo forceToJToken = null)
        {
            if (lvl >= maxrecursiondepth)
                return new JToken();        // returns NULL
            Type tt = o.GetType();
            if (tt.IsDictionary())       // if its a Dictionary<x,y> then expect a set of objects
            {
                System.Collections.IDictionary idict = o as System.Collections.IDictionary;
                var types = tt.GetGenericArguments();      // types[0] will be key, types[1] is value
                bool keyisenum = types[0].IsEnum;          // if enum, we will process they key thru either processenumdatetime or via a parse
                bool keyisdatetime = !keyisenum && types[0].Name.Equals("DateTime");
                JObject outobj = new JObject();
                objectlist.Push(o);
                foreach (dynamic kvp in idict)      // need dynamic since don't know the types of Value or Key
                {
                    if (objectlist.Contains(kvp.Value))   // self reference
                    {
                        if (ignoreunserialisable)       // if ignoring this, just continue with the next one
                            continue;
                        objectlist.Pop();
                        return new JToken(JToken.TType.Error, "Self Reference in IDictionary");
                    }
                    JToken inner = Execute(kvp.Value, lvl + 1, null, null, forcecustomconvertwithmi, forceToJToken);
                    if (inner.IsInError)      // used as an error type
                    {
                        objectlist.Pop();
                        return inner;
                    }
                    if (keyisdatetime)
                    {
                        DateTime t = (DateTime)kvp.Key;
                        if (processenumdatetime != null)
                        {
                            string str = processenumdatetime(types[0], tt, kvp.Key);
                            outobj[str] = inner;
                        }
                        else
                            outobj[t.ToStringZulu()] = inner;
                    }
                    else if (keyisenum)
                    {
                        if (processenumdatetime != null)
                        {
                            string str = processenumdatetime(types[0], tt, kvp.Key);
                            outobj[str] = inner;
                        }
                        else
                        {
                            outobj[kvp.Key.ToString()] = inner;
                        }
                    }
                    else
                        outobj[kvp.Key.ToString()] = inner;
                }
                objectlist.Pop();
                return outobj;
            }
            else if (o is string)           // strings look like classes and IEnumerables, so intercept here
            {
                var r = forcecustomconvertwithmi != null ? customconvert(forcecustomconvertwithmi, o) : JToken.CreateToken(o, false);
                return r ?? new JToken(JToken.TType.Error, "Unserializable " + tt.Name);
            }
            else if (tt.IsEnumerable())       // enumerables, arrays, lists, hashsets
            {
                var ilist = o as System.Collections.IEnumerable;
                JArray outarray = new JArray();
                objectlist.Push(o);
                foreach (var oa in ilist)
                {
                    if (objectlist.Contains(oa))        // self reference
                    {
                        objectlist.Pop();
                        return new JToken(JToken.TType.Error, "Self Reference in IEnumerable");
                    }
                    JToken inner = Execute(oa, lvl + 1, null, null, forcecustomconvertwithmi, forceToJToken);
                    if (inner.IsInError)      // used as an error type
                    {
                        objectlist.Pop();
                        return inner;
                    }
                    outarray.Add(inner);
                }
                objectlist.Pop();
                return outarray;
            }
            else if (forcecustomconvertwithmi != null)           // else its an object, not an array, not a string, and if we have forced a custom convert on it, do it
            {
                return customconvert(forcecustomconvertwithmi, o);
            }
            else if (forceToJToken != null)           // else its an object, not an array, not a string, and if we have forced a custom convert on it, do it
            {
                return (JToken)forceToJToken.Invoke(o, new object[] { });
            }
            else if (tt.IsEnum)
            {
                if (processenumdatetime != null)
                {
                    return new JToken(processenumdatetime(tt, null, o));       // convert to string, JToken it
                }
                else
                {
                    return new JToken(o.ToString());                    // just tostring it
                }
            }
            else if (tt.Name.Equals("DateTime"))
            {
                if (processenumdatetime != null)
                {
                    return new JToken(processenumdatetime(tt, null, o));       // convert to string, JToken it
                }
                else
                {
                    return new JToken(((DateTime)o).ToStringZulu());    // zulu it
                }
            }
            else if (tt.IsClass || (tt.IsValueType && !tt.IsPrimitive))     // if class or non primitive value type (struct)
            {
                JObject outobj = new JObject();
                objectlist.Push(o);
                Dictionary<string, JToken.MemberAttributeSettings> namestosettings = GetOrCreate(tt, setname, membersearchflags);
                foreach (var kvp in namestosettings)  // for all active settings
                {
                    JToken.MemberAttributeSettings actrl = kvp.Value;
                    var mi = actrl.MemberInfo;
                    Type innertype;
                    if (mi.MemberType == System.Reflection.MemberTypes.Property)        // only properties and fields are allowed thru
                    {
                        innertype = ((System.Reflection.PropertyInfo)mi).PropertyType;
                    }
                    else if (mi.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        var fi = (System.Reflection.FieldInfo)mi;
                        innertype = fi.FieldType;
                    }
                    else
                        continue;       // not a prop/field
                    string attrname = mi.Name;
                    if ((memberinclude != null && !memberinclude.Contains(attrname)) ||
                            (memberignore != null && memberignore.Contains(attrname)))
                    {
                        continue;
                    }
                    if (ignoredtypes != null && Array.IndexOf(ignoredtypes, innertype) >= 0)
                        continue;
                    Object innervalue = null;
                    try
                    {
                        if (mi.MemberType == System.Reflection.MemberTypes.Property)
                        {
                            var pi = (System.Reflection.PropertyInfo)mi;
                            if (pi.GetIndexParameters().Length == 0)       // reject any indexer properties! new Dec 22
                            {
                                innervalue = pi.GetValue(o);
                            }
                        }
                        else
                            innervalue = ((System.Reflection.FieldInfo)mi).GetValue(o);
                    }
                    catch (Exception ex)
                    {
                        objectlist.Pop();
                        return new JToken(JToken.TType.Error, $"QuickJSON FromObject Unable to Get value of {mi.Name} due to {ex}");
                    }
                    if (innervalue != null)
                    {
                        if (objectlist.Contains(innervalue))        // self reference
                        {
                            if (ignoreunserialisable)               // if ignoring this, just continue with the next one
                                continue;
                            objectlist.Pop();
                            return new JToken(JToken.TType.Error, "Self Reference by " + tt.Name + ":" + mi.Name);
                        }
                        JToken token = null;
                        if (actrl.UseCustomConverter)        // custom format tag on item overrides everything
                        {
                            token = customconvert(actrl.MemberInfo, innervalue);
                        }
                        else if (innertype.IsEnumerable())        // arrays/lists
                        {
                            token = Execute(innervalue, lvl + 1, actrl.IgnoreSet, actrl.IncludeSet, actrl.UseCustomConverterOnArrayElements ? actrl.MemberInfo : null, actrl.ToJTokenConvert);
                        }
                        else if (actrl.ToJTokenConvert != null)
                        {
                            token = (JToken)actrl.ToJTokenConvert.Invoke(innervalue, new object[] { });
                        }
                        else
                        {
                            token = Execute(innervalue, lvl + 1, actrl.IgnoreSet, actrl.IncludeSet);// actrl.CustomFormatArray ? actrl.MemberInfo : null);
                        }
                        if (token.IsInError)
                        {
                            if (!ignoreunserialisable)
                            {
                                objectlist.Pop();
                                return token;
                            }
                        }
                        else
                            outobj[actrl.Name] = token;
                    }
                    else
                    {
                        bool ignoreifnull = ignoreobjectpropertyifnull || actrl.IgnoreMemberIfNull;
                        if (ignoreifnull == false)
                            outobj[actrl.Name] = JToken.Null();        // its null so its a JNull
                    }
                }
                objectlist.Pop();
                return outobj;
            }
            else
            {
                var r = JToken.CreateToken(o, false);        // return token or null indicating unserializable
                return r ?? new JToken(JToken.TType.Error, "Unserializable " + tt.Name);
            }
        }
        public static Dictionary<string, JToken.MemberAttributeSettings> GetOrCreate(Type classtype, string setname, BindingFlags membersearchflags)
        {
            lock (CacheOfTypesFromMembers)        // thread lock
            {
                var key = new Tuple<string, Type, BindingFlags>(setname, classtype, membersearchflags);
                if (!CacheOfTypesFromMembers.TryGetValue(key, out Dictionary<string, JToken.MemberAttributeSettings> namestosettings))
                {
                    namestosettings = new Dictionary<string, JToken.MemberAttributeSettings>();
                    CacheOfTypesFromMembers[key] = namestosettings;
                    var allmembers = classtype.GetMembers(membersearchflags);
                    foreach (var mi in allmembers)
                    {
                        if (mi.MemberType == System.Reflection.MemberTypes.Property || mi.MemberType == System.Reflection.MemberTypes.Field)
                        {
                            var ac = new JToken.MemberAttributeSettings() { Name = mi.Name, MemberInfo = mi };
                            bool ignore = false;
                            var calist = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                            if (calist.Length == 1)
                            {
                                JsonIgnoreAttribute jia = calist[0] as JsonIgnoreAttribute;
                                if (jia.Setting == null)       // null, means all sets
                                {
                                    ignore = true;
                                }
                                else
                                {
                                    for (int i = 0; i < jia.Setting.Length; i++)
                                    {
                                        if (jia.Setting[i].Set.EqualsI(setname))  // else we match on set name
                                        {
                                            if (jia.Setting[i].Ignore != null)
                                                ac.IgnoreSet = jia.Setting[i].Ignore.ToHashSet();
                                            else if (jia.Setting[i].IncludeOnly != null)
                                                ac.IncludeSet = jia.Setting[i].IncludeOnly.ToHashSet();
                                            else
                                                ignore = true;
                                            break;
                                        }
                                    }
                                }
                            }
                            if (!ignore)    // if we ignore it, just don't store
                            {
                                {
                                    Type otype = mi.FieldPropertyType();        // new JULY 26 see if we have a custom converter ToJToken()
                                    if (otype.IsArray)  // set the ConstructorInfo of an array to the underlying type. This is passed from the array down
                                    {
                                        otype = otype.GetElementType();
                                    }
                                    else if (otype == typeof(string))      // because it looks like an ienumarable
                                    {
                                    }
                                    else if (otype.IsDictionary())      // enumerables, use generic type arg [0] to pick up underlying type (best I could find)
                                    {
                                        var genargs = otype.GetGenericArguments();
                                        if (genargs.Length == 2)
                                            otype = genargs[1];     // Key, Value, we want value
                                        else
                                            otype = null;
                                    }
                                    else if (otype.IsEnumerable())      // enumerables, use generic type arg [0] to pick up underlying type (best I could find)
                                    {
                                        var genargs = otype.GetGenericArguments();
                                        if (genargs.Length == 1)
                                            otype = genargs[0];
                                        else
                                            otype = null;
                                    }
                                    if (otype != null)
                                    {
                                        ac.ToJTokenConvert = (MethodInfo)otype.GetMembers(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name.Equals("ToJToken", StringComparison.CurrentCultureIgnoreCase)
                                                                   && x.MemberType == MemberTypes.Method && ((MethodInfo)x).ReturnType == typeof(QuickJSON.JToken) && ((MethodInfo)x).GetParameters().Length == 0).FirstOrDefault();
                                    }
                                }
                                var renamelist = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                                if (renamelist.Length == 1)
                                {
                                    JsonNameAttribute na = renamelist[0] as JsonNameAttribute;
                                    if (na.Sets == null)        // null, means all sets
                                    {
                                        ac.Name = na.Names[0];
                                    }
                                    else
                                    {
                                        for (int i = 0; i < na.Sets.Length; i++)        // find set, if not found, no change, else first one gives the name
                                        {
                                            if (na.Sets[i].EqualsI(setname))
                                            {
                                                ac.Name = na.Names[i];
                                                break;
                                            }
                                        }
                                    }
                                }
                                var ignorenulllist = mi.GetCustomAttributes(typeof(JsonIgnoreIfNullAttribute), false);
                                if (ignorenulllist.Length == 1)
                                {
                                    JsonIgnoreIfNullAttribute iin = ignorenulllist[0] as JsonIgnoreIfNullAttribute;
                                    if (iin.Sets == null)       // null, means all sets
                                    {
                                        ac.IgnoreMemberIfNull = true;
                                    }
                                    else
                                    {
                                        for (int i = 0; i < iin.Sets.Length; i++)  // find set, if not found, not applicable. Else its ignore if null
                                        {
                                            if (iin.Sets[i].EqualsI(setname))
                                            {
                                                ac.IgnoreMemberIfNull = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                                var customoutput = mi.GetCustomAttributes(typeof(JsonCustomFormat), false);
                                if (customoutput.Length == 1)
                                {
                                    JsonCustomFormat jcf = customoutput[0] as JsonCustomFormat;
                                    if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                    {
                                        ac.UseCustomConverter = true;
                                    }
                                }
                                var customoutputarray = mi.GetCustomAttributes(typeof(JsonCustomFormatArray), false);
                                if (customoutputarray.Length == 1)
                                {
                                    JsonCustomFormatArray jcf = customoutputarray[0] as JsonCustomFormatArray;
                                    if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                    {
                                        ac.UseCustomConverterOnArrayElements = true;
                                    }
                                }
                                namestosettings[mi.Name] = ac;      // and store, we will process this
                            }
                            else
                            {
                            }
                        }
                    }
                }
                else
                {
                }
                return namestosettings;
            }
        }
    }
}
namespace QuickJSON
{
    public partial class JObject : JToken, IEnumerable<KeyValuePair<string, JToken>>
    {
        public JObject()
        {
            TokenType = TType.Object;
            Objects = new Dictionary<string, JToken>(16);   // giving a small initial cap seems to help
        }
        public JObject(IDictionary dict) : this()           // convert from a dictionary. Key must be string
        {
            foreach (DictionaryEntry x in dict)
            {
                this.Add((string)x.Key, JToken.CreateToken(x.Value));
            }
        }
        public JObject(JObject other) : this()              // create with deep copy from another object
        {
            foreach (var kvp in other.Objects)
            {
                Objects[kvp.Key] = kvp.Value.Clone();
            }
        }
        public override JToken this[object key]
        {
            get { if (key is string && Objects.TryGetValue((string)key, out JToken v)) return v; else return null; }
            set { System.Diagnostics.Debug.Assert(key is string); Objects[(string)key] = (value == null) ? JToken.Null() : value; }
        }
        public JToken this[string key]
        {
            get { if (Objects.TryGetValue(key, out JToken v)) return v; else return null; }
            set { Objects[key] = (value == null) ? JToken.Null() : value; }
        }
        public override JToken First() { return Objects.First().Value; }
        public override JToken Last() { return Objects.Last().Value; }
        public override JToken FirstOrDefault() { return Objects.Count > 0 ? Objects.First().Value : null; }
        public override JToken LastOrDefault() { return Objects.Count > 0 ? Objects.Last().Value : null; }
        public string[] PropertyNames() { return Objects.Keys.ToArray(); }
        public override bool Contains(string name) { return Objects.ContainsKey(name); }
        public bool ContainsAllOfThese(params string[] name) { return Objects.Where(kvp => name.Contains(kvp.Key)).Count() == name.Length; }
        public int ContainsIndexOf(params string[] name) { for (int i = 0; i < name.Length; i++) { if (Objects.ContainsKey(name[i])) return i; } return -1; }
        public int ContainsIndexOf(out JToken ret, params string[] name) { for (int i = 0; i < name.Length; i++) { if (Objects.ContainsKey(name[i])) { ret = Objects[name[i]]; return i; } } ret = null; return -1; }
        public bool ContainsAnyOf(params string[] name) { for (int i = 0; i < name.Length; i++) { if (Objects.ContainsKey(name[i])) return true; } return false; }
        public IEnumerable<string> UnknownProperties(params string[] name) { return Objects.Where(kvp => !name.Contains(kvp.Key)).Select(kvp => kvp.Key); }
        public IEnumerable<string> UnknownProperties(string[] name, params string[] name2) { return Objects.Where(kvp => !(name.Contains(kvp.Key) || name2.Contains(kvp.Key))).Select(kvp => kvp.Key); }
        public bool TryGetValue(string name, out JToken token) { return Objects.TryGetValue(name, out token); }
        public JToken Contains(string[] ids)
        {
            foreach (string key in ids)
            {
                if (Objects.ContainsKey(key))
                    return Objects[key];
            }
            return null;
        }
        public bool Rename(string fromname, string newname)
        {
            if (Objects.ContainsKey(fromname))
            {
                Objects[newname] = Objects[fromname];
                Objects.Remove(fromname);
                return true;
            }
            else
                return false;
        }

        public override int Count { get { return Objects.Count; } }
        public override void Add(string key, JToken value) { this[key] = value; }
        public override void Add<T>(string key, T o) { dynamic x = o; this[key] = (JToken)x; }
        public void Merge(JObject other, bool allowoverwrite = true)
        {
            foreach (var kvp in other.Objects)
            {
                if (allowoverwrite || !this.Contains(kvp.Key))
                    this[kvp.Key] = kvp.Value;
            }
        }
        public bool Remove(string key) { return Objects.Remove(key); }
        public void Remove(params string[] key) { foreach (var k in key) Objects.Remove(k); }
        public void RemoveWildcard(string wildcard, bool caseinsensitive = false)       // use * ?
        {
            var list = new List<string>();
            foreach (var kvp in Objects)
            {
                if (kvp.Key.WildCardMatch(wildcard, caseinsensitive))
                {
                    list.Add(kvp.Key);
                }
            }
            foreach (var k in list) Objects.Remove(k);
        }
        public override void Clear() { Objects.Clear(); }
        public new IEnumerator<KeyValuePair<string, JToken>> GetEnumerator() { return Objects.GetEnumerator(); }
        public new static JObject Parse(string text, ParseOptions flags = ParseOptions.None)        // null if failed.
        {
            var res = JToken.Parse(text, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JObject))
                throw new JsonException("Parse is not returning a JObject");
            return res as JObject;
        }
        public new static JObject ParseThrowCommaEOL(string text)        // throws if fails, allows trailing commas and checks EOL
        {
            var res = JToken.Parse(text, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.ThrowOnError);
            if (!(res is JObject))
                throw new JsonException("Parse is not returning a JObject");
            return res as JObject;
        }
        public new static JObject Parse(string text, out string error, ParseOptions flags)
        {
            var res = JToken.Parse(text, out error, flags);
            if ((flags & ParseOptions.ThrowOnError) != 0 && !(res is JObject))
                throw new JsonException("Parse is not returning a JObject");
            return res as JObject;
        }
        internal override IEnumerator<JToken> GetSubClassTokenEnumerator() { return Objects.Values.GetEnumerator(); }
        internal override IEnumerator GetSubClassEnumerator() { return Objects.GetEnumerator(); }
        private Dictionary<string, JToken> Objects { get; set; }
    }
}
namespace QuickJSON
{
    public partial class JObject : JToken, IEnumerable<KeyValuePair<string, JToken>>
    {
        public JObject Filter(JObject allowedFields, string path = "")
        {
            JObject ret = new JObject();
            foreach (var kvp in this)
            {
                string mpath = $"{path}.{kvp.Key}";
                if (allowedFields.Contains(kvp.Key))
                {
                    JToken allowedField = allowedFields[kvp.Key];
                    if (kvp.Value.HasValue)
                    {
                        if (allowedField.BoolNull() == true)      // if straight value and allowed
                        {
                            ret[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                        }
                    }                                                               // if Jarray, allowed is Jarray, and one JOBJECT underneath
                    else if (kvp.Value.IsArray && allowedField.IsArray && allowedField.Count == 1 && allowedField[0] is JObject)
                    {
                        JObject allowed = (JObject)allowedField[0];
                        JArray vals = new JArray();
                        foreach (JObject val in kvp.Value)      // go thru array
                        {
                            vals.Add(val.Filter(allowed, $"{mpath}[]"));
                        }
                        ret[kvp.Key] = vals;
                    }
                    else if (kvp.Value.IsArray && allowedField.StrNull() == "[]")     //  if Jarray, and allowed fields is a special [] string marker
                    {
                        JArray vals = new JArray();
                        foreach (JToken val in kvp.Value)       // just add all values
                        {
                            if (val.HasValue)
                            {
                                vals.Add(val);
                            }
                            else
                            {
                                System.Diagnostics.Trace.WriteLine($"Array value {mpath}[] is not a value: {val?.ToString()}");
                            }
                        }
                        ret[kvp.Key] = vals;
                    }
                    else if (kvp.Value.IsObject && allowedField.IsObject)       // if object, and allowed is object
                    {
                        JObject allowed = (JObject)allowedField;
                        JObject val = (JObject)kvp.Value;
                        ret[kvp.Key] = val.Filter(allowed, mpath);     // recurse add
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine($"Object value {mpath} {kvp.Value.TokenType} is not of expected type: {kvp.Value?.ToString()}");
                    }
                }
                else
                {
                }
            }
            return ret;
        }
    }
}
namespace QuickJSON
{
    public partial class JToken
    {
        public class JsonException : System.Exception
        {
            public string Error { get; set; }
            public JsonException(string errortext) { Error = errortext; }
        }
        [Flags]
        public enum ParseOptions
        {
            None = 0,
            AllowTrailingCommas = 1,
            CheckEOL = 2,
            ThrowOnError = 4,
            IgnoreBadObjectValue = 8,
            IgnoreBadArrayValue = 16,
        }
        public static JToken Parse(string text, ParseOptions flags = ParseOptions.None)
        {
            StringParserQuick parser = new StringParserQuick(text);
            return Parse(parser, out string unused, flags, text.Length, defaultstackdepth);
        }
        public static JToken ParseThrow(string text, ParseOptions flags = ParseOptions.None)
        {
            StringParserQuick parser = new StringParserQuick(text);
            return Parse(parser, out string unused, flags | ParseOptions.ThrowOnError, text.Length, defaultstackdepth);
        }
        public static JToken ParseThrowCommaEOL(string text)
        {
            StringParserQuick parser = new StringParserQuick(text);
            return Parse(parser, out string unused, JToken.ParseOptions.AllowTrailingCommas | JToken.ParseOptions.CheckEOL | JToken.ParseOptions.ThrowOnError, text.Length, defaultstackdepth);
        }
        public static JToken Parse(string text, out string error, ParseOptions flags = ParseOptions.None)
        {
            StringParserQuick parser = new StringParserQuick(text);
            return Parse(parser, out error, flags, text.Length, defaultstackdepth);
        }
        public static JToken Parse(System.IO.TextReader trx, out string error, ParseOptions flags = ParseOptions.None, int chunksize = 16384, int charbufsize = 16384)
        {
            StringParserQuickTextReader parser = new StringParserQuickTextReader(trx, chunksize);
            return Parse(parser, out error, flags, charbufsize, defaultstackdepth);
        }
        public static JToken Parse(IStringParserQuick parser, out string error, ParseOptions flags, int charbufsize, int stackdepth)
        {
            char[] textbuffer = new char[charbufsize];      // textbuffer to use for string decodes - one get of it, multiple reuses, faster
            JToken res = IntParse(parser, out error, flags, textbuffer, stackdepth);
            if (res != null && (flags & ParseOptions.CheckEOL) != 0 && !parser.IsEOL())
            {
                return ParseError(parser, "Extra Chars after JSON", flags, out error);
            }
            else
                return res;
        }
        public static JToken Parse(IStringParserQuick parser, out string error, ParseOptions flags, char[] textbuffer, int stackdepth)
        {
            JToken res = IntParse(parser, out error, flags, textbuffer, stackdepth);
            if (res != null && (flags & ParseOptions.CheckEOL) != 0 && !parser.IsEOL())
            {
                return ParseError(parser, "Extra Chars after JSON", flags, out error);
            }
            else
                return res;
        }
        private static JToken IntParse(IStringParserQuick parser, out string error, ParseOptions flags, char[] textbuffer, int stackdepth)
        {
            error = null;
            JToken[] stack = new JToken[stackdepth];
            int sptr = 0;
            bool comma = false;
            JArray curarray = null;
            JObject curobject = null;
            {
                JToken o = parser.JNextValue(textbuffer, false);       // grab new value, not array end
                if (o == null)
                {
                    return ParseError(parser, "No Obj/Array", flags, out error);
                }
                o.Level = sptr;
                if (o.TokenType == TType.Array)
                {
                    stack[++sptr] = o;                      // push this one onto stack
                    curarray = o as JArray;                 // this is now the current array
                }
                else if (o.TokenType == TType.Object)
                {
                    stack[++sptr] = o;                      // push this one onto stack
                    curobject = o as JObject;               // this is now the current object
                }
                else
                {
                    return o;                               // value only
                }
            }
            while (true)
            {
                if (curobject != null)      // if object..
                {
                    int emptynamenumber = 0;        // empty "" names are converted to <!!!EmptyNameN!!!> so they can be dereferenced
                    while (true)
                    {
                        char next = parser.GetChar();
                        if (next == '}')    // end object
                        {
                            if (comma == true && (flags & ParseOptions.AllowTrailingCommas) == 0)
                            {
                                return ParseError(parser, "Comma", flags, out error);
                            }
                            else
                            {
                                parser.SkipSpace();
                                JToken prevtoken = stack[--sptr];
                                if (prevtoken == null)      // if popped stack is null, we are back to beginning, return this
                                {
                                    return stack[sptr + 1];
                                }
                                else
                                {
                                    comma = parser.IsCharMoveOn(',');
                                    curobject = prevtoken as JObject;
                                    if (curobject == null)
                                    {
                                        curarray = prevtoken as JArray;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (next == '"')   // property name
                        {
                            if (comma == false && curobject.Count > 0)
                            {
                                return ParseError(parser, "Missing comma before property name", flags, out error);
                            }
                            int textlen = parser.NextQuotedString(next, textbuffer, true);      // names can be empty
                            if (textlen < 0)
                            {
                                return ParseError(parser, "Object missing property name", flags, out error);
                            }
                            else if (!parser.IsCharMoveOn(':'))
                            {
                                return ParseError(parser, "Object missing : after property name", flags, out error);
                            }
                            else
                            {
                                string originalname = new string(textbuffer, 0, textlen);
                                JToken o = parser.JNextValue(textbuffer, false);      // get value
                                if (o == null)
                                {
                                    if ((flags & ParseOptions.IgnoreBadObjectValue) != 0)       // if we get a bad value, and flag set, try and move to the next start point
                                    {
                                        while (true)
                                        {
                                            char nc = parser.PeekChar();
                                            if (nc == char.MinValue || nc == '"' || nc == '}')  // looking for next property " or } or eol
                                                break;
                                            else
                                                parser.GetChar();
                                        }
                                    }
                                    else
                                    {
                                        return ParseError(parser, "Object bad value", flags, out error);
                                    }
                                }
                                else
                                {
                                    if (originalname.Length == 0)       // if empty name, we give it an internal name, but keep the original for ToString()
                                    {
                                        o.Name = $"!!!EmptyName{emptynamenumber++.ToStringInvariant()}!!!";
                                        o.OriginalName = originalname;
                                    }
                                    else if (curobject.Contains(originalname))          // if its a repeat..
                                    {
                                        int total = 0;
                                        foreach (var kvp in curobject)
                                        {
                                            if (kvp.Key == originalname || (kvp.Key.StartsWith(originalname + "[") && kvp.Key.EndsWith("]")))
                                                total++;    // if we have a repeat increase count
                                        }
                                        o.Name = $"!!!Repeat-{originalname}[{total.ToStringInvariant()}]!!!";
                                        o.OriginalName = originalname;
                                    }
                                    else
                                        o.Name = originalname;                  // we keep originalname null for space reasons and to indicate Name is correct
                                    o.Level = sptr;
                                    curobject[o.Name] = o;                    // assign to dictionary
                                    if (o.TokenType == TType.Array)         // if array, we need to change to this as controlling object on top of stack
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            return ParseError(parser, "Stack overflow", flags, out error);
                                        }
                                        stack[++sptr] = o;                  // push this one onto stack
                                        curarray = o as JArray;             // this is now the current object
                                        curobject = null;
                                        comma = false;
                                        break;
                                    }
                                    else if (o.TokenType == TType.Object)   // if object, this is the controlling object
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            return ParseError(parser, "Stack overflow", flags, out error);
                                        }
                                        stack[++sptr] = o;                  // push this one onto stack
                                        curobject = o as JObject;           // this is now the current object
                                        comma = false;
                                    }
                                    else
                                    {
                                        comma = parser.IsCharMoveOn(',');
                                    }
                                }
                            }
                        }
                        else
                        {
                            return ParseError(parser, "Bad format in object", flags, out error);
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        JToken o = parser.JNextValue(textbuffer, true);       // grab new value
                        if (o == null)
                        {
                            if ((flags & ParseOptions.IgnoreBadArrayValue) != 0)       // if we get a bad value, and flag set, try and move to the next start point
                            {
                                curarray.Add(JToken.Null());                // add a null
                                while (true)
                                {
                                    char nc = parser.PeekChar();
                                    if (nc == char.MinValue || nc == ']')  // looking for EOL or ] - we leave that on the parser, as next time around we want it to read an end array
                                        break;
                                    else if (nc == ',')        // if on comma, waste it, we have our value, go onto next
                                    {
                                        parser.GetChar();
                                        break;
                                    }
                                    else
                                        parser.GetChar();
                                }
                            }
                            else
                            {
                                return ParseError(parser, "Bad array value", flags, out error);
                            }
                        }
                        else if (o.TokenType == TType.EndArray)          // if end marker, jump back
                        {
                            if (comma == true && (flags & ParseOptions.AllowTrailingCommas) == 0)
                            {
                                return ParseError(parser, "Trailing Comma found", flags, out error);
                            }
                            else
                            {
                                parser.SkipSpace();
                                JToken prevtoken = stack[--sptr];
                                if (prevtoken == null)      // if popped stack is null, we are back to beginning, return this
                                {
                                    return stack[sptr + 1];
                                }
                                else
                                {
                                    comma = parser.IsCharMoveOn(',');
                                    curobject = prevtoken as JObject;
                                    if (curobject == null)
                                    {
                                        curarray = prevtoken as JArray;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        else if ((comma == false && curarray.Count > 0))   // missing comma
                        {
                            return ParseError(parser, "Missing Comma between array elements", flags, out error);
                        }
                        else
                        {
                            o.Level = sptr;
                            curarray.Add(o);
                            if (o.TokenType == TType.Array) // if array, we need to change to this as controlling object on top of stack
                            {
                                if (sptr == stack.Length - 1)
                                {
                                    return ParseError(parser, "Stack overflow", flags, out error);
                                }
                                stack[++sptr] = o;              // push this one onto stack
                                curarray = o as JArray;         // this is now the current array
                                comma = false;
                            }
                            else if (o.TokenType == TType.Object) // if object, this is the controlling object
                            {
                                if (sptr == stack.Length - 1)
                                {
                                    return ParseError(parser, "Stack overflow", flags, out error);
                                }
                                stack[++sptr] = o;              // push this one onto stack
                                curobject = o as JObject;       // this is now the current object
                                curarray = null;
                                comma = false;
                                break;
                            }
                            else
                            {
                                comma = parser.IsCharMoveOn(',');
                            }
                        }
                    }
                }
            }
        }
        static private string GenErrorString(IStringParserQuick parser, string text)
        {
            int pp = Math.Max(parser.Position - 1, 0);
            string error = "JSON " + text + " at " + parser.Position + " " + parser.Line.Substring(0, pp) + " <ERROR> "
                            + parser.Line.Substring(pp);
            System.Diagnostics.Debug.WriteLine(error);
            return error;
        }
        static private JToken ParseError(IStringParserQuick parser, string text, ParseOptions flags, out string error)
        {
            error = GenErrorString(parser, text);
            if ((flags & ParseOptions.ThrowOnError) != 0)
            {
                throw new JsonException(error);
            }
            return null;
        }
        private const int defaultstackdepth = 256;
    }
}
namespace QuickJSON
{
    public partial class JToken
    {
        public class TokenException : System.Exception
        {
            public string Error { get; set; }
            public TokenException(string s) { Error = s; }
        }
        public static IEnumerable<JToken> ParseToken(TextReader tr, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            var parser = new StringParserQuickTextReader(tr, charbufsize);
            return ParseToken(parser, flags, charbufsize);
        }
        public static IEnumerable<JToken> ParseToken(IStringParserQuick parser, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            var res = ParseTokenInt(parser, flags, charbufsize);
            if ((flags & ParseOptions.CheckEOL) != 0 && !parser.IsEOL())
            {
                throw new TokenException(GenErrorString(parser, "Extra Chars after JSON"));
            }
            return res;
        }
        private static IEnumerable<JToken> ParseTokenInt(IStringParserQuick parser, JToken.ParseOptions flags, int maxstringlen)
        {
            char[] textbuffer = new char[maxstringlen];
            JToken[] stack = new JToken[1024];
            int sptr = 0;
            bool comma = false;
            JArray curarray = null;
            JObject curobject = null;
            {
                parser.SkipSpace();
                JToken o = parser.JNextValue(textbuffer, false);       // grab new value, not array end
                if (o == null)
                {
                    throw new TokenException(GenErrorString(parser, "No Obj/Array"));
                }
                o.Level = sptr;

                if (o.TokenType == JToken.TType.Array)
                {
                    stack[++sptr] = o;                      // push this one onto stack
                    curarray = o as JArray;                 // this is now the current array
                    yield return o;
                }
                else if (o.TokenType == JToken.TType.Object)
                {
                    stack[++sptr] = o;                      // push this one onto stack
                    curobject = o as JObject;               // this is now the current object
                    yield return o;
                }
                else
                {
                    yield return o;                               // value only
                    yield break;
                }
            }
            while (true)
            {
                if (curobject != null)      // if object..
                {
                    while (true)
                    {
                        char next = parser.GetChar();
                        if (next == '}')    // end object
                        {
                            parser.SkipSpace();
                            if (comma == true && (flags & JToken.ParseOptions.AllowTrailingCommas) == 0)
                            {
                                throw new TokenException(GenErrorString(parser, "Comma"));
                            }
                            else
                            {
                                yield return new JToken(JToken.TType.EndObject, level: sptr - 1);
                                JToken prevtoken = stack[--sptr];
                                if (prevtoken == null)      // if popped stack is null, we are back to beginning, return this
                                {
                                    yield break;
                                }
                                else
                                {
                                    comma = parser.IsCharMoveOn(',');
                                    curobject = prevtoken as JObject;
                                    if (curobject == null)
                                    {
                                        curarray = prevtoken as JArray;
                                        break;
                                    }
                                }
                            }
                        }
                        else if (next == '"')   // property name
                        {
                            int textlen = parser.NextQuotedString(next, textbuffer, true);      // names can be empty
                            if (textlen < 0 || (comma == false && curobject.Count > 0) || !parser.IsCharMoveOn(':'))
                            {
                                throw new TokenException(GenErrorString(parser, "Object missing property name"));
                            }
                            else
                            {
                                string name = new string(textbuffer, 0, textlen);
                                JToken o = parser.JNextValue(textbuffer, false);      // get value
                                if (o == null)
                                {
                                    if ((flags & ParseOptions.IgnoreBadObjectValue) != 0)       // if we get a bad value, and flag set, try and move to the next start point
                                    {
                                        while (true)
                                        {
                                            char nc = parser.PeekChar();
                                            if (nc == char.MinValue || nc == '"' || nc == '}')  // looking for next property " or } or eol
                                                break;
                                            else
                                                parser.GetChar();
                                        }
                                    }
                                    else
                                    {
                                        throw new TokenException(GenErrorString(parser, "Object bad value"));
                                    }
                                }
                                else
                                {
                                    o.Name = name;              // we keep the name, even if its a repeat or an empty string. OriginalName stays empty
                                    o.Level = sptr;
                                    yield return o;
                                    if (o.TokenType == JToken.TType.Array) // if array, we need to change to this as controlling object on top of stack
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                        }
                                        stack[++sptr] = o;          // push this one onto stack
                                        curarray = o as JArray;     // this is now the current object
                                        curobject = null;
                                        comma = false;
                                        break;
                                    }
                                    else if (o.TokenType == JToken.TType.Object)   // if object, this is the controlling object
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                        }
                                        stack[++sptr] = o;          // push this one onto stack
                                        curobject = o as JObject;                 // this is now the current object
                                        comma = false;
                                    }
                                    else
                                    {
                                        comma = parser.IsCharMoveOn(',');
                                    }
                                }
                            }
                        }
                        else
                        {
                            throw new TokenException(GenErrorString(parser, "Bad format in object"));
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        JToken o = parser.JNextValue(textbuffer, true);       // grab new value
                        if (o == null)
                        {
                            throw new TokenException(GenErrorString(parser, "Bad array value"));
                        }
                        else if (o.TokenType == JToken.TType.EndArray)          // if end marker, jump back
                        {
                            if (comma == true && (flags & JToken.ParseOptions.AllowTrailingCommas) == 0)
                            {
                                throw new TokenException(GenErrorString(parser, "Comma"));
                            }
                            else
                            {
                                yield return new JToken(JToken.TType.EndArray, level: sptr - 1);
                                JToken prevtoken = stack[--sptr];
                                if (prevtoken == null)      // if popped stack is null, we are back to beginning, return this
                                {
                                    yield break;
                                }
                                else
                                {
                                    comma = parser.IsCharMoveOn(',');
                                    curobject = prevtoken as JObject;
                                    if (curobject == null)
                                    {
                                        curarray = prevtoken as JArray;
                                    }
                                    else
                                        break;
                                }
                            }
                        }
                        else if ((comma == false && curarray.Count > 0))   // missing comma
                        {
                            throw new TokenException(GenErrorString(parser, "Comma"));
                        }
                        else
                        {
                            o.Level = sptr;
                            yield return o;
                            if (o.TokenType == JToken.TType.Array) // if array, we need to change to this as controlling object on top of stack
                            {
                                if (sptr == stack.Length - 1)
                                {
                                    throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                }
                                stack[++sptr] = o;              // push this one onto stack
                                curarray = o as JArray;         // this is now the current array
                                comma = false;
                            }
                            else if (o.TokenType == JToken.TType.Object) // if object, this is the controlling object
                            {
                                if (sptr == stack.Length - 1)
                                {
                                    throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                }
                                stack[++sptr] = o;              // push this one onto stack
                                curobject = o as JObject;       // this is now the current object
                                curarray = null;
                                comma = false;
                                break;
                            }
                            else
                            {
                                comma = parser.IsCharMoveOn(',');
                            }
                        }
                    }
                }
            }
        }
        public static bool LoadTokens(IEnumerator<JToken> enumerator)
        {
            JToken t = enumerator.Current;
            if (t.IsObject)
            {
                JObject jo = t as JObject;
                while (enumerator.MoveNext())
                {
                    JToken i = enumerator.Current;
                    if (i.IsEndObject)
                        return true;
                    else if (i.IsArray || i.IsObject)
                    {
                        if (!LoadTokens(enumerator))
                            return false;
                    }
                    if (i.IsProperty)
                    {
                        jo.Add(i.Name, i);
                    }
                }
                return false;
            }
            else if (t.IsArray)
            {
                JArray ja = t as JArray;
                while (enumerator.MoveNext())
                {
                    JToken i = enumerator.Current;
                    if (i.IsEndArray)
                        return true;
                    else if (i.IsArray || i.IsObject)
                    {
                        if (!LoadTokens(enumerator))
                            return false;
                    }
                    ja.Add(i);
                }
                return false;
            }
            return false;
        }
    }

}
namespace QuickJSON
{
    public static class JTokenExtensions
    {
        public static T ToObjectQ<T>(this JToken token)
        {
            return ToObject<T>(token, false);
        }
        public static T ToObject<T>(this JToken token, bool ignoretypeerrors = false,
                                        Func<Type, Type, string, object> process = null,
                                        string setname = null,
                                        Func<MemberInfo, object, object> customconverter = null)
        {
            try
            {
                ToObjectConverter cnv = new ToObjectConverter(ignoretypeerrors, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                                                                process, customconverter, setname);
                Object ret = cnv.Execute(token, typeof(T), null, false, 0, null);
                if (ret is ToObjectError)
                {
                    System.Diagnostics.Debug.WriteLine("JSON ToObject error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                    return default(T);
                }
                else if (ret != null)      // or null
                    return (T)ret;          // must by definition have returned tt.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception JSON ToObject<T> {ex.Message} [{ex.Data?.GetType().Name}] {ex.StackTrace}");
            }
            return default(T);
        }
        public static Object ToObjectProtected(this JToken token, Type converttype, bool ignoretypeerrors,
                                    System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                                    Object initialobject = null,
                                    Func<Type, Type, string, object> process = null,
                                    string setname = null,
                                    Func<MemberInfo, object, object> customconverter = null
                                    )
        {
            try
            {
                ToObjectConverter cnv = new ToObjectConverter(ignoretypeerrors, membersearchflags, process, customconverter, setname);
                return cnv.Execute(token, converttype, initialobject, false, 0, null);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Exception JSON ToObject {ex.Message} [{ex.Data?.GetType().Name}] {ex.StackTrace}");
                return new ToObjectError($"JSON ToObject Exception {ex.Message}");
            }
        }
        public static Object ToObject(this JToken token, Type converttype, bool ignoretypeerrors,
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
            Object initialobject = null,
            Func<Type, Type, string, object> process = null,
            string setname = null,
            Func<MemberInfo, object, object> customconverter = null
            )
        {
            ToObjectConverter cnv = new ToObjectConverter(ignoretypeerrors, membersearchflags, process, customconverter, setname);
            return cnv.Execute(token, converttype, initialobject, false, 0, null);
        }
        [System.Diagnostics.DebuggerDisplay("ToObjectError:{ErrorString}")]
        public class ToObjectError
        {
            public string ErrorString;
            public string PropertyName;
            public ToObjectError(string s) { ErrorString = s; PropertyName = ""; }
        };
        public static void ClearToObjectCache()
        {
            ToObjectConverter.CacheOfTypesToMembers.Clear();
        }
    }
    internal class ToObjectConverter
    {
        public Object Execute(JToken token, Type converttype, Object initialobject, bool forcecustomcall, int minimumarraylength, ConstructorInfo forceconstructor)
        {
            if (token == null)
            {
                return null;
            }
            else if (token.IsArray)
            {
                JArray jarray = (JArray)token;
                try               // we need to protect this, as createinstance can fail here if a bad type is fed in, and we really want to report it via ObjectError
                {
                    if (converttype.IsArray)        // need to handle arrays because it needs a defined length
                    {
                        dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype, Math.Max(token.Count, minimumarraylength));
                        for (int i = 0; i < token.Count; i++)
                        {
                            dynamic ret = Execute(token[i], converttype.GetElementType(), null, forcecustomcall, 0, forceconstructor);
                            if (ret != null && ret.GetType() == typeof(ToObjectError))      // if failed to convert, augment the error and return
                            {
                                ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                                return ret;
                            }
                            else
                            {
                                instance[i] = ret;
                            }
                        }
                        return instance;
                    }
                    else if (converttype.IsEnumerable())      // else anything which is enumerable can be handled by add
                    {
                        dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the List
                        var types = converttype.GetGenericArguments();
                        for (int i = 0; i < token.Count; i++)
                        {
                            dynamic ret = Execute(token[i], types[0], null, forcecustomcall, 0, forceconstructor);
                            if (ret != null && ret.GetType() == typeof(ToObjectError))  // lists must be full, any errors are errors
                            {
                                ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                                return ret;
                            }
                            else
                            {
                                instance.Add(ret);
                            }
                        }
                        return instance;
                    }
                    else
                        return new ToObjectError($"JSONToObject: Not array {converttype.Name}");        // catch all ending
                }
                catch (Exception ex)
                {
                    return new ToObjectError($"JSONToObject: Create Error {converttype.Name} {ex.Message}");
                }
            }
            else if (token.TokenType == JToken.TType.Object)                   // objects are best efforts.. fills in as many fields as possible
            {
                if (converttype.IsDictionary())       // if its a Dictionary<x,y> then expect a set of objects
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras
                    System.Collections.IDictionary idict = instance as System.Collections.IDictionary;
                    var types = converttype.GetGenericArguments();      // types[0] will be key, types[1] is value
                    bool keyisenum = types[0].IsEnum;          // if enum, we will process they key thru either processenumdatetime or via a parse
                    bool keyisdatetime = !keyisenum && types[0].Name.Equals("DateTime");
                    bool keyisstring = !keyisenum && !keyisdatetime && types[0].Name.Equals("String"); // if string, direct assign of kvp.Key to it
                    ConstructorInfo keyconstructor = null;
                    if (!keyisenum && !keyisstring && !keyisdatetime)            // if neither, see if there is a constructor taking a single string..
                    {
                        keyconstructor = types[0].GetConstructors(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(string)).FirstOrDefault();

                        if (keyconstructor == null)             // no constructor, so try and see if we can use Convert.ChangeType on it
                        {
                            if (types[0].GetInterface("System.IConvertible") == null)
                                return new ToObjectError($"JSONToObject: Dictionary but key is not enum or string or does not have a constructor taking a single string {types[0].Name}");
                        }
                    }
                    foreach (var kvp in (JObject)token)
                    {
                        dynamic ret = Execute(kvp.Value, types[1], null, forcecustomcall, 0, forceconstructor);
                        if (ret != null && ret.GetType() == typeof(ToObjectError))
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + kvp.Key + "." + ((ToObjectError)ret).PropertyName;
                            if (ignoretypeerrors)
                            {
                                System.Diagnostics.Debug.WriteLine("Ignoring Object error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                            }
                            else
                            {
                                return ret;
                            }
                        }
                        else
                        {
                            if (keyisstring)
                            {
                                idict[kvp.Key] = ret;            // direct assign
                            }
                            else if (keyisdatetime)
                            {
                                if (processenumdatetime != null)     // if key is an enum, and we have a proprietory convert.. call it to convert key.)
                                {
                                    object k = processenumdatetime(types[0], converttype, kvp.Key);
                                    idict[k] = ret;
                                }
                                else
                                {
                                    object k = Convert.ChangeType(kvp.Key, types[0]);                  // must be IConvertible
                                    idict[k] = ret;
                                }
                            }
                            else if (keyisenum)
                            {
                                if (processenumdatetime != null)     // if key is an enum, and we have a proprietory convert.. call it to convert key.)
                                {
                                    object k = processenumdatetime(types[0], converttype, kvp.Key);
                                    idict[k] = ret;
                                }
                                else
                                {
                                    try
                                    {
                                        object k = Enum.Parse(types[0], kvp.Key, true);
                                        idict[k] = ret;
                                    }
                                    catch
                                    {
                                        return new ToObjectError($"JSONToObject: Dictionary key unrecognised value '{kvp.Key}' for enum {types[0].Name}");
                                    }
                                }
                            }
                            else if (keyconstructor != null)
                            {
                                object k = keyconstructor.Invoke(new object[] { kvp.Key });       // make it with the string as its parameter
                                idict[k] = ret;
                            }
                            else
                            {
                                object k = Convert.ChangeType(kvp.Key, types[0]);                  // must be IConvertible
                                idict[k] = ret;
                            }
                        }
                    }
                    return instance;
                }
                else if (converttype.IsClass ||      // if class
                            (converttype.IsValueType && !converttype.IsPrimitive && !converttype.IsEnum && converttype != typeof(DateTime)))   // or struct, but not datetime (handled below)
                {
                    var instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras
                    AttrList attrlist = GetOrCreate(converttype, setname, membersearchflags);
                    foreach (var kvp in (JObject)token)
                    {
                        if (attrlist.MembersToSet.TryGetValue(kvp.Key, out AttrControl ac))
                        {
                            MemberInfo mi = ac.MemberInfo;
                            if (mi != null)                                     // ignore any ones with null as its memberinfo as its an ignored value
                            {
                                Type otype = mi.FieldPropertyType();
                                string name = otype.Name;                       // compare by name quicker than is
                                var tk = kvp.Value;
                                bool success = false;
                                if (name.Equals("Nullable`1"))                          // nullable types
                                {
                                    if (!tk.IsNull)         // if not null, then get underlying type.. and store in name. If null, its a null and below will set it to null
                                        name = otype.GenericTypeArguments[0].Name;    // get
                                }
                                if (ac.UseCustomConvert)               // if this is marked as custom, convert the whole object as custom
                                {
                                    Object ret = customconvert(mi, tk.Value);
                                    if (ret is ToObjectError)
                                        return ret;
                                    success = mi.SetValue(instance, ret);
                                }
                                else if (tk.IsArray)
                                {
                                    Object ret = Execute(kvp.Value, otype, null, ac.UseCustomConvertArray, ac.MinimumArrayLength, ac.JTokenConstructorInfo);
                                    if (ret is ToObjectError)
                                        return ret;

                                    success = mi.SetValue(instance, ret);
                                }
                                else if (tk.IsObject)
                                {
                                    Object ret = Execute(kvp.Value, otype, null, ac.UseCustomConvertArray, 0, ac.JTokenConstructorInfo);
                                    if (ret is ToObjectError)
                                        return ret;
                                    success = mi.SetValue(instance, ret);
                                }
                                else if (ac.JTokenConstructorInfo != null)      // new July 26, construct thru constructor call
                                {
                                    if (tk.IsNull)                              // if token is null, it means don't make it..
                                        success = true;
                                    else
                                    {
                                        var ret = ac.JTokenConstructorInfo.Invoke(new object[] { tk });       // make it with the token
                                        success = mi.SetValue(instance, ret);       // set it
                                    }
                                }
                                else if (name.Equals("String"))                              // copies of QuickJSON explicit operators in QuickJSON.cs
                                {
                                    if (tk.IsNull)
                                        success = mi.SetValue(instance, null);
                                    else if (tk.IsString)
                                        success = mi.SetValue(instance, (string)tk.Value);
                                }
                                else if (name.Equals("Int64"))
                                {
                                    if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, tk.Value);
                                    else if (tk.TokenType == TType.Double)
                                        success = mi.SetValue(instance, (long)(double)tk.Value);
                                }
                                else if (name.Equals("Double"))
                                {
                                    if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, (double)(long)tk.Value);
                                    else if (tk.TokenType == TType.ULong)
                                        success = mi.SetValue(instance, (double)(ulong)tk.Value);
#if JSONBIGINT
                                    else if (tk.TokenType == TType.BigInt)
                                        success = mi.SetValue(instance, (double)(System.Numerics.BigInteger)tk.Value);
#endif
                                    else if (tk.TokenType == TType.Double)
                                        success = mi.SetValue(instance, (double)tk.Value);
                                }
                                else if (name.Equals("Int32"))
                                {
                                    if (tk.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                                        success = mi.SetValue(instance, (int)(long)tk.Value);
                                    else if (tk.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                                        success = mi.SetValue(instance, (int)(double)tk.Value);
                                }
                                else if (name.Equals("Boolean"))
                                {
                                    if (tk.TokenType == TType.Boolean)
                                        success = mi.SetValue(instance, (bool)tk.Value);
                                    else if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, (long)tk.Value != 0);
                                }
                                else if (name.Equals("Single"))
                                {
                                    if (tk.TokenType == TType.Long)
                                        success = mi.SetValue(instance, (float)(long)tk.Value);
                                    else if (tk.TokenType == TType.ULong)
                                        success = mi.SetValue(instance, (float)(ulong)tk.Value);
#if JSONBIGINT
                                    else if (tk.TokenType == TType.BigInt)
                                        success = mi.SetValue(instance, (float)(System.Numerics.BigInteger)tk.Value);
#endif
                                    else if (tk.TokenType == TType.Double)
                                        success = mi.SetValue(instance, (float)(double)tk.Value);
                                }
                                else if (otype.IsEnum)
                                {
                                    if (tk.IsString)     // if string, possible
                                    {
                                        Object p = null;
                                        if (processenumdatetime != null)    // process can return value
                                        {
                                            p = processenumdatetime(otype, null, (string)tk.Value);
                                            if (p != null && p.GetType() != typeof(ToObjectError))  // if good, and not error, try set
                                                success = mi.SetValue(instance, p);
                                        }
                                        else
                                        {
                                            try
                                            {
                                                p = Enum.Parse(otype, (string)tk.Value, true); // get object, will except if bad, in which case we just swallow
                                                success = mi.SetValue(instance, p);
                                            }
                                            catch
                                            {   // error, leave success off
                                                if (JToken.TraceOutput)
                                                    System.Diagnostics.Trace.WriteLine($"QuickJSON ToObject: Cannot set value on enum property {mi.Name} in {converttype.Name} because `{tk.Str()}` is not a member");
                                                if (!ignoretypeerrors)
                                                    return new ToObjectError($"QuickJSON ToObject: Cannot set value on enum property {mi.Name} in {converttype.Name} because `{tk.Str()}` is not a member");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return new ToObjectError($"QuickJSON ToObject: Cannot set value on enum property {mi.Name} in {converttype.Name} because token is not a string");
                                    }
                                }
                                else if (name.Equals("UInt32"))
                                {
                                    if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                                        success = mi.SetValue(instance, (uint)(long)tk.Value);
                                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                                        success = mi.SetValue(instance, (uint)(double)tk.Value);
                                }
                                else if (name.Equals("UInt64"))
                                {
                                    if (tk.TokenType == TType.ULong)
                                        success = mi.SetValue(instance, (ulong)tk.Value);
                                    else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                                        success = mi.SetValue(instance, (ulong)(long)tk.Value);
                                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                                        success = mi.SetValue(instance, (ulong)(double)tk.Value);
                                }
                                else if (name.Equals("Nullable`1"))                     // name stays nullable`1 if value is null
                                {
                                    success = mi.SetValue(instance, null);
                                }
                                else
                                {
                                    Object ret = Execute(kvp.Value, otype, null, false, 0, null);
                                    success = ret == null || ret.GetType() != typeof(ToObjectError);    // is it a good conversion
                                    if (success)      // if good, set
                                        success = mi.SetValue(instance, ret);
                                }
                                if (!success)
                                {
                                    if (ignoretypeerrors)
                                    {
                                        if (JToken.TraceOutput)
                                            System.Diagnostics.Trace.WriteLine($"QuickJSON ToObject Ignoring cannot set value on property {mi.Name} in {converttype.Name} {tk.TokenType} vs {otype.Name} {Environment.StackTrace}");
                                        success = true;
                                    }
                                    else
                                    {
                                        return new ToObjectError($"Cannot set value on property {mi.Name} in {converttype.Name}`");
                                    }
                                }
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine($"ToObject ignore {converttype.Name}:{kvp.Key}");
                            }
                        }
                        else
                        {
                        }
                    }
                    if (attrlist.MembersMustConstruct != null)
                    {
                        foreach (var attrcontrol in attrlist.MembersMustConstruct)
                        {
                            if (attrcontrol.DefaultConstructorInfo == null)     // if they don't have a default constructor moanthe o
                            {
                                return new ToObjectError($"JSONToObject: No default constructor defined for {attrcontrol.MemberInfo.Name}");
                            }
                            else if (attrcontrol.MemberInfo.GetValue(instance) == null)    // if null, then default construct
                            {
                                var ret = attrcontrol.DefaultConstructorInfo.Invoke(new object[] { });       // make it with no parameters
                                if (ret != null)
                                {
                                    if (!attrcontrol.MemberInfo.SetValue(instance, ret))
                                        return new ToObjectError($"JSONToObject: [JTokenAlwaysCreate] member set failed for {attrcontrol.MemberInfo.Name}");
                                }
                                else
                                    return new ToObjectError($"JSONToObject: [JTokenAlwaysCreate] constructor invoke failed and reported null {attrcontrol.MemberInfo.Name}");
                            }
                        }
                    }
                    return instance;
                }
                else
                    return new ToObjectError($"JSONToObject: Not class {converttype.Name}");
            }
            else
            {
                var tk = token;
                string name = converttype.Name;                         // compare by name quicker than is
                if (name.Equals("Nullable`1"))                          // nullable types
                {
                    if (tk.IsNull)
                        return null;
                    name = converttype.GenericTypeArguments[0].Name;    // get underlying type.. and store in name
                    converttype = converttype.GenericTypeArguments[0];  // update converttype
                }
                if (forcecustomcall)                                      // if custom convert is forced, do it
                {
                    return customconvert(converttype, token.Value);
                }
                else if (converttype.IsEnum)
                {
                    if (!token.IsString)
                    {
                        if (TraceOutput)
                            System.Diagnostics.Trace.WriteLine($"QuickJSON ToObject: Enum Token is not string for {converttype.Name}");
                        return new ToObjectError($"JSONToObject: Enum Token is not string for {converttype.Name}");
                    }
                    try
                    {
                        if (forcecustomcall)       // if force custom on it, do it
                        {
                            return customconvert(converttype, token.Value);
                        }
                        else if (processenumdatetime != null)        // if enum processor, do it
                        {
                            var value = processenumdatetime(converttype, null, (string)token.Value);
                            return value;
                        }
                        else
                        {
                            return Enum.Parse(converttype, (string)token.Value, true);
                        }
                    }
                    catch
                    {
                        if (TraceOutput)
                            System.Diagnostics.Trace.WriteLine($"QuickJSON ToObject: Unrecognised value '{token.Str()}' for enum {converttype.Name}");
                        return new ToObjectError($"QuickJSON ToObject: Cannot set value on enum property in {converttype.Name} because `{tk.Str()}` is not a member");
                    }
                }
                else if (forceconstructor != null)                                      // if custom convert is forced, do it
                {
                    var ret = forceconstructor.Invoke(new object[] { tk });       // make it with the token
                    return ret;
                }
                else if (name.Equals("String"))                              // copies of QuickJSON explicit operators in QuickJSON.cs
                {
                    if (tk.IsNull)
                        return null;
                    else if (tk.IsString)
                        return tk.Value;
                }
                else if (name.Equals("Int32"))
                {
                    if (tk.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                        return (int)(long)tk.Value;
                    else if (tk.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                        return (int)(double)tk.Value;
                }
                else if (name.Equals("Int64"))
                {
                    if (tk.TokenType == TType.Long)
                        return tk.Value;
                    else if (tk.TokenType == TType.Double)
                        return (long)(double)tk.Value;
                }
                else if (name.Equals("Boolean"))
                {
                    if (tk.TokenType == TType.Boolean)
                        return (bool)tk.Value;
                    else if (tk.TokenType == TType.Long)
                        return (long)tk.Value != 0;
                }
                else if (name.Equals("Double"))
                {
                    if (tk.TokenType == TType.Long)
                        return (double)(long)tk.Value;
                    else if (tk.TokenType == TType.ULong)
                        return (double)(ulong)tk.Value;
#if JSONBIGINT
                    else if (tk.TokenType == TType.BigInt)
                        return (double)(System.Numerics.BigInteger)tk.Value;
#endif
                    else if (tk.TokenType == TType.Double)
                        return (double)tk.Value;
                }
                else if (name.Equals("Single"))
                {
                    if (tk.TokenType == TType.Long)
                        return (float)(long)tk.Value;
                    else if (tk.TokenType == TType.ULong)
                        return (float)(ulong)tk.Value;
#if JSONBIGINT
                    else if (tk.TokenType == TType.BigInt)
                        return (float)(System.Numerics.BigInteger)tk.Value;
#endif
                    else if (tk.TokenType == TType.Double)
                        return (float)(double)tk.Value;
                }
                else if (name.Equals("UInt32"))
                {
                    if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                        return (uint)(long)tk.Value;
                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                        return (uint)(double)tk.Value;
                }
                else if (name.Equals("UInt64"))
                {
                    if (tk.TokenType == TType.ULong)
                        return (ulong)tk.Value;
                    else if (tk.TokenType == TType.Long && (long)tk.Value >= 0)
                        return (ulong)(long)tk.Value;
                    else if (tk.TokenType == TType.Double && (double)tk.Value >= 0)
                        return (ulong)(double)tk.Value;
                }
                else if (name.Equals("DateTime"))
                {
                    if (tk.IsString)       // must be a string
                    {
                        if (processenumdatetime != null)
                        {
                            return processenumdatetime(converttype, null, (string)tk.Value);
                        }
                        else
                        {
                            if (DateTime.TryParse((string)tk.Value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime ret))
                            {
                                return ret;
                            }
                        }
                    }
                }
                else if (name.Equals("Object"))                     // catch all, june 7/6/23 fix
                {
                    return tk.Value;
                }
                return new ToObjectError("JSONToObject: Bad Conversion " + tk.TokenType + " to " + converttype.Name);
            }
        }
        [System.Diagnostics.DebuggerDisplay("{MemberInfo.Name} {UseCustomFormat} {UseCustomFormatArray}:{MinimumArrayLength} {AlwaysCreate}")]
        public class AttrControl
        {
            public ConstructorInfo JTokenConstructorInfo; // null if no public <name>(QuickJson.JToken) defined, valid for object and array elements
            public MemberInfo MemberInfo;       // null if not to include this json name information into the object, else the member to set.
            public bool UseCustomConvert;           // JsonCustomFormat: if custom format call the customformat should be used to convert the whole object
            public bool UseCustomConvertArray;      // JsonCustomFormatArray : if custom format array the array elements the customformat call should be used to convert it
            public int MinimumArrayLength;          // JsonCustomArrayLength : minimum array length to make
            public ConstructorInfo DefaultConstructorInfo; // null if not [JsonAlwaysCreate] else reference to its default empty constructor
        }
        public class AttrList
        {
            public Dictionary<string, AttrControl> MembersToSet { get; set; } = new Dictionary<string, AttrControl>();
            public List<AttrControl> MembersMustConstruct = null;
        }
        static public Dictionary<Tuple<string, Type, BindingFlags>, AttrList> CacheOfTypesToMembers =
                    new Dictionary<Tuple<string, Type, BindingFlags>, AttrList>();
        static public AttrList GetOrCreate(Type converttype, string setname, BindingFlags membersearchflags)
        {
            lock (CacheOfTypesToMembers)        // thread lock
            {
                var key = new Tuple<string, Type, BindingFlags>(setname, converttype, membersearchflags);
                if (!CacheOfTypesToMembers.TryGetValue(key, out AttrList attrlist))     // if not found
                {
                    CacheOfTypesToMembers[key] = attrlist = new AttrList();     // set up a new one.
                    var list = new List<System.Reflection.MemberInfo>();
                    list.AddRange(converttype.GetFields(membersearchflags));        // get field list..
                    list.AddRange(converttype.GetProperties(membersearchflags));        // get property list
                    foreach (System.Reflection.MemberInfo mi in list)
                    {
                        bool includeit = true;
                        var calist = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                        if (calist.Length == 1)
                        {
                            JsonIgnoreAttribute jia = calist[0] as JsonIgnoreAttribute;
                            if (jia.Setting == null)       // null, means all sets
                            {
                                includeit = false;
                            }
                            else
                            {
                                for (int i = 0; i < jia.Setting.Length; i++)        // try and find the set, if found
                                {
                                    if (jia.Setting[i].Set.EqualsI(setname))        // found set, as long as its not an ignore/include list..
                                    {
                                        if (jia.Setting[i].Ignore == null && jia.Setting[i].IncludeOnly == null)
                                            includeit = false;
                                        break;
                                    }
                                }
                            }
                        }
                        var ac = new AttrControl();     // member info = null, therefore default is ignore.
                        if (includeit)      // only worth doing this if we are including it
                        {
                            ac.MemberInfo = mi;
                            {
                                Type otype = mi.FieldPropertyType();        // new JULY 26 see if we have a custom constructor
                                if (otype.IsArray)  // set the ConstructorInfo of an array to the underlying type. This is passed from the array down
                                {
                                    otype = otype.GetElementType();
                                }
                                else if (otype == typeof(string))
                                {
                                }
                                else if (otype.IsDictionary())
                                {
                                    var genargs = otype.GetGenericArguments();
                                    if (genargs.Length == 2)
                                        otype = genargs[1];     // key/value, we want value
                                    else
                                        otype = null;
                                }
                                else if (otype.IsEnumerable())
                                {
                                    var genargs = otype.GetGenericArguments();
                                    if (genargs.Length == 1)
                                        otype = genargs[0];
                                    else
                                        otype = null;
                                }
                                if (otype != null)
                                    ac.JTokenConstructorInfo = otype.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(QuickJSON.JToken)).FirstOrDefault();
                            }
                            var customoutput = mi.GetCustomAttributes(typeof(JsonCustomFormat), false);
                            if (customoutput.Length == 1)
                            {
                                JsonCustomFormat jcf = customoutput[0] as JsonCustomFormat;
                                if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                {
                                    ac.UseCustomConvert = true;
                                }
                            }
                            var customoutputarray = mi.GetCustomAttributes(typeof(JsonCustomFormatArray), false);
                            if (customoutputarray.Length == 1)
                            {
                                JsonCustomFormatArray jcf = customoutputarray[0] as JsonCustomFormatArray;
                                if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                {
                                    ac.UseCustomConvertArray = true;
                                }
                            }
                            var customminimumarraylength = mi.GetCustomAttributes(typeof(JsonCustomArrayLength), false);
                            if (customminimumarraylength.Length == 1)
                            {
                                JsonCustomArrayLength jcf = customminimumarraylength[0] as JsonCustomArrayLength;
                                if (jcf.Sets == null)
                                {
                                    ac.MinimumArrayLength = jcf.MinimumLength[0];
                                }
                                else
                                {
                                    int indexof = Array.IndexOf(jcf.Sets, setname);     // see if set name defined, if so, pick up minimum length
                                    if (indexof >= 0)
                                        ac.MinimumArrayLength = jcf.MinimumLength[indexof];
                                }
                            }
                            var customalwayscreate = mi.GetCustomAttributes(typeof(JsonAlwaysCreate), false);
                            if (customalwayscreate.Length == 1)
                            {
                                JsonAlwaysCreate jcf = customalwayscreate[0] as JsonAlwaysCreate;
                                if (jcf.Sets == null || jcf.Sets.Contains(setname, StringComparer.InvariantCulture))    // if matches
                                {
                                    ac.DefaultConstructorInfo = mi.FieldPropertyType().GetConstructors(BindingFlags.Public | BindingFlags.Instance).Where(x => x.GetParameters().Length == 0).FirstOrDefault();
                                    if (attrlist.MembersMustConstruct == null)
                                        attrlist.MembersMustConstruct = new List<AttrControl>();
                                    attrlist.MembersMustConstruct.Add(ac);
                                }
                            }
                        }
                        var renamelist = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                        if (renamelist.Length == 1)
                        {
                            JsonNameAttribute na = renamelist[0] as JsonNameAttribute;
                            if (na.Sets == null)        // null, means all sets
                            {
                                foreach (var x in na.Names)
                                    attrlist.MembersToSet[x] = ac;
                            }
                            else
                            {
                                for (int i = 0; i < na.Sets.Length; i++)  // find set, if not found, no change, else add name to one accepted for this member
                                {
                                    if (na.Sets[i].EqualsI(setname))
                                    {
                                        attrlist.MembersToSet[na.Names[i]] = ac;
                                    }
                                }
                            }
                        }
                        else
                        {
                            attrlist.MembersToSet[mi.Name] = ac;
                        }
                    }
                }
                else
                {
                }
                return attrlist;
            }
        }
        private bool ignoretypeerrors;
        private BindingFlags membersearchflags;
        private Func<Type, Type, string, object> processenumdatetime;
        private Func<MemberInfo, object, object> customconvert;
        private string setname;
        public ToObjectConverter(bool ignoretypeerrors, BindingFlags membersearchflags, Func<Type, Type, string, object> process,
                                    Func<MemberInfo, object, object> customconvert, string setname)
        {
            this.ignoretypeerrors = ignoretypeerrors;
            this.membersearchflags = membersearchflags;
            this.processenumdatetime = process;
            this.customconvert = customconvert;
            this.setname = setname;
        }
    }
}
namespace QuickJSON
{
    public partial class JToken
    {
        public override string ToString()
        {
            return ToString(this, "", "", "", false);
        }
        public string ToStringLiteral()
        {
            return ToString(this, "", "", "", true);
        }
        public string ToString(bool verbose = false, string oapad = "  ")
        {
            return verbose ? ToString(this, "", "\r\n", oapad, false) : ToString(this, "", "", "", false);
        }
        public string ToString(string oapad)            // not verbose, but prefix in from of obj/array is configuration
        {
            return ToString(this, "", "", oapad, false);
        }
        public string ToString(string prepad, string postpad, string oapad, bool stringliterals, int linelength = int.MaxValue)
        {
            var sb = new System.Text.StringBuilder();
            int lastcr = 0;
            ToStringBuilder(sb, this, prepad, postpad, oapad, stringliterals, ref lastcr, linelength);
            return sb.ToString();
        }
        public static string ToString(JToken token, string prepad, string postpad, string oapad, bool stringliterals, int linelength = int.MaxValue)
        {
            var sb = new System.Text.StringBuilder();
            int lastcr = 0;
            ToStringBuilder(sb, token, prepad, postpad, oapad, stringliterals, ref lastcr, linelength);
            return sb.ToString();
        }
    }
}
namespace QuickJSON
{
    public partial class JToken
    {
        public static void ToStringBuilder(StringBuilder str, JToken token, string prepad, string postpad, string oapad, bool stringliterals)
        {
            int lastcr = 0;
            ToStringBuilder(str, token, prepad, postpad, oapad, stringliterals, ref lastcr, int.MaxValue);
        }
        public static void ToStringBuilder(StringBuilder str, JToken token, string prepad, string postpad, string oapad, bool stringliterals, ref int lastcr, int maxlinelength)
        {
            if (token.TokenType == TType.String)
            {
                if (stringliterals)       // used if your extracting the value of the data as a string, and not turning it back to json.
                    str.Append(prepad).Append((string)token.Value).Append(postpad);
                else
                    str.Append(prepad).Append('"').AppendEscapeControlCharsFull((string)token.Value).Append('"').Append(postpad);
            }
            else if (token.TokenType == TType.Double)
            {
                string sd = ((double)token.Value).ToStringInvariant("R");       // round trip it - use 'R' since minvalue won't work very well. See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
                if (!(sd.Contains("E") || sd.Contains(".")))                // needs something to indicate its a double, and if it does not have a dot or E, it needs a .0
                    sd += ".0";
                str.Append(prepad).Append(sd).Append(postpad);
            }
            else if (token.TokenType == TType.Long)
                str.Append(prepad).Append(((long)token.Value).ToStringInvariant()).Append(postpad);
            else if (token.TokenType == TType.ULong)
                str.Append(prepad).Append(((ulong)token.Value).ToStringInvariant()).Append(postpad);
#if JSONBIGINT
            else if (token.TokenType == TType.BigInt)
                str.Append(prepad).Append(((System.Numerics.BigInteger)token.Value).ToString(System.Globalization.CultureInfo.InvariantCulture)).Append(postpad);
#endif
            else if (token.TokenType == TType.Boolean)
                str.Append(prepad).Append(((bool)token.Value).ToString().ToLower()).Append(postpad);
            else if (token.TokenType == TType.Null)
                str.Append(prepad).Append("null").Append(postpad);
            else if (token.TokenType == TType.Array)
            {
                if (str.Length - lastcr > maxlinelength)
                {
                    str.Append(System.Environment.NewLine);
                    lastcr = str.Length;
                }
                str.Append(prepad).Append('[').Append(postpad);
                string arrpad = prepad + oapad;
                JArray ja = token as JArray;
                for (int i = 0; i < ja.Count; i++)
                {
                    bool notlast = i < ja.Count - 1;
                    ToStringBuilder(str, ja[i], arrpad, postpad, oapad, stringliterals, ref lastcr, maxlinelength);
                    if (notlast)
                    {
                        str.Remove(str.Length - postpad.Length, postpad.Length);    // remove the postpad
                        str.Append(',').Append(postpad);
                        if (str.Length - lastcr > maxlinelength)        // we don't cr at the last one, as we want the ] to be next
                        {
                            str.Append(System.Environment.NewLine);
                            lastcr = str.Length;
                        }
                    }
                }
                str.Append(prepad).Append(']').Append(postpad);
                if (str.Length - lastcr > maxlinelength)
                {
                    str.Append(System.Environment.NewLine);
                    lastcr = str.Length;
                }
            }
            else if (token.TokenType == TType.Object)
            {
                str.Append(prepad).Append('{').Append(postpad);
                string objpad = prepad + oapad;
                int i = 0;
                JObject jo = ((JObject)token);
                foreach (var e in jo)
                {
                    bool notlast = i++ < jo.Count - 1;
                    string name = e.Key;          // We use the key name, but it may be synthetic, so if its in the object we use the ParsedName
                    if (IsKeyNameSynthetic(name))
                        name = e.Value.ParsedName;
                    if (e.Value is JObject || e.Value is JArray)
                    {
                        if (stringliterals)
                            str.Append(objpad).Append(name).Append(':').Append(postpad);
                        else
                            str.Append(objpad).Append('"').AppendEscapeControlCharsFull(name).Append("\":").Append(postpad);
                        ToStringBuilder(str, e.Value, objpad, postpad, oapad, stringliterals, ref lastcr, maxlinelength);
                        if (notlast)
                        {
                            str.Remove(str.Length - postpad.Length, postpad.Length);    // remove the postpad
                            str.Append(',').Append(postpad);
                        }
                    }
                    else
                    {
                        if (stringliterals)
                            str.Append(objpad).Append(name).Append(':');
                        else
                            str.Append(objpad).Append('"').AppendEscapeControlCharsFull(name).Append("\":");
                        ToStringBuilder(str, e.Value, "", "", oapad, stringliterals, ref lastcr, maxlinelength);
                        if (notlast)
                            str.Append(',');
                        str.Append(postpad);
                    }
                    if (notlast && str.Length - lastcr > maxlinelength)
                    {
                        str.Append(System.Environment.NewLine);
                        lastcr = str.Length;
                    }
                }
                str.Append(prepad).Append('}').Append(postpad);
                if (str.Length - lastcr > maxlinelength)
                {
                    str.Append(System.Environment.NewLine);
                    lastcr = str.Length;
                }
            }
            else if (token.TokenType == TType.Error)
                str.Append("ERROR:" + (string)token.Value);
            else
                str.Append("?unknown");
        }
    }
}
namespace QuickJSON.Utils
{
    public static class Extensions
    {
        public static string ToStringZulu(this DateTime dt)     // zulu warrior format web style
        {
            if (dt.Millisecond != 0)
                return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fff'Z'");
            else
                return dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'");
        }
        public static string ToStringInvariant(this short v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this ushort v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this int v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this int v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this uint v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this uint v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this long v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this long v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this ulong v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this ulong v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringIntValue(this bool v)
        {
            return v ? "1" : "0";
        }
        public static string ToStringInvariant(this bool? v)
        {
            return (v.HasValue) ? (v.Value ? "1" : "0") : "";
        }
        public static string ToStringInvariant(this double v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this double v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this float v, string format)
        {
            return v.ToString(format, System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this float v)
        {
            return v.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        public static string ToStringInvariant(this double? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        public static string ToStringInvariant(this float? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        public static string ToStringInvariant(this int? v)
        {
            return (v.HasValue) ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        public static string ToStringInvariant(this int? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        public static string ToStringInvariant(this long? v)
        {
            return (v.HasValue) ? v.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        public static string ToStringInvariant(this long? v, string format)
        {
            return (v.HasValue) ? v.Value.ToString(format, System.Globalization.CultureInfo.InvariantCulture) : "";
        }
        public static bool ApproxEquals(this double left, double right, double epsilon = 2.2204460492503131E-16)       // fron newtonsoft JSON, et al, calculate relative epsilon and compare
        {
            if (left == right)
            {
                return true;
            }
            double tolerance = ((Math.Abs(left) + Math.Abs(right)) + 10.0) * epsilon;       // given an arbitary epsilon, scale to magnitude of values
            double difference = left - right;
            return (-tolerance < difference && tolerance > difference);
        }
        public static bool SetValue(this MemberInfo mi, Object instance, Object value)   // given a member of fields/property, set value in instance
        {
            if (mi.MemberType == System.Reflection.MemberTypes.Field)
            {
                var fi = (System.Reflection.FieldInfo)mi;
                fi.SetValue(instance, value);
                return true;
            }
            else if (mi.MemberType == System.Reflection.MemberTypes.Property)
            {
                var pi = (System.Reflection.PropertyInfo)mi;
                if (pi.SetMethod != null)
                {
                    pi.SetValue(instance, value);
                    return true;
                }
                else
                    return false;
            }
            else
                throw new NotSupportedException();
        }
        public static object GetValue(this MemberInfo mi, Object instance)   // given a member of fields/property, get value in instance
        {
            if (mi.MemberType == System.Reflection.MemberTypes.Field)
            {
                var fi = (System.Reflection.FieldInfo)mi;
                return fi.GetValue(instance);
            }
            else if (mi.MemberType == System.Reflection.MemberTypes.Property)
            {
                var pi = (System.Reflection.PropertyInfo)mi;
                if (pi.GetMethod != null)
                {
                    return pi.GetValue(instance);
                }
                else
                    return null;
            }
            else
                throw new NotSupportedException();
        }
        static public Type FieldPropertyType(this MemberInfo mi)        // from member info for properties/fields return type
        {
            if (mi.MemberType == System.Reflection.MemberTypes.Property)
                return ((System.Reflection.PropertyInfo)mi).PropertyType;
            else if (mi.MemberType == System.Reflection.MemberTypes.Field)
                return ((System.Reflection.FieldInfo)mi).FieldType;
            else
                return null;
        }
        public static string EscapeControlCharsFull(this string obj)        // unicode points not escaped out
        {
            string s = obj.Replace(@"\", @"\\");        // \->\\
            s = s.Replace("\r", @"\r");     // CR -> \r
            s = s.Replace("\"", "\\\"");     // " -> \"
            s = s.Replace("\t", @"\t");     // TAB - > \t
            s = s.Replace("\b", @"\b");     // BACKSPACE - > \b
            s = s.Replace("\f", @"\f");     // FORMFEED -> \f
            s = s.Replace("\n", @"\n");     // LF -> \n
            return s;
        }
        public static System.Text.StringBuilder AppendEscapeControlCharsFull(this System.Text.StringBuilder str, string s)
        {
            foreach (var c in s)
            {
                if (c == '\\')
                    str.Append(@"\\");
                else if (c == '\r')
                    str.Append(@"\r");
                else if (c == '"')
                    str.Append("\\\"");
                else if (c == '\t')
                    str.Append(@"\t");
                else if (c == '\b')
                    str.Append(@"\b");
                else if (c == '\f')
                    str.Append(@"\f");
                else if (c == '\n')
                    str.Append(@"\n");
                else
                    str.Append(c);
            }
            return str;
        }
        static public int? ToHex(this char c)
        {
            if (char.IsDigit(c))
                return c - '0';
            else if ("ABCDEF".Contains(c))
                return c - 'A' + 10;
            else if ("abcdef".Contains(c))
                return c - 'a' + 10;
            else
                return null;
        }
        public static string RegExWildCardToRegular(this string value)
        {
            if (value.Contains("*") || value.Contains("?"))
                return "^" + System.Text.RegularExpressions.Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
            else
                return "^" + value + ".*$";
        }
        public static bool WildCardMatch(this string value, string match, bool caseinsensitive = false)
        {
            match = match.RegExWildCardToRegular();
            return System.Text.RegularExpressions.Regex.IsMatch(value, match, caseinsensitive ? System.Text.RegularExpressions.RegexOptions.IgnoreCase : System.Text.RegularExpressions.RegexOptions.None);
        }
        public static uint Checksum(this string s)
        {
            uint checksum = 0;
            foreach (var ch in s)
            {
                checksum += 7 + (uint)ch * 23;
            }
            return Math.Max(1, checksum);
        }
        static public int? InvariantParseIntNull(this string s)     // s can be null
        {
            int i;
            if (s != null && int.TryParse(s, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out i))
                return i;
            else
                return null;
        }
        public static string AlwaysQuoteString(this string obj)
        {
            return "\"" + obj.Replace("\"", "\\\"") + "\"";
        }
        public static bool EqualsI(this string a, string b)
        {
            return a == null && b == null ? true : a != null ? a.Equals(b, StringComparison.InvariantCulture) : false;
        }
        public static Type GetItemType<T>(this IEnumerable<T> enumerable)
        {
            return typeof(T);
        }
        public static bool IsDictionary(this Type type)
        {
            return typeof(System.Collections.IDictionary).IsAssignableFrom(type);
        }
        public static bool IsEnumerable(this Type type)
        {
            return typeof(System.Collections.IEnumerable).IsAssignableFrom(type);
        }
    }
}
namespace QuickJSON.Utils
{
    internal static class NamespaceDoc { } // just for documentation purposes
    public interface IStringParserQuick
    {
        int Position { get; }
        string Line { get; }
        void SkipSpace();
        bool IsEOL();       // function as it can have side effects
        char GetChar();
        char GetNextNonSpaceChar(bool skipspacesafter = true);
        char PeekChar();
        bool IsStringMoveOn(string s);
        bool IsCharMoveOn(char t, bool skipspaceafter = true);
        void BackUp();
        int NextQuotedString(char quote, char[] buffer, bool replaceescape = false, bool skipafter = true);
        JToken JNextNumber(bool sign, bool skipafter = true);
        JToken JNextValue(char[] buffer, bool inarray);
        int NextCharBlock(char[] buffer, System.Func<char, bool> test, bool skipafter = true);
        uint ChecksumCharBlock(System.Func<char, bool> test, bool skipafter = true);
    }
}
namespace QuickJSON.Utils
{
    [System.Diagnostics.DebuggerDisplay("Action {new string(line,pos,line.Length-pos)} : ({new string(line,0,line.Length)})")]
    public class StringParserQuick : IStringParserQuick
    {
        #region Init and basic status
        public StringParserQuick(string l, int p = 0)
        {
            line = l.ToCharArray();
            pos = p;
            SkipSpace();
        }
        public int Position { get { return pos; } }
        public string Line { get { return new string(line, 0, line.Length); } }
        public bool IsEOL() { return pos == line.Length; }
        public int Left { get { return Math.Max(line.Length - pos, 0); } }
        #endregion
        #region Character or String related functions
        public void SkipSpace()
        {
            while (pos < line.Length && char.IsWhiteSpace(line[pos]))
                pos++;
        }
        public char PeekChar()
        {
            return (pos < line.Length) ? line[pos] : char.MinValue;
        }
        public char GetChar()
        {
            return (pos < line.Length) ? line[pos++] : char.MinValue;
        }
        public char GetNextNonSpaceChar(bool skipspacesafter = true)
        {
            while (pos < line.Length && char.IsWhiteSpace(line[pos]))
                pos++;
            char ret = (pos < line.Length) ? line[pos++] : char.MinValue;
            if (skipspacesafter)
            {
                while (pos < line.Length && char.IsWhiteSpace(line[pos]))
                    pos++;
            }
            return ret;
        }
        public bool IsStringMoveOn(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                if (line[pos + i] != s[i])
                    return false;
            }
            pos += s.Length;
            SkipSpace();
            return true;
        }
        public bool IsCharMoveOn(char t, bool skipspaceafter = true)
        {
            if (pos < line.Length && line[pos] == t)
            {
                pos++;
                if (skipspaceafter)
                    SkipSpace();
                return true;
            }
            else
                return false;
        }
        public void BackUp()
        {
            pos--;
        }
        #endregion
        #region WORDs bare
        public int NextQuotedString(char quote, char[] buffer, bool replaceescape = false, bool skipafter = true)
        {
            int bpos = 0;
            while (true)
            {
                if (pos == line.Length || bpos == buffer.Length)  // if reached end of line, or out of buffer, error
                {
                    return -1;
                }
                else if (line[pos] == quote)        // if reached quote, end of string
                {
                    pos++; //skip end quote
                    if (skipafter)
                    {
                        while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                            pos++;
                    }
                    return bpos;
                }
                else if (line[pos] == '\\' && pos < line.Length - 1) // 2 chars min
                {
                    pos++;
                    char esc = line[pos++];     // grab escape and move on
                    if (esc == quote)
                    {
                        buffer[bpos++] = esc;      // place in the character
                    }
                    else if (replaceescape)
                    {
                        switch (esc)
                        {
                            case '\\':
                                buffer[bpos++] = '\\';
                                break;
                            case '/':
                                buffer[bpos++] = '/';
                                break;
                            case 'b':
                                buffer[bpos++] = '\b';
                                break;
                            case 'f':
                                buffer[bpos++] = '\f';
                                break;
                            case 'n':
                                buffer[bpos++] = '\n';
                                break;
                            case 'r':
                                buffer[bpos++] = '\r';
                                break;
                            case 't':
                                buffer[bpos++] = '\t';
                                break;
                            case 'u':
                                if (pos < line.Length - 4)
                                {
                                    int? v1 = line[pos++].ToHex();
                                    int? v2 = line[pos++].ToHex();
                                    int? v3 = line[pos++].ToHex();
                                    int? v4 = line[pos++].ToHex();
                                    if (v1 != null && v2 != null && v3 != null && v4 != null)
                                    {
                                        char c = (char)((v1 << 12) | (v2 << 8) | (v3 << 4) | (v4 << 0));
                                        buffer[bpos++] = c;
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                    buffer[bpos++] = line[pos++];
            }
        }
        #endregion
        #region Numbers and Bools
        static char[] decchars = new char[] { '.', 'e', 'E', '+', '-' };
        public JToken JNextNumber(bool sign, bool skipafter = true)
        {
            ulong ulv = 0;
            bool bigint = false;
            int start = pos;
            while (true)
            {
                if (pos == line.Length)         // if at end, return number got, no need to skip spaces
                {
                    if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string
                        if (System.Numerics.BigInteger.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out System.Numerics.BigInteger bv))
                            return new JToken(JToken.TType.BigInt, sign ? -bv : bv);
                        else
#endif
                        return null;
                    }
                    else if (pos == start)      // no chars read
                        return null;
                    else if (ulv <= long.MaxValue)
                        return new JToken(JToken.TType.Long, sign ? -(long)ulv : (long)ulv);
                    else if (sign)
                        return null;
                    else
                        return new JToken(JToken.TType.ULong, ulv);
                }
                else if (line[pos] < '0' || line[pos] > '9')        // if at end of integer..
                {
                    if (line[pos] == '.' || line[pos] == 'E' || line[pos] == 'e')  // if we have gone into a decimal, collect the string and return
                    {
                        while (pos < line.Length && ((line[pos] >= '0' && line[pos] <= '9') || decchars.Contains(line[pos])))
                            pos++;
                        string part = new string(line, start, pos - start);    // get double string
                        if (skipafter)
                        {
                            while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                                pos++;
                        }
                        if (double.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double dv))
                            return new JToken(JToken.TType.Double, sign ? -dv : dv);
                        else
                            return null;
                    }
                    else if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string
                        if (skipafter)
                        {
                            while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                                pos++;
                        }
                        if (System.Numerics.BigInteger.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out System.Numerics.BigInteger bv))
                            return new JToken(JToken.TType.BigInt,sign ? -bv : bv);
                        else
#endif
                        return null;
                    }
                    else
                    {
                        if (pos == start)   // this means no chars, caused by a - nothing
                            return null;
                        if (skipafter)
                        {
                            while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                                pos++;
                        }
                        if (ulv <= long.MaxValue)
                            return new JToken(JToken.TType.Long, sign ? -(long)ulv : (long)ulv);
                        else if (sign)
                            return null;
                        else
                            return new JToken(JToken.TType.ULong, ulv);
                    }
                }
                else
                {
                    if (ulv > ulong.MaxValue / 10)  // if going to overflow, bit int. collect all ints
                        bigint = true;
                    ulv = (ulv * 10) + (ulong)(line[pos++] - '0');
                }
            }
        }
        static JToken jendarray = new JToken(JToken.TType.EndArray);
        public JToken JNextValue(char[] buffer, bool inarray)
        {
            char next = GetChar();
            switch (next)
            {
                case '{':
                    SkipSpace();
                    return new JObject();
                case '[':
                    SkipSpace();
                    return new JArray();
                case '"':
                    int textlen = NextQuotedString(next, buffer, true);
                    return textlen >= 0 ? new JToken(JToken.TType.String, new string(buffer, 0, textlen)) : null;
                case ']':
                    if (inarray)
                    {
                        SkipSpace();
                        return jendarray;
                    }
                    else
                        return null;
                case '0':       // all positive. JSON does not allow a + at the start (integer fraction exponent)
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    BackUp();
                    return JNextNumber(false);
                case '-':
                    return JNextNumber(true);
                case 't':
                    return IsStringMoveOn("rue") ? new JToken(JToken.TType.Boolean, true) : null;
                case 'f':
                    return IsStringMoveOn("alse") ? new JToken(JToken.TType.Boolean, false) : null;
                case 'n':
                    return IsStringMoveOn("ull") ? new JToken(JToken.TType.Null) : null;
                default:
                    return null;
            }
        }
        public int NextCharBlock(char[] buffer, Func<char, bool> test, bool skipafter = true)
        {
            if (pos < Line.Length)
            {
                int i = 0;
                while (test(line[pos]))         // if we want it
                {
                    if (i >= buffer.Length)     // if we are out of buffer space error
                        return -1;
                    buffer[i++] = line[pos++];
                    if (pos == Line.Length)     // if we are out of data, error
                        return -1;
                }
                if (skipafter)
                {
                    while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                        pos++;
                }
                return i;
            }
            return -1;
        }
        public uint ChecksumCharBlock(Func<char, bool> test, bool skipafter = true)
        {
            uint checksum = 0;
            if (pos < Line.Length)
            {
                while (test(line[pos]))         // if we want it
                {
                    checksum += 7 + (uint)line[pos++] * 23;
                    if (pos == Line.Length)     // if we are out of data, return checksum
                    {
                        return Math.Max(1, checksum);
                    }
                }
                if (skipafter)
                {
                    while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                        pos++;
                }
                return Math.Max(1, checksum);
            }
            else
                return 0;
        }
        #endregion
        private int pos;        // always left after an operation on the next non space char
        private char[] line;
    }
}
namespace QuickJSON.Utils
{
    [System.Diagnostics.DebuggerDisplay("Action {new string(line,pos,line.Length-pos)} : ({new string(line,0,line.Length)})")]
    public class StringParserQuickTextReader : IStringParserQuick
    {
        public StringParserQuickTextReader(TextReader textreader, int chunksize)
        {
            tr = textreader;
            line = new char[chunksize];
        }

        public int Position { get { return pos; } }
        public string Line { get { return new string(line, 0, length); } }
        public bool IsEOL()
        {
            if (pos < length)       // if we have data, its not eol
                return false;
            else
                return !Reload();   // else reload. reload returns true if okay, so its inverted for EOL
        }
        public char GetChar()
        {
            if (pos < length)
                return line[pos++];
            else
            {
                Reload();
                return pos < length ? line[pos++] : char.MinValue;
            }
        }
        public char GetNextNonSpaceChar(bool skipspacesafter = true)
        {
            SkipSpace();
            if (pos < length)
                return line[pos++];
            else
            {
                Reload();
                if (pos < length)
                {
                    char ret = line[pos++];
                    if (skipspacesafter)
                        SkipSpace();
                    return ret;
                }
                else
                    return char.MinValue;
            }
        }
        public char PeekChar()
        {
            if (pos < length)
                return line[pos];
            else
            {
                Reload();
                return pos < length ? line[pos] : char.MinValue;
            }
        }
        public void SkipSpace()
        {
            while (pos < length && char.IsWhiteSpace(line[pos]))        // first skip what we have
                pos++;
            while (pos == length)                   // if we reached end, then reload, and skip, and repeat if required
            {
                if (!Reload())
                    return;
                while (pos < length && char.IsWhiteSpace(line[pos]))
                    pos++;
            }
        }
        public bool IsStringMoveOn(string s)
        {
            if (pos > length - s.Length)            // if not enough to cover s, reload
                Reload();
            if (pos + s.Length > length)            // if not enough for string, not true
                return false;
            for (int i = 0; i < s.Length; i++)
            {
                if (line[pos + i] != s[i])
                    return false;
            }
            pos += s.Length;
            SkipSpace();
            return true;
        }
        public bool IsCharMoveOn(char t, bool skipspaceafter = true)
        {
            if (pos == length)                          // if at end, reload
                Reload();
            if (pos < length && line[pos] == t)
            {
                pos++;
                if (skipspaceafter)
                    SkipSpace();
                return true;
            }
            else
                return false;
        }
        public void BackUp()
        {
            pos--;
        }
        public int NextQuotedString(char quote, char[] buffer, bool replaceescape = false, bool skipafter = true)
        {
            int bpos = 0;
            while (true)
            {
                if (pos == line.Length)                 // if out of chars, reload
                    Reload();
                if (pos == line.Length || bpos == buffer.Length)  // if reached end of line, or out of buffer, error
                {
                    return -1;
                }
                else if (line[pos] == quote)        // if reached quote, end of string
                {
                    pos++; //skip end quote
                    if (skipafter)
                        SkipSpace();
                    return bpos;
                }
                else if (line[pos] == '\\') // 2 chars min
                {
                    if (pos >= length - 6)      // this number left for a good go
                    {
                        Reload();
                        if (length < 1)
                            return -1;
                    }
                    pos++;
                    char esc = line[pos++];     // grab escape and move on
                    if (esc == quote)
                    {
                        buffer[bpos++] = esc;      // place in the character
                    }
                    else if (replaceescape)
                    {
                        switch (esc)
                        {
                            case '\\':
                                buffer[bpos++] = '\\';
                                break;
                            case '/':
                                buffer[bpos++] = '/';
                                break;
                            case 'b':
                                buffer[bpos++] = '\b';
                                break;
                            case 'f':
                                buffer[bpos++] = '\f';
                                break;
                            case 'n':
                                buffer[bpos++] = '\n';
                                break;
                            case 'r':
                                buffer[bpos++] = '\r';
                                break;
                            case 't':
                                buffer[bpos++] = '\t';
                                break;
                            case 'u':
                                if (pos < line.Length - 4)
                                {
                                    int? v1 = line[pos++].ToHex();
                                    int? v2 = line[pos++].ToHex();
                                    int? v3 = line[pos++].ToHex();
                                    int? v4 = line[pos++].ToHex();
                                    if (v1 != null && v2 != null && v3 != null && v4 != null)
                                    {
                                        char c = (char)((v1 << 12) | (v2 << 8) | (v3 << 4) | (v4 << 0));
                                        buffer[bpos++] = c;
                                    }
                                }
                                else
                                    return -1;
                                break;
                        }
                    }
                }
                else
                    buffer[bpos++] = line[pos++];
            }
        }
        static char[] decchars = new char[] { '.', 'e', 'E', '+', '-' };
        public JToken JNextNumber(bool sign, bool skipafter = true)
        {
            ulong ulv = 0;
            bool bigint = false;
            int start = pos;
            bool slid = false;
            while (true)
            {
                if (pos == line.Length)     // if at end of loaded text
                {
                    System.Diagnostics.Debug.Assert(slid == false);         // must not slide more than once
                    Reload(start);              // get more data, keeping d  back to start
                    start = 0;                  // start of number now at 0 in buffer
                    slid = true;
                }
                if (pos == line.Length)         // if at end, return number got
                {
                    if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string
                        if (skipafter)
                            SkipSpace();
                        if (System.Numerics.BigInteger.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out System.Numerics.BigInteger bv))
                            return new JToken(JToken.TType.BigInt, sign ? -bv : bv);
                        else
#endif
                        return null;
                    }
                    else if (pos == start)      // no chars read
                        return null;
                    else if (ulv <= long.MaxValue)
                        return new JToken(JToken.TType.Long, sign ? -(long)ulv : (long)ulv);
                    else if (sign)
                        return null;
                    else
                        return new JToken(JToken.TType.ULong, ulv);
                }
                else if (line[pos] < '0' || line[pos] > '9')        // if at end of integer..
                {
                    if (line[pos] == '.' || line[pos] == 'E' || line[pos] == 'e')  // if we have gone into a decimal, collect the string and return
                    {
                        while (true)
                        {
                            if (pos == line.Length)
                            {
                                System.Diagnostics.Debug.Assert(slid == false);         // must not slide more than once
                                Reload(start);              // get more data, keeping text back to start
                                start = 0;                  // start of number now at 0 in buffer
                                slid = true;
                            }
                            if (pos < line.Length && ((line[pos] >= '0' && line[pos] <= '9') || decchars.Contains(line[pos])))
                                pos++;
                            else
                                break;
                        }
                        string part = new string(line, start, pos - start);    // get double string
                        if (skipafter)
                            SkipSpace();
                        if (double.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double dv))
                            return new JToken(JToken.TType.Double, sign ? -dv : dv);
                        else
                            return null;
                    }
                    else if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string
                        if (skipafter)
                            SkipSpace();
                        if (System.Numerics.BigInteger.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out System.Numerics.BigInteger bv))
                            return new JToken(JToken.TType.BigInt, sign ? -bv : bv);
                        else
#endif
                        return null;
                    }
                    else
                    {
                        if (pos == start)   // this means no chars, caused by a - nothing
                            return null;
                        if (skipafter)
                            SkipSpace();
                        if (ulv <= long.MaxValue)
                            return new JToken(JToken.TType.Long, sign ? -(long)ulv : (long)ulv);
                        else if (sign)
                            return null;
                        else
                            return new JToken(JToken.TType.ULong, ulv);
                    }
                }
                else
                {
                    if (ulv > ulong.MaxValue / 10)  // if going to overflow, bit int. collect all ints
                        bigint = true;
                    ulv = (ulv * 10) + (ulong)(line[pos++] - '0');
                }
            }
        }
        static JToken jendarray = new JToken(JToken.TType.EndArray);
        public JToken JNextValue(char[] buffer, bool inarray)
        {
            char next = GetChar();
            switch (next)
            {
                case '{':
                    SkipSpace();
                    return new JObject();
                case '[':
                    SkipSpace();
                    return new JArray();
                case '"':
                    int textlen = NextQuotedString(next, buffer, true);
                    return textlen >= 0 ? new JToken(JToken.TType.String, new string(buffer, 0, textlen)) : null;
                case ']':
                    if (inarray)
                    {
                        SkipSpace();
                        return jendarray;
                    }
                    else
                        return null;
                case '0':       // all positive. JSON does not allow a + at the start (integer fraction exponent)
                case '1':
                case '2':
                case '3':
                case '4':
                case '5':
                case '6':
                case '7':
                case '8':
                case '9':
                    BackUp();
                    return JNextNumber(false);
                case '-':
                    return JNextNumber(true);
                case 't':
                    return IsStringMoveOn("rue") ? new JToken(JToken.TType.Boolean, true) : null;
                case 'f':
                    return IsStringMoveOn("alse") ? new JToken(JToken.TType.Boolean, false) : null;
                case 'n':
                    return IsStringMoveOn("ull") ? new JToken(JToken.TType.Null) : null;
                default:
                    return null;
            }
        }
        public int NextCharBlock(char[] buffer, Func<char, bool> test, bool skipafter = true)
        {
            int bpos = 0;
            while (true)
            {
                if (pos == line.Length)     // if out of chars, reload
                    Reload();
                if (pos == line.Length || bpos == buffer.Length)  // if reached end of line, or out of buffer, error
                {
                    return -1;
                }
                if (test(line[pos]))        // if ok, store
                {
                    buffer[bpos++] = line[pos++];
                }
                else
                {
                    if (skipafter)
                        SkipSpace();
                    return bpos;
                }
            }
        }
        public uint ChecksumCharBlock(Func<char, bool> test, bool skipafter = true)
        {
            uint checksum = 0;
            while (true)
            {
                if (pos == line.Length)     // if out of chars, reload
                    Reload();
                if (pos == line.Length)  // if reached end of line, checksum
                {
                    return Math.Max(1, checksum);
                }
                if (test(line[pos]))        // if ok, store
                {
                    checksum += 7 + (uint)line[pos++] * 23;
                }
                else
                {
                    if (skipafter)
                        SkipSpace();
                    return Math.Max(1, checksum);
                }
            }
        }
        private bool Reload(int from = -1)          // from means keep at this position onwards, default is pos.
        {
            if (from == -1)
                from = pos;
            if (from < length)                      // if any left, slide
            {
                Array.Copy(line, from, line, 0, length - from);
                length -= from;
                pos -= from;
            }
            else
            {
                pos = length = 0;
            }
            if (length < line.Length)               // if space left, fill
            {
                length += tr.ReadBlock(line, length, line.Length - length);
            }
            return length > 0;
        }
        private TextReader tr;
        private char[] line;
        private int length = 0;
        private int pos = 0;
    }
}

