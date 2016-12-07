using System;
using Microsoft.Extensions.CommandLineUtils;
using Xunit;

namespace CliHelpers.Tests
{
    public class CliFlagTests
    {
        [Fact]
        public void CliFlag_can_be_created_via_AddCliFlag()
        {
            var app = new CommandLineApplication();
            var option = app.AddCliFlag("-d | --debug", "My description");
            Assert.False(option.IsSet());
        }

        [Fact]
        public void AddCliFlag_throws_when_given_null_or_empty_templates()
        {
            var app = new CommandLineApplication();
            Assert.Throws<NullReferenceException>(() => app.AddCliOption(null, "My description"));
            Assert.Throws<ArgumentException>("template", () => app.AddCliOption("", "My description"));
        }

        [Fact]
        public void AddCliFlag_throws_when_given_a_value_name_in_template()
        {
            var app = new CommandLineApplication();
            Assert.Throws<ArgumentException>("template", () => app.AddCliFlag("-d | --debug <stuff>", "My description"));
        }

    }
}
