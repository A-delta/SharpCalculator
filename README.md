# SharpCalculatorLib
SharpCalculatorLib is a variable/function calculator written in C#.
Each expression is cleaned, converted to postfix and evaluated.
The cleaning method is used to parse the expression, to handle things such as implicit products and to search for input error.
The cleaned expression is converted to [postfix notation](https://en.wikipedia.org/wiki/Reverse_Polish_notation) wich is easier to evaluate. (will maybe be transformed into a [AST](https://en.wikipedia.org/wiki/Abstract_syntax_tree) in the future)



# SharpCalculator
The SharpCalculatorApp is the console application written to interact with the library. There will be a graphical application in the future.

# TO DO
## Lib
- real error reporter
- unit tests
- more math functions
- better help function (shows docstrings) or -> console app

## Console App
- real console app with commands (function suggestion when typing for example)

## Graphical App
- finish above things
