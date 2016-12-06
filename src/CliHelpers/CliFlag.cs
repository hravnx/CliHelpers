using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
