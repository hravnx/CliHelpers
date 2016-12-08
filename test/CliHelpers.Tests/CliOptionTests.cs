using System;
using System.Diagnostics.CodeAnalysis;
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

    }

}
