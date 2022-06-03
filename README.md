# Osu-Difficulty-Statistics
-----
This program aims to serve as a utility to assist new and seasoned osu! ruleset creators curate and develop the performance and difficulty calculators of their rulesets with the ability to test them on the actual osu! game.

### Features
Alongside being able to test your calculators on actual osu! maps, we also provide;
-   Hot reloading,
-   Strain graph view for each skills,
-   Extended attributes view,
-   Hit objects attributes view,
-   ...with the possibility of additional features in the future.

## Integration
This project was created with the intent of not interfering with the original base to your ruleset, and in order to properly use it; you will need to create an extended variant of it built on top of it with the [`IExtendedRuleset`](https://github.com/taulazer/Osu-Difficulty-Statistics/blob/master/osu.Rulesets.Difficulty.Statistics/Rulesets/IExtendedRuleset.cs) interface.

We have an example of an "extended" ruleset provided for tau! [here](https://github.com/taulazer/Extended-Tau).

## Building
This program can be built for multiple platforms, primarily depending on IDE, and compilation method. Most IDE's will provide an option to build the instance you have of this program .

### Requirements
* A platform with the [.NET 6.0](https://dotnet.microsoft.com/download/dotnet-core), or above installed.
* An IDE, such as [JetBrains Rider](https://www.jetbrains.com/rider/), or [Visual Studio 2019](https://visualstudio.microsoft.com/) and above, that include a sustainable syntax highlighter and intellisense for easier workability with the codebase of this program.

### Command Line
You can also build and run from the command-line with a single command:

```sh
dotnet run --project osu.Rulesets.Difficulty.Statistics.Desktop
```

If you are not interested in debugging, you can add  `-c Release`  to gain performance. In this case, you must replace  `Debug`  with  `Release`  in any commands mentioned in this document.

If the build fails, try to restore NuGet packages with  `dotnet restore`.

## Contributions

All contributions are appreciated, as to improve the program on its functionality. As this program isn't perfect, we would enjoy all additions to the code through bugfixing and ideas. Contributions should be done over an issue or a pull request, to give maintainers a chance to review changes to the codebase.

For new ideas and features, we would prefer for you to write an issue before trying to add it to show the maintainers.

## License

Osu-Difficulty-Statistics is licenced under the  [MIT](https://opensource.org/licenses/MIT)  License. For licensing information, refer to the  [license file](https://github.com/taulazer/Osu-Difficulty-Statistics/blob/master/LICENSE)  regarding what is permitted regarding the codebase of this program.

The licensing here does not directly apply to  [osu!](https://github.com/ppy/osu), as it is bound to its own licensing. What is reflected in our licensing  _may_  not be allowed in the  [osu!](https://github.com/ppy/osu)  github repository.
