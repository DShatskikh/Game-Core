// https://github.com/mhallin/SimpleJSON.NET
// SimpleJSON.NET is licensed under a 3-Clause BSD License:
//
// Copyright (c) 2011, Boldai AB All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
// - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and
//   the following disclaimer in the documentation and/or other materials provided with the distribution.
// - Neither the name of the <organization> nor the names of its contributors may be used to endorse or
//   promote products derived from this software without specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
// ARE DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
// BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY
// OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
// NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Io.AppMetrica.Editor.Features.Utils {
    internal enum JObjectKind {
        Object,
        Array,
        String,
        Number,
        Boolean,
        Null
    }

    internal enum IntegerSize {
        UInt64,
        Int64,
        UInt32,
        Int32,
        UInt16,
        Int16,
        UInt8,
        Int8,
    }

    internal enum FloatSize {
        Double,
        Single
    }

    internal class JObject {
        public JObjectKind Kind { get; private set; }

        public Dictionary<string, JObject> ObjectValue { get; private set; }
        public List<JObject> ArrayValue { get; private set; }
        public string StringValue { get; private set; }
        public bool BooleanValue { get; private set; }

        public int Count {
            get {
                return Kind == JObjectKind.Array ? ArrayValue.Count
                     : Kind == JObjectKind.Object ? ObjectValue.Count
                     : 0;
            }
        }

        public double DoubleValue { get; private set; }
        public float FloatValue { get; private set; }
        public ulong ULongValue { get; private set; }
        public long LongValue { get; private set; }
        public uint UIntValue { get; private set; }
        public int IntValue { get; private set; }
        public ushort UShortValue { get; private set; }
        public short ShortValue { get; private set; }
        public byte ByteValue { get; private set; }
        public sbyte SByteValue { get; private set; }

        public bool IsNegative { get; private set; }
        public bool IsFractional { get; private set; }
        public IntegerSize MinInteger { get; private set; }
        public FloatSize MinFloat { get; private set; }

        public JObject this[string key] { get { return ObjectValue[key]; } }
        public JObject this[int key] { get { return ArrayValue[key]; } }

        public static explicit operator string(JObject obj) { return obj.StringValue; }
        public static explicit operator bool(JObject obj) { return obj.BooleanValue; }
        public static explicit operator double(JObject obj) { return obj.DoubleValue; }
        public static explicit operator float(JObject obj) { return obj.FloatValue; }
        public static explicit operator ulong(JObject obj) { return obj.ULongValue; }
        public static explicit operator long(JObject obj) { return obj.LongValue; }
        public static explicit operator uint(JObject obj) { return obj.UIntValue; }
        public static explicit operator int(JObject obj) { return obj.IntValue; }
        public static explicit operator ushort(JObject obj) { return obj.UShortValue; }
        public static explicit operator short(JObject obj) { return obj.ShortValue; }
        public static explicit operator byte(JObject obj) { return obj.ByteValue; }
        public static explicit operator sbyte(JObject obj) { return obj.SByteValue; }

        public static JObject CreateString(string str) { return new JObject(str); }
        public static JObject CreateBoolean(bool b) { return new JObject(b); }
        public static JObject CreateNull() { return new JObject(); }
        public static JObject CreateNumber(bool isNegative, bool isFractional, bool negativeExponent, ulong integerPart, ulong fractionalPart, int fractionalPartLength, ulong exponent) { return new JObject(isNegative, isFractional, negativeExponent, integerPart, fractionalPart, fractionalPartLength, exponent); }
        public static JObject CreateArray(List<JObject> list) { return new JObject(list); }
        public static JObject CreateObject(Dictionary<string, JObject> dict) { return new JObject(dict); }

        private JObject(string str) {
            Kind = JObjectKind.String;
            StringValue = str;
        }

        private JObject(bool b) {
            Kind = JObjectKind.Boolean;
            BooleanValue = b;
        }

        private JObject() {
            Kind = JObjectKind.Null;
        }

        private JObject(bool isNegative, bool isFractional, bool negativeExponent, ulong integerPart, ulong fractionalPart, int fractionalPartLength, ulong exponent) {
            Kind = JObjectKind.Number;
            if (!isFractional) {
                MakeInteger(isNegative, integerPart);
            } else {
                MakeFloat(isNegative, negativeExponent, integerPart, fractionalPart, fractionalPartLength, exponent);
            }
        }

        private JObject(List<JObject> list) {
            Kind = JObjectKind.Array;
            ArrayValue = list;
        }

        private JObject(Dictionary<string, JObject> dict) {
            Kind = JObjectKind.Object;
            ObjectValue = dict;
        }

        private void MakeInteger(bool isNegative, ulong integerPart) {
            IsNegative = isNegative;

            if (!IsNegative) {
                ULongValue = integerPart;
                MinInteger = IntegerSize.UInt64;

                if (ULongValue <= Int64.MaxValue) {
                    LongValue = (long)ULongValue;
                    MinInteger = IntegerSize.Int64;
                }

                if (ULongValue <= UInt32.MaxValue) {
                    UIntValue = (uint)ULongValue;
                    MinInteger = IntegerSize.UInt32;
                }

                if (ULongValue <= Int32.MaxValue) {
                    IntValue = (int)ULongValue;
                    MinInteger = IntegerSize.Int32;
                }

                if (ULongValue <= UInt16.MaxValue) {
                    UShortValue = (ushort)ULongValue;
                    MinInteger = IntegerSize.UInt16;
                }

                if (ULongValue <= (ulong)Int16.MaxValue) {
                    ShortValue = (short)ULongValue;
                    MinInteger = IntegerSize.Int16;
                }

                if (ULongValue <= Byte.MaxValue) {
                    ByteValue = (byte)ULongValue;
                    MinInteger = IntegerSize.UInt8;
                }

                if (ULongValue <= (ulong)SByte.MaxValue) {
                    SByteValue = (sbyte)ULongValue;
                    MinInteger = IntegerSize.Int8;
                }

                DoubleValue = ULongValue;
                MinFloat = FloatSize.Double;

                if (DoubleValue <= Single.MaxValue) {
                    FloatValue = (float)DoubleValue;
                    MinFloat = FloatSize.Single;
                }
            } else {
                LongValue = -(long)integerPart;
                MinInteger = IntegerSize.Int64;

                if (LongValue >= Int32.MinValue) {
                    IntValue = (int)LongValue;
                    MinInteger = IntegerSize.Int32;
                }

                if (LongValue >= Int16.MinValue) {
                    ShortValue = (short)LongValue;
                    MinInteger = IntegerSize.Int16;
                }

                if (LongValue >= SByte.MinValue) {
                    SByteValue = (sbyte)LongValue;
                    MinInteger = IntegerSize.Int8;
                }

                DoubleValue = LongValue;
                MinFloat = FloatSize.Double;

                if (DoubleValue >= -Single.MaxValue) {
                    FloatValue = (float)DoubleValue;
                    MinFloat = FloatSize.Single;
                }
            }
        }

        private void MakeFloat(bool isNegative, bool negativeExponent, ulong integerPart, ulong fractionalPart, int fractionalPartLength, ulong exponent) {
            DoubleValue = (isNegative ? -1 : 1) * ((double)integerPart + (double)fractionalPart / Math.Pow(10, fractionalPartLength)) * Math.Pow(10, (negativeExponent ? -1 : 1) * (long)exponent);
            MinFloat = FloatSize.Double;
            IsFractional = true;

            if (DoubleValue < 0) {
                IsNegative = true;

                if (DoubleValue >= -Single.MaxValue) {
                    FloatValue = (float)DoubleValue;
                    MinFloat = FloatSize.Single;
                }
            } else if (DoubleValue <= Single.MaxValue) {
                FloatValue = (float)DoubleValue;
                MinFloat = FloatSize.Single;
            }
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(obj, this)) return true;
            if (!(obj is JObject)) return false;

            var jobj = (JObject)obj;
            if (jobj.Kind != Kind) return false;

            switch (Kind) {
            case JObjectKind.Array:
                if (ArrayValue.Count != jobj.ArrayValue.Count) return false;
                for (var i = 0; i < ArrayValue.Count; ++i) {
                    if (!ArrayValue[i].Equals(jobj.ArrayValue[i])) return false;
                }
                return true;
            case JObjectKind.Boolean:
                return BooleanValue == jobj.BooleanValue;
            case JObjectKind.Number:
                return EqualNumber(this, jobj);
            case JObjectKind.Object:
                if (ObjectValue.Count != jobj.ObjectValue.Count) return false;
                foreach (var pair in ObjectValue) {
                    if (!jobj.ObjectValue.ContainsKey(pair.Key) ||
                        !pair.Value.Equals(jobj.ObjectValue[pair.Key])) return false;
                }
                return true;
            case JObjectKind.String:
                return StringValue == jobj.StringValue;
            }

            return true;
        }

        public override int GetHashCode() {
            switch (Kind) {
            case JObjectKind.Array: return ArrayValue.GetHashCode();
            case JObjectKind.Boolean: return BooleanValue.GetHashCode();
            case JObjectKind.Null: return 0;
            case JObjectKind.Object: return ObjectValue.GetHashCode();
            case JObjectKind.String: return StringValue.GetHashCode();
            case JObjectKind.Number:
                if (IsFractional) return DoubleValue.GetHashCode();
                if (IsNegative) return LongValue.GetHashCode();
                return ULongValue.GetHashCode();
            }
            return 0;
        }

        public static bool EqualNumber(JObject o1, JObject o2) {
            if (o1.MinFloat != o2.MinFloat ||
                o1.MinInteger != o2.MinInteger ||
                o1.IsNegative != o2.IsNegative ||
                o1.IsFractional != o2.IsFractional) return false;

            if (o1.IsFractional) {
                return o1.DoubleValue == o2.DoubleValue;
            }
            if (o1.IsNegative) {
                return o1.LongValue == o2.LongValue;
            }

            return o1.ULongValue == o2.ULongValue;
        }
    }
    
    internal class ParseError : Exception {
        public readonly int Position;

        public ParseError(string message, int position) : base(message) {
            Position = position;
        }
    }

    internal static class JSONDecoder {
        private const char ObjectStart = '{';
        private const char ObjectEnd = '}';
        private const char ObjectPairSeparator = ',';
        private const char ObjectSeparator = ':';
        private const char ArrayStart = '[';
        private const char ArrayEnd = ']';
        private const char ArraySeparator = ',';
        private const char StringStart = '"';
        private const char NullStart = 'n';
        private const char TrueStart = 't';
        private const char FalseStart = 'f';

        public static JObject Decode(string json) {
            var data = Scan(json, 0);
            return data.Result;
        }

        private struct ScannerData {
            public readonly JObject Result;
            public readonly int Index;

            public ScannerData(JObject result, int index) {
                Result = result;
                Index = index;
            }
        }

        private static readonly Dictionary<char, string> EscapeChars =
            new Dictionary<char, string>
                {
                    { '"', "\"" },
                    { '\\', "\\" },
                    { 'b', "\b" },
                    { 'f', "\f" },
                    { 'n', "\n" },
                    { 'r', "\r" },
                    { 't', "\t" },
                };

        private static ScannerData Scan(string json, int index) {
            index = SkipWhitespace(json, index);
            var nextChar = json[index];

            switch (nextChar) {
            case ObjectStart:
                return ScanObject(json, index);
            case ArrayStart:
                return ScanArray(json, index);
            case StringStart:
                return ScanString(json, index + 1);
            case TrueStart:
                return ScanTrue(json, index);
            case FalseStart:
                return ScanFalse(json, index);
            case NullStart:
                return ScanNull(json, index);
            default:
                if (IsNumberStart(nextChar)) {
                    return ScanNumber(json, index);
                }
                throw new ParseError("Unexpected token " + nextChar, index);
            }
        }

        private static ScannerData ScanString(string json, int index) {
            string s;
            index = ScanBareString(json, index, out s);

            return new ScannerData(JObject.CreateString(s), index + 1);
        }

        private static ScannerData ScanTrue(string json, int index) {
            return new ScannerData(JObject.CreateBoolean(true), ExpectConstant(json, index, "true"));
        }

        private static ScannerData ScanFalse(string json, int index) {
            return new ScannerData(JObject.CreateBoolean(false), ExpectConstant(json, index, "false"));
        }

        private static ScannerData ScanNull(string json, int index) {
            return new ScannerData(JObject.CreateNull(), ExpectConstant(json, index, "null"));
        }

        private static ScannerData ScanNumber(string json, int index) {
            var negative = false;
            var fractional = false;
            var negativeExponent = false;

            if (json[index] == '-') {
                negative = true;
                ++index;
            }

            ulong integerPart = 0;
            if (json[index] != '0') {
                while (json.Length > index && char.IsNumber(json[index])) {
                    integerPart = (integerPart * 10) + (ulong)(json[index] - '0');
                    ++index;
                }
            } else {
                ++index;
            }

            ulong fractionalPart = 0;
            int fractionalPartLength = 0;
            if (json.Length > index && json[index] == '.') {
                fractional = true;

                ++index;
                while (json.Length > index && char.IsNumber(json[index])) {
                    fractionalPart = (fractionalPart * 10) + (ulong)(json[index] - '0');
                    ++index;
                    ++fractionalPartLength;
                }
            }

            ulong exponent = 0;
            if (json.Length > index && (json[index] == 'e' || json[index] == 'E')) {
                fractional = true;
                ++index;

                if (json[index] == '-') {
                    negativeExponent = true;
                    ++index;
                } else if (json[index] == '+') {
                    ++index;
                }

                while (json.Length > index && char.IsNumber(json[index])) {
                    exponent = (exponent * 10) + (ulong)(json[index] - '0');
                    ++index;
                }
            }

            return new ScannerData(JObject.CreateNumber(negative, fractional, negativeExponent, integerPart, fractionalPart, fractionalPartLength, exponent), index);
        }

        private static ScannerData ScanArray(string json, int index) {
            var list = new List<JObject>();

            var nextTokenIndex = SkipWhitespace(json, index + 1);
            if (json[nextTokenIndex] == ArrayEnd) return new ScannerData(JObject.CreateArray(list), nextTokenIndex + 1);

            while (json[index] != ArrayEnd) {
                ++index;
                var result = Scan(json, index);
                index = SkipWhitespace(json, result.Index);
                if (json[index] != ArraySeparator && json[index] != ArrayEnd) {
                    throw new ParseError("Expecting array separator (,) or array end (])", index);
                }
                list.Add(result.Result);
            }
            return new ScannerData(JObject.CreateArray(list), index + 1);
        }

        private static ScannerData ScanObject(string json, int index) {
            var dict = new Dictionary<string, JObject>();

            var nextTokenIndex = SkipWhitespace(json, index + 1);
            if (json[nextTokenIndex] == ObjectEnd) return new ScannerData(JObject.CreateObject(dict), nextTokenIndex + 1);

            while (json[index] != ObjectEnd) {
                index = SkipWhitespace(json, index + 1);
                if (json[index] != '"') {
                    throw new ParseError("Object keys must be strings", index);
                }
                string key;
                index = ScanBareString(json, index + 1, out key) + 1;
                index = SkipWhitespace(json, index);
                if (json[index] != ObjectSeparator) {
                    throw new ParseError("Expecting object separator (:)", index);
                }
                ++index;
                var valueResult = Scan(json, index);
                index = SkipWhitespace(json, valueResult.Index);
                if (json[index] != ObjectEnd && json[index] != ObjectPairSeparator) {
                    throw new ParseError("Expecting object pair separator (,) or object end (})", index);
                }
                dict[key] = valueResult.Result;
            }
            return new ScannerData(JObject.CreateObject(dict), index + 1);
        }

        private static int SkipWhitespace(string json, int index) {
            while (char.IsWhiteSpace(json[index])) {
                ++index;
            }
            return index;
        }

        private static int ExpectConstant(string json, int index, string expected) {
            if (json.Substring(index, expected.Length) != expected) {
                throw new ParseError(string.Format("Expected '{0}' got '{1}'",
                                                   expected,
                                                   json.Substring(index, expected.Length)),
                                     index);
            }
            return index + expected.Length;
        }

        private static bool IsNumberStart(char b) {
            return b == '-' || (b >= '0' && b <= '9');
        }

        private static int ScanBareString(string json, int index, out string result) {
            // First determine length
            var lengthIndex = index;
            var foundEscape = false;
            while (json[lengthIndex] != '"') {
                if (json[lengthIndex] == '\\') {
                    foundEscape = true;
                    ++lengthIndex;
                    if (EscapeChars.ContainsKey(json[lengthIndex]) || json[lengthIndex] == 'u') {
                        ++lengthIndex;
                    } else if (json[lengthIndex] == 'u') {
                        lengthIndex += 5;
                    }
                } else {
                    ++lengthIndex;
                }
            }

            if (!foundEscape) {
                result = json.Substring(index, lengthIndex - index);
                return lengthIndex;
            }

            var strBuilder = new StringBuilder(lengthIndex - index);

            var lastIndex = index;
            while (json[index] != '"') {
                if (json[index] == '\\') {
                    if (index > lastIndex) {
                        strBuilder.Append(json, lastIndex, index - lastIndex);
                    }
                    ++index;
                    if (EscapeChars.ContainsKey(json[index])) {
                        strBuilder.Append(EscapeChars[json[index]]);
                        ++index;
                    } else if (json[index] == 'u') {
                        ++index;
                        var unicodeSequence = Convert.ToInt32(json.Substring(index, 4), 16);
                        strBuilder.Append((char)unicodeSequence);
                        index += 4;
                    }
                    lastIndex = index;
                } else {
                    ++index;
                }
            }

            if (lastIndex != index) {
                strBuilder.Append(json, lastIndex, index - lastIndex);
            }

            result = strBuilder.ToString();
            return index;
        }
    }
    
    internal class JSONEncoder {
        public static string Encode(object obj) {
            var encoder = new JSONEncoder();
            encoder.EncodeObject(obj);
            return encoder._buffer.ToString();
        }

        private readonly StringBuilder _buffer = new StringBuilder();

        internal static readonly Dictionary<char, string> EscapeChars =
            new Dictionary<char, string>
                {
                    { '"', "\\\"" },
                    { '\\', "\\\\" },
                    { '\b', "\\b" },
                    { '\f', "\\f" },
                    { '\n', "\\n" },
                    { '\r', "\\r" },
                    { '\t', "\\t" },
                    { '\u2028', "\\u2028" },
                    { '\u2029', "\\u2029" }
                };

        private JSONEncoder() { }

        private void EncodeObject(object obj) {
            if (obj == null) {
                EncodeNull();
            } else if (obj is string) {
                EncodeString((string)obj);
            } else if (obj is float) {
                EncodeFloat((float)obj);
            } else if (obj is double) {
                EncodeDouble((double)obj);
            } else if (obj is int) {
                EncodeLong((int)obj);
            } else if (obj is uint) {
                EncodeULong((uint)obj);
            } else if (obj is long) {
                EncodeLong((long)obj);
            } else if (obj is ulong) {
                EncodeULong((ulong)obj);
            } else if (obj is short) {
                EncodeLong((short)obj);
            } else if (obj is ushort) {
                EncodeULong((ushort)obj);
            } else if (obj is byte) {
                EncodeULong((byte)obj);
            } else if (obj is bool) {
                EncodeBool((bool)obj);
            } else if (obj is IDictionary) {
                EncodeDictionary((IDictionary)obj);
            } else if (obj is IDictionary<string, object>) {
                EncodeDictionary((IDictionary<string, object>)obj);
            } else if (obj is IEnumerable) {
                EncodeEnumerable((IEnumerable)obj);
            } else if (obj is Enum) {
                EncodeObject(Convert.ChangeType(obj, Enum.GetUnderlyingType(obj.GetType())));
            } else if (obj is JObject) {
                var jobj = (JObject)obj;
                switch (jobj.Kind) {
                case JObjectKind.Array:
                    EncodeEnumerable(jobj.ArrayValue);
                    break;
                case JObjectKind.Boolean:
                    EncodeBool(jobj.BooleanValue);
                    break;
                case JObjectKind.Null:
                    EncodeNull();
                    break;
                case JObjectKind.Number:
                    if (jobj.IsFractional) {
                        EncodeDouble(jobj.DoubleValue);
                    } else if (jobj.IsNegative) {
                        EncodeLong(jobj.LongValue);
                    } else {
                        EncodeULong(jobj.ULongValue);
                    }
                    break;
                case JObjectKind.Object:
                    EncodeDictionary(jobj.ObjectValue);
                    break;
                case JObjectKind.String:
                    EncodeString(jobj.StringValue);
                    break;
                default:
                    EncodeString(obj.ToString());
                    break;
                }
            } else {
                EncodeString(obj.ToString());
            }
        }

        private void EncodeNull() {
            _buffer.Append("null");
        }

        private void EncodeString(string str) {
            _buffer.Append('"');
            foreach (var c in str) {
                if (EscapeChars.ContainsKey(c)) {
                    _buffer.Append(EscapeChars[c]);
                } else {
                    if (c > 0x80 || c < 0x20) {
                        _buffer.Append("\\u" + Convert.ToString(c, 16)
                                                   .ToUpper(CultureInfo.InvariantCulture)
                                                   .PadLeft(4, '0'));
                    } else {
                        _buffer.Append(c);
                    }
                }
            }
            _buffer.Append('"');
        }

        private void EncodeFloat(float f) {
            _buffer.Append(f.ToString(CultureInfo.InvariantCulture));
        }

        private void EncodeDouble(double d) {
            _buffer.Append(d.ToString(CultureInfo.InvariantCulture));
        }

        private void EncodeLong(long l) {
            _buffer.Append(l.ToString(CultureInfo.InvariantCulture));
        }

        private void EncodeULong(ulong l) {
            _buffer.Append(l.ToString(CultureInfo.InvariantCulture));
        }

        private void EncodeBool(bool b) {
            _buffer.Append(b ? "true" : "false");
        }

        private void EncodeDictionary(IDictionary<string, object> d) {
            var isFirst = true;
            _buffer.Append('{');
            foreach (KeyValuePair<string, object> pair in d) {
                if (pair.Value == null) continue;
                if (!isFirst) _buffer.Append(',');
                EncodeString(pair.Key);
                _buffer.Append(':');
                EncodeObject(pair.Value);
                isFirst = false;
            }
            _buffer.Append('}');
        }
        
        private void EncodeDictionary(IDictionary d) {
            var isFirst = true;
            _buffer.Append('{');
            foreach (DictionaryEntry pair in d) {
                if (pair.Value == null) continue;
                if (!isFirst) _buffer.Append(',');
                EncodeString(pair.Key.ToString());
                _buffer.Append(':');
                EncodeObject(pair.Value);
                isFirst = false;
            }
            _buffer.Append('}');
        }

        private void EncodeEnumerable(IEnumerable e) {
            var isFirst = true;
            _buffer.Append('[');
            foreach (var obj in e) {
                if (!isFirst) _buffer.Append(',');
                EncodeObject(obj);
                isFirst = false;
            }
            _buffer.Append(']');
        }
    }
}
