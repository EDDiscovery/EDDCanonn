/*
 * Copyright 2019-2022 Robbyxp1 @ github.com
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not use this
 * file except in compliance with the License. You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software distributed under
 * the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
 * ANY KIND, either express or implied. See the License for the specific language
 * governing permissions and limitations under the License.
 */

using QuickJSON.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using static QuickJSON.JToken;
using System.Reflection;
namespace QuickJSON
{
    internal static class NamespaceDoc { } // just for documentation purposes
    [System.Diagnostics.DebuggerDisplay("{TokenType} {ToString()}")]
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
        public bool HasValue { get { return Value != null; } }
        public bool IsString { get { return TokenType == TType.String; } }
        public bool IsInt { get { return TokenType == TType.Long || TokenType == TType.ULong || TokenType == TType.BigInt; } }
        public bool IsLong { get { return TokenType == TType.Long; } }
        public bool IsBigInt { get { return TokenType == TType.BigInt; } }
        public bool IsULong { get { return TokenType == TType.ULong; } }
        public bool IsDouble { get { return TokenType == TType.Double; } }
        public bool IsBool { get { return TokenType == TType.Boolean; } }
        public bool IsArray { get { return TokenType == TType.Array; } }
        public bool IsObject { get { return TokenType == TType.Object; } }
        public bool IsNull { get { return TokenType == TType.Null; } }
        public bool IsProperty { get { return Name != null; } }                     // indicates that the object is an object property
        public bool IsEndObject { get { return TokenType == TType.EndObject; } }    // only seen for TokenReader
        public bool IsEndArray { get { return TokenType == TType.EndArray; } }      // only seen for TokenReader
        public bool IsInError { get { return TokenType == TType.Error; } }          // only seen for FromObject when asking for error return
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

        public JToken(TType tokentype, Object value = null)
        {
            TokenType = tokentype; Value = value;
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
                System.Diagnostics.Debug.WriteLine("Failed to serialise type " + obj.GetType().Name);
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
        public bool ValueEquals(Object value)               // NOTE not doing float/double due to approximations. Don't override Equals.
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
        #region Operators and functions
        public virtual JToken this[object key] { get { return null; } set { throw new NotImplementedException(); } }
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
        public virtual void Clear() { throw new NotImplementedException(); }    // clear all children
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
        public void Add(JToken o) { Elements.Add(o); }
        public void AddRange(IEnumerable<JToken> o) { Elements.AddRange(o); }
        public void RemoveAt(int index) { Elements.RemoveAt(index); }
        public void RemoveRange(int index, int count) { Elements.RemoveRange(index, count); }
        public override void Clear() { Elements.Clear(); }
        public JToken Find(System.Predicate<JToken> predicate) { return Elements.Find(predicate); }
        public T Find<T>(System.Predicate<JToken> predicate) { Object r = Elements.Find(predicate); return (T)r; }
        public List<string> String() { return Elements.ConvertAll<string>((o) => { return o.TokenType == TType.String ? ((string)o.Value) : null; }); }
        public List<int> Int() { return Elements.ConvertAll<int>((o) => { return (int)((long)o.Value); }); }
        public List<long> Long() { return Elements.ConvertAll<long>((o) => { return ((long)o.Value); }); }
        public List<double> Double() { return Elements.ConvertAll<double>((o) => { return ((double)o.Value); }); }
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
    public sealed class JsonIgnoreAttribute : Attribute
    {
    }
    public sealed class JsonNameAttribute : Attribute
    {
        public string[] Names { get; set; }
        public JsonNameAttribute(params string[] names) { Names = names; }
    }
    public partial class JToken
    {
        public static JToken FromObject(Object obj)
        {
            return FromObject(obj, false);
        }
        public static JToken FromObject(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256,
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
        {
            Stack<Object> objectlist = new Stack<object>();
            var r = FromObjectInt(obj, ignoreunserialisable, ignored, objectlist, 0, maxrecursiondepth, membersearchflags);
            System.Diagnostics.Debug.Assert(objectlist.Count == 0);
            return r.IsInError ? null : r;
        }
        public static JToken FromObjectWithError(Object obj, bool ignoreunserialisable, Type[] ignored = null, int maxrecursiondepth = 256,
            System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static)
        {
            Stack<Object> objectlist = new Stack<object>();
            var r = FromObjectInt(obj, ignoreunserialisable, ignored, objectlist, 0, maxrecursiondepth, membersearchflags);
            System.Diagnostics.Debug.Assert(objectlist.Count == 0);
            return r;
        }
        private static JToken FromObjectInt(Object o, bool ignoreunserialisable, Type[] ignored, Stack<Object> objectlist, int lvl, int maxrecursiondepth, System.Reflection.BindingFlags membersearchflags)
        {
            if (lvl >= maxrecursiondepth)
                return new JToken();        // returns NULL
            Type tt = o.GetType();
            if (tt.IsArray)
            {
                Array b = o as Array;
                JArray outarray = new JArray();
                objectlist.Push(o);
                for (int i = 0; i < b.Length; i++)
                {
                    object oa = b.GetValue(i);
                    if (objectlist.Contains(oa))        // self reference
                    {
                        objectlist.Pop();
                        return new JToken(TType.Error, "Self Reference in Array");
                    }
                    JToken inner = FromObjectInt(oa, ignoreunserialisable, ignored, objectlist, lvl + 1, maxrecursiondepth, membersearchflags);
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
            else if (typeof(System.Collections.IList).IsAssignableFrom(tt))
            {
                var ilist = o as System.Collections.IList;
                JArray outarray = new JArray();
                objectlist.Push(o);
                foreach (var oa in ilist)
                {
                    if (objectlist.Contains(oa))        // self reference
                    {
                        objectlist.Pop();
                        return new JToken(TType.Error, "Self Reference in IList");
                    }
                    JToken inner = FromObjectInt(oa, ignoreunserialisable, ignored, objectlist, lvl + 1, maxrecursiondepth, membersearchflags);
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
            else if (typeof(System.Collections.IDictionary).IsAssignableFrom(tt))       // if its a Dictionary<x,y> then expect a set of objects
            {
                System.Collections.IDictionary idict = o as System.Collections.IDictionary;
                JObject outobj = new JObject();
                objectlist.Push(o);
                foreach (dynamic kvp in idict)      // need dynamic since don't know the types of Value or Key
                {
                    if (objectlist.Contains(kvp.Value))   // self reference
                    {
                        if (ignoreunserialisable)       // if ignoring this, just continue with the next one
                            continue;
                        objectlist.Pop();
                        return new JToken(TType.Error, "Self Reference in IDictionary");
                    }
                    JToken inner = FromObjectInt(kvp.Value, ignoreunserialisable, ignored, objectlist, lvl + 1, maxrecursiondepth, membersearchflags);
                    if (inner.IsInError)      // used as an error type
                    {
                        objectlist.Pop();
                        return inner;
                    }
                    if (kvp.Key is DateTime)               // handle date time specially, use zulu format
                    {
                        DateTime t = (DateTime)kvp.Key;
                        outobj[t.ToStringZulu()] = inner;
                    }
                    else
                        outobj[kvp.Key.ToString()] = inner;
                }
                objectlist.Pop();
                return outobj;
            }
            else if ((tt.IsClass && tt != typeof(string)) ||                           // if class, but not string (handled in CreateToken)
                      (tt.IsValueType && !tt.IsPrimitive && !tt.IsEnum && tt != typeof(DateTime))     // if value type, not primitive, not enum, its a structure. Not datetime (handled in CreateToken)
                      )
            {
                JObject outobj = new JObject();
                var allmembers = tt.GetMembers(membersearchflags);
                objectlist.Push(o);
                foreach (var mi in allmembers)
                {
                    Type innertype = null;
                    if (mi.MemberType == System.Reflection.MemberTypes.Property)
                        innertype = ((System.Reflection.PropertyInfo)mi).PropertyType;
                    else if (mi.MemberType == System.Reflection.MemberTypes.Field)
                    {
                        var fi = (System.Reflection.FieldInfo)mi;
                        innertype = fi.FieldType;
                    }
                    else
                        continue;
                    if (ignored != null && Array.IndexOf(ignored, innertype) >= 0)
                        continue;
                    var ca = mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false);
                    if (ca.Length > 0)                                              // ignore any ones with JsonIgnore on it.
                        continue;
                    string attrname = mi.Name;
                    var rename = mi.GetCustomAttributes(typeof(JsonNameAttribute), false);
                    if (rename.Length == 1)                                         // any ones with a rename, use that name
                    {
                        dynamic attr = rename[0];                                   // dynamic since compiler does not know rename type
                        attrname = attr.Names[0];                                   // only first entry is used for FromObject
                    }
                    Object innervalue = null;
                    if (mi.MemberType == System.Reflection.MemberTypes.Property)
                        innervalue = ((System.Reflection.PropertyInfo)mi).GetValue(o);
                    else
                        innervalue = ((System.Reflection.FieldInfo)mi).GetValue(o);
                    if (innervalue != null)
                    {
                        if (objectlist.Contains(innervalue))        // self reference
                        {
                            if (ignoreunserialisable)               // if ignoring this, just continue with the next one
                                continue;
                            objectlist.Pop();
                            return new JToken(TType.Error, "Self Reference by " + tt.Name + ":" + mi.Name);
                        }
                        var token = FromObjectInt(innervalue, ignoreunserialisable, ignored, objectlist, lvl + 1, maxrecursiondepth, membersearchflags);     // may return End Object if not serializable
                        if (token.IsInError)
                        {
                            if (!ignoreunserialisable)
                            {
                                objectlist.Pop();
                                return token;
                            }
                        }
                        else
                            outobj[attrname] = token;
                    }
                    else
                    {
                        outobj[attrname] = JToken.Null();        // its null so its a JNull
                    }
                }
                objectlist.Pop();
                return outobj;
            }
            else
            {
                var r = JToken.CreateToken(o, false);        // return token or null indicating unserializable
                return r ?? new JToken(TType.Error, "Unserializable " + tt.Name);
            }
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
        public static string MultiStr(this JObject token, string[] propertynameslist, string def = "")       // multiple lookup in Object of names
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
#if JSONBIGINT
        public static System.Numerics.BigInteger BigInteger(this JToken token, System.Numerics.BigInteger def)
        {
            if (token == null)
                return def;
            else if (token.TokenType == JToken.TType.ULong)
                return (ulong)token.Value;
            else if (token.IsLong && (long)token.Value >= 0)
                return (ulong)(long)token.Value;
            else if (token.TokenType == JToken.TType.BigInt)
                return (System.Numerics.BigInteger)token.Value;
            else
                return def;
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
        public bool Contains(string name) { return Objects.ContainsKey(name); }
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
        public override int Count { get { return Objects.Count; } }
        public void Add(string key, JToken value) { this[key] = value; }
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
                JToken o = DecodeValue(parser, textbuffer, false);       // grab new value, not array end
                if (o == null)
                {
                    return ParseError(parser, "No Obj/Array", flags, out error);
                }
                else if (o.TokenType == TType.Array)
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
                    while (true)
                    {
                        char next = parser.GetChar();
                        if (next == '}')    // end object
                        {
                            parser.SkipSpace();
                            if (comma == true && (flags & ParseOptions.AllowTrailingCommas) == 0)
                            {
                                return ParseError(parser, "Comma", flags, out error);
                            }
                            else
                            {
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
                            int textlen = parser.NextQuotedString(next, textbuffer, true);
                            if (textlen < 1 || (comma == false && curobject.Count > 0))
                            {
                                return ParseError(parser, "Object missing property name", flags, out error);
                            }
                            else if (!parser.IsCharMoveOn(':'))
                            {
                                return ParseError(parser, "Object missing : after property name", flags, out error);
                            }
                            else
                            {
                                string name = new string(textbuffer, 0, textlen);
                                JToken o = DecodeValue(parser, textbuffer, false);      // get value
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
                                    o.Name = name;                          // object gets the name, indicating its a property
                                    curobject[name] = o;                    // assign to dictionary
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
                        JToken o = DecodeValue(parser, textbuffer, true);       // grab new value
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
                                return ParseError(parser, "Comma", flags, out error);
                            }
                            else
                            {
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
                            return ParseError(parser, "Comma", flags, out error);
                        }
                        else
                        {
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
        static JToken jendarray = new JToken(TType.EndArray);
        static private JToken DecodeValue(IStringParserQuick parser, char[] textbuffer, bool inarray)
        {
            char next = parser.GetChar();
            switch (next)
            {
                case '{':
                    parser.SkipSpace();
                    return new JObject();
                case '[':
                    parser.SkipSpace();
                    return new JArray();
                case '"':
                    int textlen = parser.NextQuotedString(next, textbuffer, true);
                    return textlen >= 0 ? new JToken(TType.String, new string(textbuffer, 0, textlen)) : null;
                case ']':
                    if (inarray)
                    {
                        parser.SkipSpace();
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
                    parser.BackUp();
                    return parser.JNextNumber(false);
                case '-':
                    return parser.JNextNumber(true);
                case 't':
                    return parser.IsStringMoveOn("rue") ? new JToken(TType.Boolean, true) : null;
                case 'f':
                    return parser.IsStringMoveOn("alse") ? new JToken(TType.Boolean, false) : null;
                case 'n':
                    return parser.IsStringMoveOn("ull") ? new JToken(TType.Null) : null;
                default:
                    return null;
            }
        }
        static private string GenErrorString(IStringParserQuick parser, string text)
        {
            string error = "JSON " + text + " at " + parser.Position + " " + parser.Line.Substring(0, parser.Position) + " <ERROR> "
                            + parser.Line.Substring(parser.Position);
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
        public static IEnumerable<JToken> ParseToken(string text, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            using (StringReader sr = new StringReader(text))         // read directly from file..
            {
                var parser = new StringParserQuickTextReader(sr, 16384);
                return ParseToken(parser, flags, charbufsize);
            }
        }
        public static IEnumerable<JToken> ParseToken(TextReader tr, JToken.ParseOptions flags = JToken.ParseOptions.None, int charbufsize = 16384)
        {
            var parser = new StringParserQuickTextReader(tr, 16384);
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
        private static IEnumerable<JToken> ParseTokenInt(IStringParserQuick parser, JToken.ParseOptions flags = JToken.ParseOptions.None, int maxstringlen = 16384)
        {
            char[] textbuffer = new char[maxstringlen];
            JToken[] stack = new JToken[256];
            int sptr = 0;
            bool comma = false;
            JArray curarray = null;
            JObject curobject = null;
            {
                parser.SkipSpace();
                JToken o = DecodeValue(parser, textbuffer, false);       // grab new value, not array end
                if (o == null)
                {
                    throw new TokenException(GenErrorString(parser, "No Obj/Array"));
                }
                else if (o.TokenType == JToken.TType.Array)
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
                                yield return new JToken(JToken.TType.EndObject);
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
                            int textlen = parser.NextQuotedString(next, textbuffer, true);
                            if (textlen < 1 || (comma == false && curobject.Count > 0) || !parser.IsCharMoveOn(':'))
                            {
                                throw new TokenException(GenErrorString(parser, "Object missing property name"));
                            }
                            else
                            {
                                string name = new string(textbuffer, 0, textlen);
                                JToken o = DecodeValue(parser, textbuffer, false);      // get value
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
                                    o.Name = name;
                                    yield return o;
                                    if (o.TokenType == JToken.TType.Array) // if array, we need to change to this as controlling object on top of stack
                                    {
                                        if (sptr == stack.Length - 1)
                                        {
                                            throw new TokenException(GenErrorString(parser, "Stack overflow"));
                                        }
                                        stack[++sptr] = o;          // push this one onto stack
                                        curarray = o as JArray;                 // this is now the current object
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
                        JToken o = DecodeValue(parser, textbuffer, true);       // grab new value
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
                                yield return new JToken(JToken.TType.EndArray);
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
                    else
                    {
                        ja.Add(i);
                    }
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
        public static T ToObjectQ<T>(this JToken token)            // quick version, with checkcustomattr off
        {
            return ToObject<T>(token, false, false);
        }
        public static T ToObject<T>(this JToken token, bool ignoretypeerrors = false, bool checkcustomattr = true)
        {
            Type tt = typeof(T);
            try
            {
                Object ret = token.ToObject(tt, ignoretypeerrors, checkcustomattr);        // paranoia, since there are a lot of dynamics, trap any exceptions
                if (ret is ToObjectError)
                {
                    System.Diagnostics.Debug.WriteLine("To Object error:" + ((ToObjectError)ret).ErrorString + ":" + ((ToObjectError)ret).PropertyName);
                    return default(T);
                }
                else if (ret != null)      // or null
                    return (T)ret;          // must by definition have returned tt.
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception JSON ToObject " + ex.Message + " " + ex.StackTrace);
            }
            return default(T);
        }
        public static Object ToObjectProtected(this JToken token, Type converttype, bool ignoretypeerrors, bool checkcustomattr,
                                    System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                                    Object initialobject = null)
        {
            try
            {
                return ToObject(token, converttype, ignoretypeerrors, checkcustomattr, membersearchflags, initialobject);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Exception JSON ToObject " + ex.Message + " " + ex.StackTrace);
            }
            return null;

        }
        public class ToObjectError
        {
            public string ErrorString;
            public string PropertyName;
            public ToObjectError(string s) { ErrorString = s; PropertyName = ""; }
        };
        public static Object ToObject(this JToken token, Type converttype, bool ignoretypeerrors, bool checkcustomattr,
                System.Reflection.BindingFlags membersearchflags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static,
                Object initialobject = null)
        {
            if (token == null)
            {
                return null;
            }
            else if (token.IsArray)
            {
                JArray jarray = (JArray)token;
                if (converttype.IsArray)
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype, token.Count);   // dynamic holder for instance of array[]
                    for (int i = 0; i < token.Count; i++)
                    {
                        Object ret = ToObject(token[i], converttype.GetElementType(), ignoretypeerrors, checkcustomattr);      // get the underlying element, must match array element type
                        if (ret != null && ret.GetType() == typeof(ToObjectError))      // arrays must be full, any errors means an error
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                            return ret;
                        }
                        else
                        {
                            dynamic d = converttype.GetElementType().ChangeTo(ret);
                            instance[i] = d;
                        }
                    }
                    return instance;
                }
                else if (typeof(System.Collections.IList).IsAssignableFrom(converttype))
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the List
                    var types = converttype.GetGenericArguments();
                    for (int i = 0; i < token.Count; i++)
                    {
                        Object ret = ToObject(token[i], types[0], ignoretypeerrors, checkcustomattr);      // get the underlying element, must match types[0] which is list type
                        if (ret != null && ret.GetType() == typeof(ToObjectError))  // lists must be full, any errors are errors
                        {
                            ((ToObjectError)ret).PropertyName = converttype.Name + "." + i.ToString() + "." + ((ToObjectError)ret).PropertyName;
                            return ret;
                        }
                        else
                        {
                            dynamic d = types[0].ChangeTo(ret);
                            instance.Add(d);
                        }
                    }
                    return instance;
                }
                else
                    return new ToObjectError($"JSONToObject: Not array {converttype.Name}");
            }
            else if (token.TokenType == JToken.TType.Object)                   // objects are best efforts.. fills in as many fields as possible
            {
                if (typeof(System.Collections.IDictionary).IsAssignableFrom(converttype))       // if its a Dictionary<x,y> then expect a set of objects
                {
                    dynamic instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras
                    var types = converttype.GetGenericArguments();
                    foreach (var kvp in (JObject)token)
                    {
                        Object ret = ToObject(kvp.Value, types[1], ignoretypeerrors, checkcustomattr);        // get the value as the dictionary type - it must match type or it get OE
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
                            dynamic k = types[0].ChangeTo(kvp.Key);             // convert kvp.Key, to the dictionary type. May except if not compatible
                            dynamic d = types[1].ChangeTo(ret);                 // convert value to the data type.
                            instance[k] = d;
                        }
                    }
                    return instance;
                }
                else if (converttype.IsClass ||      // if class
                         (converttype.IsValueType && !converttype.IsPrimitive && !converttype.IsEnum && converttype != typeof(DateTime)))   // or struct, but not datetime (handled below)
                {
                    var instance = initialobject != null ? initialobject : Activator.CreateInstance(converttype);        // create the class, so class must has a constructor with no paras
                    System.Reflection.MemberInfo[] fi = converttype.GetFields(membersearchflags);        // get field list..
                    object[] finames = null;        // alternate names of all fields, loaded only if checkcustom is on,  Object array of string[]
                    System.Reflection.MemberInfo[] pi = null;   // lazy load the property list
                    object[] pinames = null;       // alternate names of all the properties,  Object array of string[]
                    if (checkcustomattr)
                    {
                        finames = new object[fi.Length];
                        for (int i = 0; i < fi.Length; i++)     // thru all the fi fields, see if they have a custom attribute of JsonNameAttribute, if so, pick up the names list
                        {
                            var attrlist = fi[i].GetCustomAttributes(typeof(JsonNameAttribute), false);
                            if (attrlist.Length == 1)
                                finames[i] = ((dynamic)attrlist[0]).Names;
                            else
                                finames[i] = new string[] { fi[i].Name };
                        }
                    }
                    foreach (var kvp in (JObject)token)
                    {
                        System.Reflection.MemberInfo mi = null;
                        if (finames == null)
                        {
                            var fipos = System.Array.FindIndex(fi, x => x.Name.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase));     // straight name lookup
                            if (fipos >= 0)
                                mi = fi[fipos];
                        }
                        else
                        {
                            for (int fipos = 0; fipos < finames.Length; fipos++)
                            {
                                if (System.Array.IndexOf((string[])finames[fipos], kvp.Key) >= 0)
                                {
                                    mi = fi[fipos];
                                    break;
                                }
                            }
                        }
                        if (mi == null)
                        {
                            if (pi == null)     // lazy load pick up, only load these if fields not found
                            {
                                pi = converttype.GetProperties(membersearchflags);
                                if (checkcustomattr)
                                {
                                    pinames = new object[pi.Length];
                                    for (int i = 0; i < pi.Length; i++)
                                    {
                                        var attrlist = pi[i].GetCustomAttributes(typeof(JsonNameAttribute), false);
                                        if (attrlist.Length == 1)
                                            pinames[i] = ((dynamic)attrlist[0]).Names;
                                        else
                                            pinames[i] = new string[] { pi[i].Name };
                                    }
                                }
                            }
                            if (pinames == null)
                            {
                                var pipos = System.Array.FindIndex(pi, x => x.Name.Equals(kvp.Key, StringComparison.InvariantCultureIgnoreCase));
                                if (pipos >= 0)
                                    mi = pi[pipos];
                            }
                            else
                            {
                                for (int pipos = 0; pipos < pinames.Length; pipos++)
                                {
                                    if (System.Array.IndexOf((string[])pinames[pipos], kvp.Key) >= 0)
                                    {
                                        mi = pi[pipos];
                                        break;
                                    }
                                }
                            }
                        }
                        if (mi != null)                                   // if we found a class member
                        {
                            var ca = checkcustomattr ? mi.GetCustomAttributes(typeof(JsonIgnoreAttribute), false) : null;
                            if (ca == null || ca.Length == 0)                                              // ignore any ones with JsonIgnore on it.
                            {
                                Type otype = mi.FieldPropertyType();
                                if (otype != null)                          // and its a field or property
                                {
                                    Object ret = ToObject(kvp.Value, otype, ignoretypeerrors, checkcustomattr);    // get the value - must match otype.. ret may be zero for ? types
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
                                        if (!mi.SetValue(instance, ret))         // and set. Set will fail if the property is get only
                                        {
                                            if (ignoretypeerrors)
                                            {
                                                System.Diagnostics.Debug.WriteLine("Ignoring cannot set value on property " + mi.Name);
                                            }
                                            else
                                            {
                                                return new ToObjectError("Cannot set value on property " + mi.Name);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                        }
                    }
                    return instance;
                }
                else
                    return new ToObjectError($"JSONToObject: Not class {converttype.Name}");
            }
            else
            {
                string name = converttype.Name;                              // compare by name quicker than is
                if (name.Equals("Nullable`1"))                      // nullable types
                {
                    if (token.IsNull)
                        return null;
                    name = converttype.GenericTypeArguments[0].Name;         // get underlying type..
                }
                if (name.Equals("String"))                          // copies of QuickJSON explicit operators in QuickJSON.cs
                {
                    if (token.IsNull)
                        return null;
                    else if (token.IsString)
                        return token.Value;
                }
                else if (name.Equals("Int32"))
                {
                    if (token.TokenType == TType.Long)                  // it won't be a ulong/bigint since that would be too big for an int
                        return (int)(long)token.Value;
                    else if (token.TokenType == TType.Double)           // doubles get trunced.. as per previous system
                        return (int)(double)token.Value;
                }
                else if (name.Equals("Int64"))
                {
                    if (token.TokenType == TType.Long)
                        return token.Value;
                    else if (token.TokenType == TType.Double)
                        return (long)(double)token.Value;
                }
                else if (name.Equals("Boolean"))
                {
                    if (token.TokenType == TType.Boolean)
                        return (bool)token.Value;
                    else if (token.TokenType == TType.Long)
                        return (long)token.Value != 0;
                }
                else if (name.Equals("Double"))
                {
                    if (token.TokenType == TType.Long)
                        return (double)(long)token.Value;
                    else if (token.TokenType == TType.ULong)
                        return (double)(ulong)token.Value;
#if JSONBIGINT
                    else if (token.TokenType == TType.BigInt)
                        return (double)(System.Numerics.BigInteger)token.Value;
#endif
                    else if (token.TokenType == TType.Double)
                        return (double)token.Value;
                }
                else if (name.Equals("Single"))
                {
                    if (token.TokenType == TType.Long)
                        return (float)(long)token.Value;
                    else if (token.TokenType == TType.ULong)
                        return (float)(ulong)token.Value;
#if JSONBIGINT
                    else if (token.TokenType == TType.BigInt)
                        return (float)(System.Numerics.BigInteger)token.Value;
#endif
                    else if (token.TokenType == TType.Double)
                        return (float)(double)token.Value;
                }
                else if (name.Equals("UInt32"))
                {
                    if (token.TokenType == TType.Long && (long)token.Value >= 0)
                        return (uint)(long)token.Value;
                    else if (token.TokenType == TType.Double && (double)token.Value >= 0)
                        return (uint)(double)token.Value;
                }
                else if (name.Equals("UInt64"))
                {
                    if (token.TokenType == TType.ULong)
                        return (ulong)token.Value;
                    else if (token.TokenType == TType.Long && (long)token.Value >= 0)
                        return (ulong)(long)token.Value;
                    else if (token.TokenType == TType.Double && (double)token.Value >= 0)
                        return (ulong)(double)token.Value;
                }
                else if (name.Equals("DateTime"))
                {
                    DateTime? dt = token.DateTime(System.Globalization.CultureInfo.InvariantCulture);
                    if (dt != null)
                        return dt;
                }
                else if (converttype.IsEnum)
                {
                    if (!token.IsString)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSONToObject: Enum Token is not string for {converttype.Name}");
                        return new ToObjectError($"JSONToObject: Enum Token is not string for {converttype.Name}");
                    }
                    try
                    {
                        Object p = Enum.Parse(converttype, token.Str(), true);
                        return Convert.ChangeType(p, converttype);
                    }
                    catch
                    {
                        System.Diagnostics.Debug.WriteLine($"JSONToObject: Unrecognised value '{token.Str()}' for enum {converttype.Name}");
                        return new ToObjectError($"JSONToObject: Unrecognised value '{token.Str()}' for enum {converttype.Name}");
                    }
                }
                return new ToObjectError("JSONToObject: Bad Conversion " + token.TokenType + " to " + converttype.Name);
            }
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
        public static string ToString(JToken token, string prepad, string postpad, string oapad, bool stringliterals)
        {
            if (token.TokenType == TType.String)
            {
                if (stringliterals)       // used if your extracting the value of the data as a string, and not turning it back to json.
                    return prepad + (string)token.Value + postpad;
                else
                    return prepad + "\"" + ((string)token.Value).EscapeControlCharsFull() + "\"" + postpad;
            }
            else if (token.TokenType == TType.Double)
            {
                string sd = ((double)token.Value).ToStringInvariant("R");       // round trip it - use 'R' since minvalue won't work very well. See https://docs.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings#RFormatString
                if (!(sd.Contains("E") || sd.Contains(".")))                // needs something to indicate its a double, and if it does not have a dot or E, it needs a .0
                    sd += ".0";
                return prepad + sd + postpad;
            }
            else if (token.TokenType == TType.Long)
                return prepad + ((long)token.Value).ToStringInvariant() + postpad;
            else if (token.TokenType == TType.ULong)
                return prepad + ((ulong)token.Value).ToStringInvariant() + postpad;
#if JSONBIGINT
            else if (token.TokenType == TType.BigInt)
                return prepad + ((System.Numerics.BigInteger)token.Value).ToString(System.Globalization.CultureInfo.InvariantCulture) + postpad;
#endif
            else if (token.TokenType == TType.Boolean)
                return prepad + ((bool)token.Value).ToString().ToLower() + postpad;
            else if (token.TokenType == TType.Null)
                return prepad + "null" + postpad;
            else if (token.TokenType == TType.Array)
            {
                string s = prepad + "[" + postpad;
                string arrpad = prepad + oapad;
                JArray ja = token as JArray;
                for (int i = 0; i < ja.Count; i++)
                {
                    bool notlast = i < ja.Count - 1;
                    s += ToString(ja[i], arrpad, postpad, oapad, stringliterals);
                    if (notlast)
                    {
                        s = s.Substring(0, s.Length - postpad.Length) + "," + postpad;
                    }
                }
                s += prepad + "]" + postpad;
                return s;
            }
            else if (token.TokenType == TType.Object)
            {
                string s = prepad + "{" + postpad;
                string objpad = prepad + oapad;
                int i = 0;
                JObject jo = ((JObject)token);
                foreach (var e in jo)
                {
                    bool notlast = i++ < jo.Count - 1;
                    if (e.Value is JObject || e.Value is JArray)
                    {
                        if (stringliterals)
                            s += objpad + e.Key.EscapeControlCharsFull() + ":" + postpad;
                        else
                            s += objpad + "\"" + e.Key.EscapeControlCharsFull() + "\":" + postpad;
                        s += ToString(e.Value, objpad, postpad, oapad, stringliterals);
                        if (notlast)
                        {
                            s = s.Substring(0, s.Length - postpad.Length) + "," + postpad;
                        }
                    }
                    else
                    {
                        if (stringliterals)
                            s += objpad + e.Key.EscapeControlCharsFull() + ":";
                        else
                            s += objpad + "\"" + e.Key.EscapeControlCharsFull() + "\":";
                        s += ToString(e.Value, "", "", oapad, stringliterals) + (notlast ? "," : "") + postpad;
                    }
                }
                s += prepad + "}" + postpad;
                return s;
            }
            else if (token.TokenType == TType.Error)
                return "ERROR:" + (string)token.Value;
            else
                return null;
        }
    }
}
#pragma warning disable 1591
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
        public static Object ChangeTo(this Type type, Object value)     // this extends ChangeType to handle nullables.
        {
            Type underlyingtype = Nullable.GetUnderlyingType(type);     // test if its a nullable type (double?)
            if (underlyingtype != null)
            {
                if (value == null)
                    return null;
                else
                    return Convert.ChangeType(value, underlyingtype);
            }
            else
            {
                return Convert.ChangeType(value, type);       // convert to element type, which should work since we checked compatibility
            }
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
        public static string NewLine(this string s)
        {
            if (s.Length > 0 && !s.EndsWith(Environment.NewLine))
                s += Environment.NewLine;
            return s;
        }
        public static string AlwaysQuoteString(this string obj)
        {
            return "\"" + obj.Replace("\"", "\\\"") + "\"";
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
        char PeekChar();
        bool IsStringMoveOn(string s);
        bool IsCharMoveOn(char t, bool skipspace = true);
        void BackUp();
        int NextQuotedString(char quote, char[] buffer, bool replaceescape = false);
        JToken JNextNumber(bool sign);
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
            return (pos < line.Length) ? line[pos] : ' ';
        }
        public char GetChar()
        {
            return (pos < line.Length) ? line[pos++] : ' ';
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
        public bool IsCharMoveOn(char t, bool skipspace = true)
        {
            if (pos < line.Length && line[pos] == t)
            {
                pos++;
                if (skipspace)
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
        public int NextQuotedString(char quote, char[] buffer, bool replaceescape = false)
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
                    while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                        pos++;
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
        public JToken JNextNumber(bool sign)     // must be on a digit
        {
            ulong ulv = 0;
            bool bigint = false;
            int start = pos;
            while (true)
            {
                if (pos == line.Length)         // if at end, return number got
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
                        while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                            pos++;
                        if (double.TryParse(part, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double dv))
                            return new JToken(JToken.TType.Double, sign ? -dv : dv);
                        else
                            return null;
                    }
                    else if (bigint)
                    {
#if JSONBIGINT
                        string part = new string(line, start, pos - start);    // get double string
                        while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                            pos++;
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
                        while (pos < line.Length && char.IsWhiteSpace(line[pos]))   // skip spaces
                            pos++;
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
        public bool IsCharMoveOn(char t, bool skipspace = true)
        {
            if (pos == length)                          // if at end, reload
                Reload();
            if (pos < length && line[pos] == t)
            {
                pos++;
                if (skipspace)
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
        public int NextQuotedString(char quote, char[] buffer, bool replaceescape = false)
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
        public JToken JNextNumber(bool sign)     // must be on a digit
        {
            ulong ulv = 0;
            bool bigint = false;
            int start = pos;
            bool slid = false;
            while (true)
            {
                if (pos == line.Length)
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

