using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{
    public static class CliCreationExtensions
    {
        public static CliFlag AddCliFlag(this CommandLineApplication command, string template, string description)
        {
            var option = command.Option(template, description, CommandOptionType.NoValue);
            return new CliFlag(option);
        }

        public static CliOption AddCliOption(this CommandLineApplication command, string template, string description)
        {
            var option = command.Option(template, description, CommandOptionType.SingleValue);
            return new CliOption(command, option);
        }
    }
}
