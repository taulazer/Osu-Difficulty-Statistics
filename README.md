# Osu-Difficulty-Statistics

## What is this?

This project serves as a tool to help new ruleset creators develop their own difficulty and or performance calculations without having the assumption as to if values are correct or not.

Some tools we provide are:
- Hot reloading.
- Strain graph view for each skills.
- Extended attributes view.
- Hit objects attributes view.
- ...Possibly more in the future.

## Integrating your own ruleset

This project was designed with not interfering with your original codebase, but building on top of it. Inheriting [`IExtendedRuleset`](https://github.com/taulazer/Osu-Difficulty-Statistics/blob/master/osu.Rulesets.Difficulty.Statistics/Rulesets/IExtendedRuleset.cs) will give you everything you need to start building on top of your custom ruleset.

You can see an example of an "extended" ruleset via [Tau's integration](https://github.com/taulazer/Extended-Tau).

## Development
When developing or debugging the codebase, a few prerequisites are required as following:
* An IDE that supports the C# language in automatic completion, and syntax highlighting; examples of such being [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) and above, or [JetBrains Rider](https://www.jetbrains.com/rider/).
* The [osu!framework](https://github.com/ppy/osu-framework/tree/master/osu.Framework), and [osu!](https://github.com/ppy/osu) codebases are added as dependencies for building

### Source Code
You are able to clone the repository over command line, or by downloading it. Updating this code to the latest commit would be done with `git pull`, inside the root directory.
```sh
git clone https://github.com/taulazer/Osu-Difficulty-Statistics.git
cd Osu-Difficulty-Statistics
```

### Building

Build configurations for the recommended IDEs (listed above) are included. You should use the provided Build/Run functionality of your IDE to get things going.

You can also build and run from the command-line with a single command:

```shell
dotnet run --project osu.Rulesets.Difficulty.Statistics.Desktop
```

If you are not interested in debugging, you can add `-c Release` to gain performance. In this case, you must replace `Debug` with `Release` in any commands mentioned in this document.

If the build fails, try to restore NuGet packages with `dotnet restore`.

### Testing with resource/framework modifications

Sometimes it may be necessary to cross-test changes in [osu-resources](https://github.com/ppy/osu-resources) or [osu-framework](https://github.com/ppy/osu-framework). This can be achieved by running some commands as documented on the [osu-resources](https://github.com/ppy/osu-resources/wiki/Testing-local-resources-checkout-with-other-projects) and [osu-framework](https://github.com/ppy/osu-framework/wiki/Testing-local-framework-checkout-with-other-projects) wiki pages.

## Contributions
All contributions are appreciated, as to improve the mode on its playability and functionality. As this gamemode isn't perfect, we would enjoy all additions to the code through bugfixing and ideas. Contributions should be done over an issue or a pull request, to give maintainers a chance to review changes to the codebase.

For new ideas and features, we would prefer for you to write an issue before trying to add it to show the maintainers.

## License
Osu-Difficulty-Statistics is licenced under the [MIT](https://opensource.org/licenses/MIT) License. For licensing information, refer to the [license file](https://github.com/taulazer/Osu-Difficulty-Statistics/blob/master/LICENSE) regarding what is permitted regarding the codebase of Osu-Difficulty-Statistics.

The licensing here does not directly apply to [osu!](https://github.com/ppy/osu), as it is bound to its own licensing. What is reflected in our licensing *may* not be allowed in the [osu!](https://github.com/ppy/osu) github repository.