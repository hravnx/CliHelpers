using System.Diagnostics;
using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{

    /// <summary>
    /// Specifies the properties available to a CLI option
    /// </summary>
    public interface ICliOption
    {
        /// <summary>
        /// Gets the underlying command for this option
        /// </summary>
        CommandLineApplication Command { get; }

        /// <summary>
        /// Gets the name of this option
        /// </summary>
        string OptionName { get; }
    }


    public sealed class CliOption : ICliOption
    {
        public CommandLineApplication Command { get; }

        public string OptionName { get; }

        internal CommandOption Option { get; }

        internal CliOption(CommandLineApplication command, CommandOption option)
        {
            Debug.Assert(command != null);
            Debug.Assert(option != null);
            Debug.Assert(option.OptionType == CommandOptionType.SingleValue);

            Command = command;
            Option = option;
            OptionName = option.ValueName ?? option.LongName ?? option.ShortName;
        }
    }

}
