using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CliHelpers.Tests
{


    public class CliOptionTests
    {
        CommandLineApplication _app = new CommandLineApplication();

        void TestExecute(CommandLineApplication app, Action onExecute, params string[] args)
        {
            app.OnExecute(() =>
            {
                onExecute();
                return 0;
            });

            app.Execute(args);
        }

        [Fact]
        public void CliOption_can_be_created_via_AddCliOption()
        {
            var option = _app.AddCliOption("-d | --debug <debugger>", "My description");

            Assert.Equal(_app, option.Command);
            Assert.Equal(option.OptionName, "debugger");
        }

        [Fact]
        public void OptionName_can_be_read_from_value_name()
        {
            var actualName = _app.AddCliOption("-a <value>", null).OptionName;
            Assert.Equal("value", actualName);
        }

        [Fact]
        public void OptionName_can_be_read_from_long_name()
        {
            var actualName = _app.AddCliOption("-a | --a-long-name", null)
                .WithDefaultValue(10)
                .OptionName;
            Assert.Equal("a-long-name", actualName);
        }

        [Fact]
        public void OptionName_can_be_read_from_short_name()
        {
            var actualName = _app.AddCliOption("-a", null).OptionName;
            Assert.Equal("a", actualName);
        }

        [Fact]
        public void AddCliOption_throws_when_given_null_or_empty_templates()
        {
            Assert.Throws<NullReferenceException>(() => _app.AddCliOption(null, "My description"));
            Assert.Throws<ArgumentException>("template", () => _app.AddCliOption("", "My description"));
        }

        void TestDefaultValue<T>(T defValue)
        {
            var required = _app
                .AddCliOption("-a <value>", null)
                .WithDefaultValue(defValue);
            Assert.Equal(defValue, required.Value());
        }

        [Fact]
        public void CliOption_can_be_converted_to_typed_option_with_default_value()
        {
            TestDefaultValue<sbyte>(-123);
            TestDefaultValue<byte>(123);
            TestDefaultValue<short>(-135);
            TestDefaultValue<ushort>(135);
            TestDefaultValue<int>(-1023);
            TestDefaultValue<uint>(1023u);
            TestDefaultValue<long>(-567890L);
            TestDefaultValue<ulong>(567890ul);
            TestDefaultValue<float>(-1.2f);
            TestDefaultValue<double>(0.0002);
            TestDefaultValue<decimal>(123.656565665m);
            TestDefaultValue<DateTime>(DateTime.Now);
            TestDefaultValue("hello");
        }

        [Fact]
        public void WithDefaultValue_throws_if_unsupported_type_is_used()
        {
            var option = _app.AddCliOption("-a", null);
            Assert.Throws<InvalidCastException>(() =>
                    option.WithDefaultValue(DateTimeOffset.MinValue));
        }

        [Fact]
        public void CliOption_can_override_default_value_from_command_line()
        {
            var aOpt = _app
                .AddCliOption("-a <value>", null)
                .WithDefaultValue("hello");

            _app.OnExecute(() =>
            {
                Assert.Equal("not-hello", aOpt.Value());
                return 0;
            });

            _app.Execute("-a", "not-hello");
        }

        void TestRequiredValue<T>(string arg, T expected)
        {
            var app = new CommandLineApplication();
            var opt = app.AddCliOption("-a", null)
                .IsRequired<T>();
            TestExecute(app, () => Assert.Equal(expected, opt.Value()), "-a", arg);
        }

        [Fact]
        public void CliOption_can_be_converted_to_typed_required_value()
        {
            TestRequiredValue<sbyte>("-16", -16);
            TestRequiredValue<byte>("16", 16);
            TestRequiredValue<short>("-1446", -1446);
            TestRequiredValue<ushort>("1446", 1446);
            TestRequiredValue<int>("-21446", -21446);
            TestRequiredValue<uint>("21446", 21446);
            TestRequiredValue<long>("-2144564545646", -2144564545646);
            TestRequiredValue<ulong>("2144564545646", 2144564545646UL);
            TestRequiredValue<float>("-1.2", -1.2f);
            TestRequiredValue<double>("0.12333", 0.12333);
            TestRequiredValue<char>("b", 'b');
            TestRequiredValue<bool>("true", true);
            TestRequiredValue<decimal>("123.5667", 123.5667m);
            TestRequiredValue<DateTime>("2016-10-21 10:32:01", new DateTime(2016,10,21,10,32,1,DateTimeKind.Utc));
            TestRequiredValue<string>("hello", "hello");
        }

        [Fact]
        public void Required_CliOption_will_throw_if_no_value_was_given()
        {
            var opt = _app.AddCliOption("-a", null)
                .IsRequired<string>();

            TestExecute(_app, () => Assert.Throws<CommandParsingException>(() => opt.Value()));
            Assert.Throws<CommandParsingException>(() => TestExecute(_app, () => { }, "-a"));
        }

        [Fact]
        public void CliOption_can_be_transformed()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .WithDefaultValue("abc")
                .TransformWith(v => v + "def");

            Assert.Equal("abcdef", option.Value());
        }

        [Fact]
        public void CliOption_can_be_validated()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .WithDefaultValue("abc")
                .ValidatedWith(_ => false, "");
            Assert.Throws<CommandParsingException>(() => option.Value());
        }

        [Fact]
        public void IsNotNegative_works_for_sbytes()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .IsRequired<sbyte>()
                .IsNotNegative();

            var option2 = _app
                .AddCliOption("-b <value>", null)
                .IsRequired<sbyte>()
                .IsNotNegative();

            _app.OnExecute(() =>
            {
                Assert.Throws<CommandParsingException>(() => option.Value());
                Assert.Equal(21, option2.Value());
                return 0;
            });

            _app.Execute("-a", "-21", "-b", "21");

        }

        [Fact]
        public void IsNotNegative_works_for_shorts()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .IsRequired<short>()
                .IsNotNegative();

            var option2 = _app
                .AddCliOption("-b <value>", null)
                .IsRequired<short>()
                .IsNotNegative();

            _app.OnExecute(() =>
            {
                Assert.Throws<CommandParsingException>(() => option.Value());
                Assert.Equal(2121, option2.Value());
                return 0;
            });

            _app.Execute("-a", "-2121", "-b", "2121");

        }

        [Fact]
        public void IsNotNegative_works_for_ints()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .IsRequired<int>()
                .IsNotNegative();

            var option2 = _app
                .AddCliOption("-b <value>", null)
                .IsRequired<int>()
                .IsNotNegative();

            _app.OnExecute(() =>
            {
                Assert.Throws<CommandParsingException>(() => option.Value());
                Assert.Equal(21210, option2.Value());
                return 0;
            });

            _app.Execute("-a", "-21210", "-b", "21210");

        }

        [Fact]
        public void IsNotNegative_works_for_longs()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .IsRequired<long>()
                .IsNotNegative();

            var option2 = _app
                .AddCliOption("-b <value>", null)
                .IsRequired<long>()
                .IsNotNegative();

            _app.OnExecute(() =>
            {
                Assert.Throws<CommandParsingException>(() => option.Value());
                Assert.Equal(21210L, option2.Value());
                return 0;
            });

            _app.Execute("-a", "-21210", "-b", "21210");

        }

        [Fact]
        public void IsNotNegative_works_for_floats()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .IsRequired<float>()
                .IsNotNegative();

            var option2 = _app
                .AddCliOption("-b <value>", null)
                .IsRequired<float>()
                .IsNotNegative();

            _app.OnExecute(() =>
            {
                Assert.Throws<CommandParsingException>(() => option.Value());
                Assert.Equal(212.10f, option2.Value());
                return 0;
            });

            _app.Execute("-a", "-212.10", "-b", "212.10");

        }

        [Fact]
        public void IsNotNegative_works_for_doubles()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .IsRequired<double>()
                .IsNotNegative();

            var option2 = _app
                .AddCliOption("-b <value>", null)
                .IsRequired<double>()
                .IsNotNegative();

            _app.OnExecute(() =>
            {
                Assert.Throws<CommandParsingException>(() => option.Value());
                Assert.Equal(212.10, option2.Value());
                return 0;
            });

            _app.Execute("-a", "-212.10", "-b", "212.10");

        }

        [Fact]
        public void CliOption_can_be_transformed_to_abs_path()
        {
            var option = _app
                .AddCliOption("-a <value>", null)
                .WithDefaultValue("abc.txt")
                .AsAbsolutePath();

            var option2 = _app
                .AddCliOption("-b <value2>", null)
                .WithDefaultValue(Path.Combine(Directory.GetCurrentDirectory(), "abc.txt"))
                .AsAbsolutePath();

            Assert.True(Path.IsPathRooted(option.Value()));
            Assert.True(Path.IsPathRooted(option2.Value()));
        }

        [Fact]
        public void CliOption_can_validate_that_it_is_an_existing_file()
        {
            using (var tmp = new TempFileMaker())
            {
                var option = _app
                    .AddCliOption("-a <value>", null)
                    .WithDefaultValue(tmp.TempFilePath)
                    .FileMustExist();

                var option2 = _app
                    .AddCliOption("-b <value2>", null)
                    .WithDefaultValue(tmp.TempFilePath + ".1")
                    .FileMustExist();

                Assert.Equal(tmp.TempFilePath, option.Value());
                Assert.Throws<CommandParsingException>(() => option2.Value());
            }

        }


    }

}
