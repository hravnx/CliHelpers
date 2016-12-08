using System;
using System.IO;
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
    }


    public static class CliTransformExtensions
    {
        public static ICliOptionWithValue<U> TransformWith<T, U>(this ICliOptionWithValue<T> option, Func<T, U> transformer)
        {
            return new CliTransformingOption<T, U>(option, transformer);
        }

        public static ICliOptionWithValue<string> AsAbsolutePath(this ICliOptionWithValue<string> option)
            => option.TransformWith(s => Path.IsPathRooted(s) ? s : Path.Combine(Directory.GetCurrentDirectory(),s));
    }


    public static class CliValidationExtensions
    {
        public static ICliOptionWithValue<T> ValidatedWith<T>(this ICliOptionWithValue<T> option, Func<T, bool> validator, string errorTemplate)
            => new CliValidatingOption<T>(option, validator, errorTemplate);

        public static ICliOptionWithValue<sbyte> IsNotNegative(this ICliOptionWithValue<sbyte> option)
            => option.ValidatedWith(n => n >= 0, "'{0}' must be >= 0, was '{1}'");
        public static ICliOptionWithValue<short> IsNotNegative(this ICliOptionWithValue<short> option)
            => option.ValidatedWith(n => n >= 0, "'{0}' must be >= 0, was '{1}'");
        public static ICliOptionWithValue<int> IsNotNegative(this ICliOptionWithValue<int> option)
            => option.ValidatedWith(n => n >= 0, "'{0}' must be >= 0, was '{1}'");
        public static ICliOptionWithValue<long> IsNotNegative(this ICliOptionWithValue<long> option)
            => option.ValidatedWith(n => n >= 0, "'{0}' must be >= 0, was '{1}'");
        public static ICliOptionWithValue<float> IsNotNegative(this ICliOptionWithValue<float> option)
            => option.ValidatedWith(n => n >= 0, "'{0}' must be >= 0, was '{1}'");
        public static ICliOptionWithValue<double> IsNotNegative(this ICliOptionWithValue<double> option)
            => option.ValidatedWith(n => n >= 0, "'{0}' must be >= 0, was '{1}'");

        public static ICliOptionWithValue<string> FileMustExist(this ICliOptionWithValue<string> option)
            => option.ValidatedWith(File.Exists, "{0} '{1}' does not exist");
    }


}
