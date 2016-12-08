using System;
using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{
    public sealed class CliValidatingOption<T> : CliOptionWithValueBase<T>
    {
        ICliOptionWithValue<T> m_Prev;
        Func<T, bool> m_Validator;
        string m_ErrorTemplate;
        internal CliValidatingOption(ICliOptionWithValue<T> prev, Func<T, bool> validator, string errorTemplate)
            : base(prev)
        {
            m_Prev = prev;
            m_Validator = validator;
            m_ErrorTemplate = errorTemplate;
        }
        public override T Value()
        {
            var v = m_Prev.Value();

            if (!m_Validator(v))
            {
                throw new CommandParsingException(Command, string.Format(m_ErrorTemplate, OptionName, v));
            }
            return v;
        }
    }
}
