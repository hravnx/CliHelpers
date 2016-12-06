using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CliHelpers.Tests
{

    public class CliOptionTests
    {
        [Fact]
        public void OptionIsFine()
        {
            var app = new CommandLineApplication();
            var option = app.AddCliOption("-d | --debug <debugger>", "My description");

            Assert.Equal(app, option.Command);
            Assert.Equal(option.OptionName, "debugger");
        }
    }
}
