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

    /// <summary>
    /// Specifies the oparations available on a CLI option that has a value
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public interface ICliOptionWithValue<out T> : ICliOption
    {
        /// <summary>
        /// Gets the value of the option
        /// </summary>
        /// <returns>The value</returns>
        /// <exception cref="CommandParsingException">If there is a validation error while parsing the option on the command line</exception>
        T Value();
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
