using System;
using Microsoft.Extensions.CommandLineUtils;

namespace CliHelpers
{
    public static class CliCreationExtensions
    {
        public static CliFlag AddCliFlag(this CommandLineApplication command, string template, string description)
        {
            var option = command.Option(template, description, CommandOptionType.NoValue);
            if (!string.IsNullOrEmpty(option.ValueName))
            {
                throw new ArgumentException($"Flags can't have values (template is '{template}')", nameof(template));
            }
            return new CliFlag(option);
        }

        public static CliOption AddCliOption(this CommandLineApplication command, string template, string description)
        {
            var option = command.Option(template, description, CommandOptionType.SingleValue);
            return new CliOption(command, option);
        }
    }



    public static class CliValueExtensions
    {
        //public static IOptionWithValue<U> TransformWith<T, U>(this IOptionWithValue<T> option, Func<T, U> transformer)
        //{
        //    return new CliTransformingOption<T, U>(option, transformer);
        //}


        public static ICliOptionWithValue<T> IsRequired<T>(this CliOption option)
        {
            var converter = CliTypeConverters.GetConverter(typeof(T));
            return new CliRequiredOptionWithValue<T>(option, s => (T)converter(s));
        }

        public static ICliOptionWithValue<T> WithDefaultValue<T>(this CliOption option, T defaultValue)
        {
            var converter = CliTypeConverters.GetConverter(typeof(T));
            return new CliOptionWithDefaultValue<T>(option, defaultValue, s => (T)converter(s));
        }

//        public static IOptionWithValue<string> AsAbsolutePath(this IOptionWithValue<string> option) => option.TransformWith(s => s + "hmm");

    }
}
