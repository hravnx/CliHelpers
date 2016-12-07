using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CliHelpers.Tests
{

    public class CliOptionTests
    {
        [Fact]
        public void CliOption_can_be_created_via_AddCliOption()
        {
            var app = new CommandLineApplication();
            var option = app.AddCliOption("-d | --debug <debugger>", "My description");

            Assert.Equal(app, option.Command);
            Assert.Equal(option.OptionName, "debugger");
        }
        [Fact]
        public void CliOption_throws_when()
        {
            var app = new CommandLineApplication();
            var option = app.AddCliOption("-d | --debug <debugger>", "My description");

            Assert.Equal(app, option.Command);
            Assert.Equal(option.OptionName, "debugger");
        }
    }
    public class CliFlagTests
    {
        [Fact]
        public void OptionIsFine()
        {
            var app = new CommandLineApplication();
            var option = app.AddCliFlag("-d | --debug <debugger>", "My description");

            Assert.False(option.IsSet());
        }
    }
}
