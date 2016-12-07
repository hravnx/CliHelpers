using System;
using System.Diagnostics;
using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{
    public sealed class CliRequiredOptionWithValue<T> : CliOptionWithValueBase<T>
    {
        CliOption m_Option;
        Func<string, T> m_Converter;

        internal CliRequiredOptionWithValue(CliOption option, Func<string, T> converter)
            : base(option)
        {
            Debug.Assert(converter != null);
            m_Option = option;
            m_Converter = converter;
        }

        public override T Value()
        {
            var stringValue = m_Option.Option.Value();
            if (stringValue == null)
            {
                throw new CommandParsingException(Command, $"'{OptionName}' is required");
            }

            return m_Converter(stringValue);
        }
    }
}
