[![tau](https://alten.s-ul.eu/pRr7vj6C.png)](https://github.com/Altenhh/tau "tau")
<div align="center">

[![release](https://img.shields.io/github/v/release/naoei/tau?style=flat-square)](https://github.com/Altenhh/tau/releases)
[![GitHub license](https://img.shields.io/github/license/Altenhh/tau.svg?style=flat-square)](https://github.com/Altenhh/tau/LICENSE)
![GitHub all releases](https://img.shields.io/github/downloads/naoei/tau/total?style=flat-square)
![GitHub release (latest by date)](https://img.shields.io/github/downloads/naoei/tau/latest/total?style=flat-square)
[![Continuous Integration](https://github.com/taulazer/tau/actions/workflows/ci.yml/badge.svg)](https://github.com/taulazer/tau/actions/workflows/ci.yml)
[![Crowdin](https://badges.crowdin.net/tau/localized.svg)](https://crowdin.com/project/tau)
[![community server](https://discordapp.com/api/guilds/689728872282849313/widget.png?style=shield)](https://discord.gg/7Y8GXAa)

*An [osu!](https://github.com/ppy/osu) ruleset. Sweeping beats with your scythe.*

</div>

[Original](https://deadlysprinklez.itch.io/tau) idea belonging to *pizzapip* and *[DeadlySprinklez](https://github.com/DeadlySprinklez)*.

[Art](https://github.com/taulazer/tau/wiki/Mascot) done by [Izeunne](https://www.fiverr.com/izeunne)

## Running the Gamemode
We have [prebuilt libraries](https://github.com/Altenhh/tau/releases) for users looking to play the mode without creating a development environment. All releases will work on all operating systems that *osu!* supports.

| [Latest Releases](https://github.com/Altenhh/tau/releases)
| ------------- |

### Instructions

From the osu settings menu scroll down till you see `Open osu! folder`, that button should take you under `%appdata%/osu`.

![open osu! folder](https://github.com/LumpBloom7/sentakki/wiki/images/Instuction1.png)

Copy the ruleset file into the `rulesets` directory, do make sure that duplicate copies of the ruleset is overwritten.

Once done, restart osu!lazer, if lazer is already open. Once lazer is started, you should see the ruleset alongside the standard rulesets on the toolbar at the top.

###### Do note that this instruction will only work with desktop devices.

## Development
When developing or debugging the tau codebase, a few prerequisites are required as following:
* An IDE that supports the C# language in automatic completion, and syntax highlighting; examples of such being [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) and above, or [JetBrains Rider](https://www.jetbrains.com/rider/).
* The [osu!framework](https://github.com/ppy/osu-framework/tree/master/osu.Framework), and [osu!](https://github.com/ppy/osu) codebases are added as dependencies for building

### Source Code
You are able to clone the repository over command line, or by downloading it. Updating this code to the latest commit would be done with `git pull`, inside the tau directory.
```sh
git clone https://github.com/Altenhh/tau.git
cd tau
```

### Building the Gamemode From Source
To build Tau, you will need to have [.NET 5](https://dotnet.microsoft.com/download) installed on your computer.

First, open a terminal and navigate to wherever you have the Tau source code downloaded. Once you are in the root of the repository, enter the directory named `osu.Game.Rulesets.Tau`.

Next, run the command `dotnet build` and wait for the project to be built. This shouldn't take very long.

Once the project has finished building, dotnet should tell you where the binary was built to (usually somewhere along the lines of ./tau/osu.Game.Rulesets.Tau/bin/Debug/netstandardx.x/). Find the .dll binary in the given location and follow the installation instructions above.

## Contributions
All contributions are appreciated, as to improve the mode on its playability and functionality. As this gamemode isn't perfect, we would enjoy all additions to the code through bugfixing and ideas. Contributions should be done over an issue or a pull request, to give maintainers a chance to review changes to the codebase.

For new ideas and features, we would prefer for you to write an issue before trying to add it to show the maintainers.

If you wish to help with localisation efforts, head over to [crowdin](https://crowdin.com/project/tau).

### Special thanks

Thanks to each and every contributor to [this project](https://github.com/taulazer/tau/graphs/contributors).

Thanks to all of those who have helped with localization efforts. Members includes (as of 2022, September 5th): [Akira](https://crowdin.com/profile/princessakira), [KalTa](https://crowdin.com/profile/kalta289), [Loreos](https://crowdin.com/profile/loreos), [MioStream_](https://crowdin.com/profile/miostream_), [Morco011](https://crowdin.com/profile/morcooooooo), [Nooraldeen Samir](https://crowdin.com/profile/noordlee), and [Peri](https://crowdin.com/profile/perigee).

Thanks to all of the amazing people within our discord community.

## License
tau is licenced under the [MIT](https://opensource.org/licenses/MIT) License. For licensing information, refer to the [license file](https://github.com/Altenhh/tau/blob/master/LICENSE) regarding what is permitted regarding the codebase of tau.

The licensing here does not directly apply to [osu!](https://github.com/ppy/osu), as it is bound to its own licensing. What is reflected in our licensing *may* not be allowed in the [osu!](https://github.com/ppy/osu) github repository.
