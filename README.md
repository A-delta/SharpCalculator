# DièseCalcCLI
The SharpCalculatorApp project is a console application written to interact with the DièseCalculatorLib library. There will be a graphical application in the future (other repo) and a complete documentation.

# DièseCalculatorLib
The DièseCalculatorLib library is a math expression cleaner, parser and evaluator supporting functions, variables and basic operations (with implicit products support)
You can download the NuGet package [here](https://www.nuget.org/packages/SharpCalculatorLib/) or check the repository [here](https://github.com/DieseCalc/DieseCalcLib)

# In the future
When [.NET MAUI](https://devblogs.microsoft.com/dotnet/introducing-net-multi-platform-app-ui/) will be released, I will make a Desktop/Mobile app to interact with the Library.

# Build instructions
DièseCalcCLI releases are compiled with the following arguments `dotnet publish -c Release -p:PublishSingleFile=true -p:PublishTrimmed=true --self-contained true -r <RID>`
(Common [RID](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog) are listed here.)
