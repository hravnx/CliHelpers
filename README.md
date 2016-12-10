# CliHelpers
[![Build status](https://ci.appveyor.com/api/projects/status/cudqqch6sf5b96pr/branch/master?svg=true)](https://ci.appveyor.com/project/hravnx/clihelpers/branch/master)

## What is it?
CliHelpers is a thin helper layer to make [Microsoft.Extensions.CommandLineUtils](https://www.nuget.org/packages/Microsoft.Extensions.CommandLineUtils/)
easier to work with. 
It adds fluent functions to enable strongly-typing, transformaing and validating command-line arguments in a nice, declarative syntax.

## Examples

```csharp
using Microsoft.Extensions.CommandLineUtils;
using CliHelpers;

public class Program {
    public static void Main(string[] args) {
        var app = new CommandLineApplication(); // <-- from Microsoft.Extensions.CommandLineUtils
        var myIntOption = app
            // first add an untyped option
            .AddCliOption("-n | --number <number>", "Description")
            // next ensure it is present and has a type
            .IsRequired<int>() 
            // next apply any tranformations and validations that you need
            .IsNotNegative() // <-- validation built into CliHelpers
            .ValidateWith(n => n < 10)  // <-- custom validation, defined inline
            .TransformWith(n => n * 10) // <-- custom transform, defined inline
            ;
            
        // this handler will be called when parsing the command line
        app.OnExecute(() => {
            // n is of type int and will be 10 times the value passed on the command line
            var n = myIntOption.Value(); // <-- will throw if invalid args was passed on command line
            // ... use n ...
            return 0;
        });

        try {
            // parse and call OnExecute handler specified above
            app.Execute(args); 
        } catch (CommandParsingException ex) {
            // handle parsing errors ...
        }
    }
}
```
See test folder for further uses. Check [CLiExtension.cs](https://github.com/hravnx/CliHelpers/blob/master/src/CliHelpers/CliExtensions.cs) for
the available built-in transformations and validations.

## Installation
The package should be installed as a [NuGet](https://www.nuget.org/packages/CliHelpers/) package.








