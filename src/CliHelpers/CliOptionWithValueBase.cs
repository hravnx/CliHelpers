using System.Diagnostics;
using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{
    public abstract class CliOptionWithValueBase<T> : ICliOptionWithValue<T>
    {
        public CommandLineApplication Command { get; }

        public string OptionName { get; }

        protected CliOptionWithValueBase(ICliOption cliOption)
        {
            Debug.Assert(cliOption != null);

            Command = cliOption.Command;
            OptionName = cliOption.OptionName;
        }
        public abstract T Value();
    }

}
