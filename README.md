# SharpCalculatorLib
SharpCalculatorLib is a variable/function calculator written in C#.
Each expression is cleaned, converted to postfix and evaluated.
The cleaning method is used to parse the expression, to handle things such as implicit products and to search for input error.
The cleaned expression is converted to [postfix notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation) wich is easier to evaluate. (will maybe be transformed into a [AST](https://en.wikipedia.org/wiki/Abstract_syntax_tree) in the future)



# SharpCalculator
The SharpCalculatorApp is the console application written to interact with the library. There will be a graphical application in the future.

# Build instructions

SharpCalculator is compiled with the following arguments `dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -r <RID>`
(Common [RID](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) are listed here.)

# Contribute
Feel free to contribute by submitting new Math Functions for the SharpCalculatorLib, it can be anything, for example : `PGCD()`, `IsPrime()`, `Cos()` are missing.
Note that your function must follow the IFunction interface.


# TO DO
## Lib
- more error catchers
- more unit tests
- more math functions

## Graphical App
- finish above things
