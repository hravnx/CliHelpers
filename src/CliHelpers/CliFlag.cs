using System.Diagnostics;
using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{
    public sealed class CliFlag
    {
        CommandOption m_Option;

        internal CliFlag(CommandOption option)
        {
            Debug.Assert(option != null);
            Debug.Assert(option.OptionType == CommandOptionType.NoValue);
            m_Option = option;
        }

        public bool IsSet()
        {
            return m_Option.HasValue();
        }
    }
}
