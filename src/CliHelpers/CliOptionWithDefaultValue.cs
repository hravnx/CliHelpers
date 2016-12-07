using System;
using System.Diagnostics;

namespace CliHelpers
{
    public sealed class CliOptionWithDefaultValue<T> : CliOptionWithValueBase<T>
    {
        CliOption m_Option;
        T m_DefaultValue;
        Func<string, T> m_Converter;

        internal CliOptionWithDefaultValue(CliOption option, T defaultValue, Func<string, T> converter)
            : base(option)
        {
            Debug.Assert(defaultValue != null);
            Debug.Assert(converter != null);

            m_Option = option;
            m_DefaultValue = defaultValue;
            m_Converter = converter;
        }

        public override T Value()
        {
            var stringValue = m_Option.Option.Value();
            if (stringValue == null)
            {
                return m_DefaultValue;
            }

            return m_Converter(stringValue);
        }
    }
}
