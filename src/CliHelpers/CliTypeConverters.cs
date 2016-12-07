using System;
using System.Collections.Generic;
using System.Globalization;

namespace CliHelpers
{
    static class CliTypeConverters
    {
        static readonly Dictionary<Type, Func<string, object>> k_Converters = new Dictionary<Type, Func<string, object>>();

        static CliTypeConverters()
        {
            var provider = CultureInfo.InvariantCulture;
            k_Converters.Add(typeof(sbyte), s => ((IConvertible)s).ToSByte(provider));
            k_Converters.Add(typeof(byte), s => ((IConvertible)s).ToByte(provider));
            k_Converters.Add(typeof(short), s => ((IConvertible)s).ToInt16(provider));
            k_Converters.Add(typeof(ushort), s => ((IConvertible)s).ToUInt16(provider));
            k_Converters.Add(typeof(int), s => ((IConvertible)s).ToInt32(provider));
            k_Converters.Add(typeof(uint), s => ((IConvertible)s).ToUInt32(provider));
            k_Converters.Add(typeof(long), s => ((IConvertible)s).ToInt64(provider));
            k_Converters.Add(typeof(ulong), s => ((IConvertible)s).ToUInt64(provider));
            k_Converters.Add(typeof(float), s => ((IConvertible)s).ToSingle(provider));
            k_Converters.Add(typeof(double), s => ((IConvertible)s).ToDouble(provider));
            k_Converters.Add(typeof(char), s => ((IConvertible)s).ToChar(provider));
            k_Converters.Add(typeof(bool), s => ((IConvertible)s).ToBoolean(provider));
            k_Converters.Add(typeof(decimal), s => ((IConvertible)s).ToDecimal(provider));
            k_Converters.Add(typeof(DateTime), s => ((IConvertible)s).ToDateTime(provider));
            k_Converters.Add(typeof(string), s => s);
        }

        public static Func<string, object> GetConverter(Type t)
        {
            Func<string, object> result;
            if (!k_Converters.TryGetValue(t, out result))
            {
                throw new InvalidCastException($"Can not get converter from CLI string to type '{t.Name}'");
            }
            return result;
        }


    }
}
